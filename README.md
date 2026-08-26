# Student Equipment Borrowing System

A layered (Clean/Onion-style) console application demonstrating a student borrowing
equipment from a school inventory, with validation handled entirely by the
application service rather than the UI or the database.

## 1. Solution Structure

- **Domain**
  The core of the system: entities (`Student`, `Equipment`, `Borrow`, `Staff`) and
  the `BorrowStatusEnum`. These classes own their own invariants — for example,
  `Student.CanBorrowEquipment()` and `Equipment.MarkAsBorrowed()`/`MarkAsReturned()`
  are the *only* way to change state, since every property setter is private. Domain
  has **no dependencies** on any other project; it doesn't know that a database, a
  console, or a UI even exists.

- **Application**
  The use-case layer. It contains:
  - Repository **interfaces** (`IStudentRepository`, `IEquipmentRepository`, `IBorrowRepository`) — contracts for fetching/storing domain objects, with no knowledge of *how* that storage happens.
  - **Services** (`BorrowService`, implementing `IBorrowService`) — orchestrate a use case by pulling in domain objects and repository interfaces, applying business rules (borrow limits, availability checks), and returning a result.

  Application depends only on **Domain**. It never references Infrastructure.

- **Infrastructure**
  Concrete, technology-specific implementations of the Application-layer
  interfaces — currently `StudentInMemoryRepository`, `EquipmentInMemoryRepository`,
  and `BorrowInMemoryRepository`, each backed by a simple in-memory `List<T>`. This
  is the layer that would later be swapped out or extended (e.g. for SQLite,
  a JSON file, or a REST API) without touching Domain or Application at all.

- **Tests**
  Not yet present as a separate project in this demo, but this is where automated
  tests would live — unit tests for `BorrowService`'s business rules (using the
  in-memory repositories or hand-written fakes as fast, dependency-free test
  doubles), and, if `Tests` referenced Infrastructure, integration tests against a
  real database. It would depend on Domain and Application (and optionally
  Infrastructure), the same way `Program.cs` does now.

## 2. Dependency Direction

```text
Executable / Future UI (Program.cs today, an Avalonia project later)
          │
          ▼
     Application
       │      ▲
       ▼      │
     Domain   │
              │
     Infrastructure
```

- The **Executable/UI** depends on **Application** (to call use cases) and on
  **Infrastructure** (only at startup, to wire up which concrete repository
  implementation to use — this is the composition root).
- **Application** depends on **Domain** only. It defines repository interfaces but
  never implements or references them.
- **Infrastructure** depends on both **Domain** (to work with entities) and
  **Application** (to implement its repository interfaces).
- **Domain** depends on nothing. It sits at the center and is the most stable,
  least-changing part of the solution.

The arrow that matters most: Application never points at Infrastructure. Infrastructure
points at Application (via implementing its interfaces), not the other way around —
this inversion is what lets the storage technology change freely.

## 3. Use Case Mapping

```text
Actor:                         Student
Use Case:                      Borrow Equipment
Application Service:           BorrowService.BorrowEquipment(student, equipment, dueDate)
Domain Objects Used:           Student, Equipment, Borrow, BorrowStatusEnum
Repository Interfaces Used:    IBorrowRepository (to persist the new Borrow record);
                                IStudentRepository, IEquipmentRepository
                                (used by the caller to look up the Student and
                                Equipment before invoking the service)
Infrastructure Implementations
Used:                           BorrowInMemoryRepository, StudentInMemoryRepository,
                                EquipmentInMemoryRepository
```

## 4. Reflection

**1. Why should the application service depend on a repository interface instead of directly depending on a database implementation?**
It keeps the business rules (borrow limits, availability checks) completely
independent of *how* data is stored. `BorrowService` can be unit tested with a fast
in-memory fake, and the real storage technology (SQLite, a file, an API) can be
chosen or changed later without touching a single line of business logic. It also
prevents Application from needing a reference to whatever database library
Infrastructure uses.

**2. Which parts of your current solution could remain unchanged if SQLite were added later?**
All of **Domain** and all of **Application** (interfaces and services) — they don't
know or care that storage changed. Only **Infrastructure** would change: new classes
like `StudentSqliteRepository` would be written to implement the existing
`IStudentRepository` interface. `Program.cs` would need a one-line change to
register the new implementation instead of the in-memory one.

**3. Which project would eventually contain Avalonia Views?**
A new UI/Presentation project (e.g. `StudentBorrowEquipSystem.UI`) that depends on
**Application** (to call services like `BorrowService`) and on **Infrastructure**
(only at startup, to wire dependencies). Views and ViewModels belong there —
never inside Domain, Application, or Infrastructure.

**4. Should an Avalonia button directly execute database queries? Why or why not?**
No. That would collapse the layers — the UI would end up owning business rules and
data-access code, making them impossible to test independently and impossible to
reuse if a second UI (e.g. a web API) were added later. The button's click handler
should call an Application service method (e.g. `BorrowEquipment(...)`), exactly
like `Program.cs` does now; the service still owns the validation, and a repository
implementation still owns the actual query.

**5. What part of your implementation represents the actual business operation requested by the actor?**
`BorrowService.BorrowEquipment(...)`. It's the single place where the real
business operation — "a student borrows a piece of equipment" — is defined:
checking eligibility, checking availability, creating the `Borrow` record, and
updating related domain state. Everything else (`Program.cs`, the repositories)
is supporting plumbing that feeds this operation or persists its result.