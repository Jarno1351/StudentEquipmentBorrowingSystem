# From LINQ to SQL


EF Core does not run LINQ. It translates the expression tree into SQL, sends the SQL to SQLite,
and turns the rows back into objects. The three queries below are the ones the application
actually uses; each sits behind a repository interface (`IEquipmentRepository`, `IBorrowRepository`),
so the services never see EF Core.

The SQL was captured with `LogTo(..., RelationalEventId.CommandExecuted)` using
`tests/TestProgram/SqlInspection.cs` (`dotnet run --project tests/TestProgram -- --sql`).

---

## Query 1 - Available equipment

**Requirement:** a student requests an available piece of equipment, so the Borrow form should
only offer items that can be borrowed right now.

### LINQ Query

`EfEquipmentRepository.GetAvailableAsync`

```csharp
_context.Equipment
    .AsNoTracking()
    .Where(e => e.IsAvailable)
    .OrderBy(e => e.EquipmentName)
    .ToListAsync(cancellationToken);
```

### Generated SQL

```sql
SELECT "e"."Id", "e"."EquipmentName", "e"."EquipmentType", "e"."IsAvailable"
FROM "Equipment" AS "e"
WHERE "e"."IsAvailable"
ORDER BY "e"."EquipmentName"
```

### Explanation

`Where` became the `WHERE` clause and `OrderBy` became `ORDER BY`. SQLite stores a `bool` as 0 or 1,
so the `IsAvailable` column can be used as the condition directly (older EF Core versions write
`= 1`). The `SELECT` lists every mapped column of `Equipment` because the query returns whole
`Equipment` entities. `AsNoTracking()` has no effect on the SQL; it only tells EF Core not to keep
the results in its change tracker, since this list is read-only. Nothing runs until `ToListAsync()`,
and the filtering and sorting happen inside SQLite, so unavailable rows are never transferred to
the application.

---

## Query 2 - Active borrowings with student and equipment

**Requirement:** the Active Borrowings page lists items currently on loan, showing who has them,
what they are and when they are due.

### LINQ Query

`EfBorrowRepository.GetActiveAsync`

```csharp
_context.Borrows
    .AsNoTracking()
    .Include(b => b.StudentBorrower)
    .Include(b => b.EquipmentBorrowed)
    .Where(b => b.Status != BorrowStatusEnum.Returned)
    .OrderBy(b => b.DueDate)
    .ToListAsync(cancellationToken);
```

### Generated SQL

```sql
SELECT "b"."Id", "b"."BorrowDate", "b"."DueDate", "b"."EquipmentBorrowed_Id", "b"."ReturnDate",
       "b"."Status", "b"."StudentBorrower_StudentID",
       "s"."StudentID", "s"."Address", "s"."College", "s"."ContactNumber", "s"."Course",
       "s"."EmailAddress", "s"."FullName", "s"."YearLevel",
       "e"."Id", "e"."EquipmentName", "e"."EquipmentType", "e"."IsAvailable"
FROM "Borrows" AS "b"
LEFT JOIN "Students" AS "s" ON "b"."StudentBorrower_StudentID" = "s"."StudentID"
INNER JOIN "Equipment" AS "e" ON "b"."EquipmentBorrowed_Id" = "e"."Id"
WHERE "b"."Status" <> 1
ORDER BY "b"."DueDate"
```

### Explanation

Each `Include` became a `JOIN` that brings the related row back in the same statement, so one query
returns borrows, students and equipment together instead of one extra query per borrow. The join
types come from the table design: `StudentBorrower_StudentID` is nullable in the migration, so
EF Core uses a `LEFT JOIN` for the student, while `EquipmentBorrowed_Id` is required, so the
equipment uses an `INNER JOIN`. The `WHERE` clause shows how the enum is stored: `Status` is an
`INTEGER` column (Active = 0, Returned = 1, Overdue = 2), so `!= BorrowStatusEnum.Returned`
becomes `<> 1`. That keeps both Active and Overdue borrows, because an overdue item is still on loan.
`ORDER BY DueDate` sorts correctly because SQLite stores dates as ISO-8601 text. This is the same
join the hand-written query 3 in `database-queries.sql` performs.

---

## Query 3 - Number of active borrows for a student

**Requirement:** a student may only have 3 items on loan at once, so `BorrowService` needs the
student's current count before it approves a new borrow.

### LINQ Query

`EfBorrowRepository.CountActiveByStudentAsync`

```csharp
_context.Borrows
    .CountAsync(b => b.StudentBorrower.StudentID == studentID
                     && b.Status != BorrowStatusEnum.Returned,
                cancellationToken);
```

### Generated SQL

```sql
-- parameter: @studentID = '2022303110'
SELECT COUNT(*)
FROM "Borrows" AS "b"
WHERE "b"."StudentBorrower_StudentID" = @studentID AND "b"."Status" <> 1
```

### Explanation

`CountAsync` with a condition became an aggregate: SQLite counts the matching rows and returns a
single number, instead of the application loading every borrow the student ever made (including
returned history) and counting in memory. The student's ID is passed as a query parameter
(`@studentID`), not pasted into the SQL text, which keeps the statement reusable and safe from
SQL injection. `b.StudentBorrower.StudentID` did not need a join: it is the foreign key stored in
the `Borrows` table, so EF Core compares the `StudentBorrower_StudentID` column directly.

---

## LINQ does not remove SQL concepts

| LINQ | SQL concept it becomes |
|---|---|
| `Where(...)` | `WHERE` filter |
| `OrderBy(...)` | `ORDER BY` |
| `Include(...)` | `JOIN` (`INNER` for a required relationship, `LEFT` for an optional one) |
| `CountAsync(...)` | `COUNT(*)` aggregate |
| a variable used in a lambda | a query parameter |
| `AsNoTracking()` | nothing in SQL; it only changes what EF Core remembers afterwards |

Understanding the SQL still matters. The join type depends on whether a foreign key is nullable,
the enum and date comparisons depend on how the columns are stored (`INTEGER` and `TEXT`), and
whether a query is fast depends on indexes such as `IX_Borrows_Status`, `IX_Borrows_DueDate` and
`IX_Equipment_IsAvailable` that were defined in the entity configurations. `EXPLAIN QUERY PLAN`
in SQLite shows whether an index is used.