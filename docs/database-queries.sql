-- SQL Queries for Student Equipment Borrowing System
-- These queries demonstrate understanding of the relational database structure
-- and SQL operations. They are documentation examples that can be used to query
-- the SQLite database created by EF Core migrations.
--
-- Note: The actual table and column names are determined by EF Core's mapping
-- configuration defined in the Configurations folder.

-- ============================================================================
-- 1. BASIC RETRIEVAL: Get all equipment
-- ============================================================================
SELECT *
FROM Equipment;


-- ============================================================================
-- 2. FILTERING: Retrieve only currently available equipment
-- ============================================================================
SELECT Id, EquipmentName, EquipmentType, IsAvailable
FROM Equipment
WHERE IsAvailable = 1
ORDER BY EquipmentName;


-- ============================================================================
-- 3. JOIN: Active borrowings with student and equipment information
-- ============================================================================
-- This query retrieves all active borrowings together with the corresponding
-- student and equipment details. Status = 0 represents BorrowStatusEnum.Active
SELECT
	s.FullName AS Student,
	e.EquipmentName AS Equipment,
	b.BorrowDate AS Borrowed,
	b.DueDate AS Due,
	b.Status
FROM Borrows AS b
INNER JOIN Students AS s
	ON s.StudentID = b.StudentBorrower_StudentID
INNER JOIN Equipment AS e
	ON e.Id = b.EquipmentBorrowed_Id
WHERE b.Status = 0
ORDER BY b.DueDate ASC;


-- ============================================================================
-- 4. AGGREGATE: Number of active borrowings per student
-- ============================================================================
-- This aggregate query shows how many items each student currently has borrowed.
-- Includes students with zero active borrowings.
SELECT
	s.StudentID,
	s.FullName AS Student,
	COUNT(b.Id) AS ActiveBorrowings
FROM Students AS s
LEFT JOIN Borrows AS b
	ON s.StudentID = b.StudentBorrower_StudentID
	AND b.Status = 0
GROUP BY s.StudentID, s.FullName
ORDER BY ActiveBorrowings DESC, s.FullName ASC;


-- ============================================================================
-- 4b. AGGREGATE (ALTERNATIVE): Equipment usage statistics
-- ============================================================================
-- This query shows how many times each piece of equipment has been borrowed.
SELECT
	e.EquipmentName,
	e.EquipmentType,
	COUNT(b.Id) AS TimesBoRowred,
	CASE
		WHEN e.IsAvailable = 1 THEN 'Available'
		ELSE 'Borrowed'
	END AS CurrentStatus
FROM Equipment AS e
LEFT JOIN Borrows AS b
	ON e.Id = b.EquipmentBorrowed_Id
GROUP BY e.Id, e.EquipmentName, e.EquipmentType, e.IsAvailable
ORDER BY TimesBoRowred DESC;


-- ============================================================================
-- 4c. AGGREGATE (ALTERNATIVE): Count of all active borrowings
-- ============================================================================
-- Simple count of items currently checked out.
SELECT COUNT(*) AS ActiveBorrowingCount
FROM Borrows
WHERE Status = 0;


-- ============================================================================
-- 5. UPDATE: Mark equipment as available (returned from a student)
-- ============================================================================
-- This statement changes the IsAvailable field for a specific piece of equipment.
-- In the application, this is handled by the repository when a borrow is marked
-- as returned, but this demonstrates the underlying SQL operation.
UPDATE Equipment
SET IsAvailable = 1
WHERE Id = '00000000-0000-0000-0000-000000000001';


-- ============================================================================
-- BONUS QUERIES: Additional useful queries for the system
-- ============================================================================

-- Find overdue borrowings
-- The application computes "overdue" from the due date (BorrowService.GetOverdueBorrowsAsync):
-- a borrowing is overdue when it is still Active (Status = 0) and its due date has passed.
-- Status = 2 (Overdue) is never written by the application, so it is not used here.
SELECT
	s.FullName AS Student,
	s.ContactNumber,
	e.EquipmentName,
	b.DueDate,
	CAST((julianday('now', 'localtime') - julianday(b.DueDate)) AS INTEGER) AS DaysOverdue
FROM Borrows AS b
INNER JOIN Students AS s ON s.StudentID = b.StudentBorrower_StudentID
INNER JOIN Equipment AS e ON e.Id = b.EquipmentBorrowed_Id
WHERE b.Status = 0
  AND b.DueDate < datetime('now', 'localtime')
ORDER BY b.DueDate ASC;


-- Find borrowing history for a specific student
SELECT
	e.EquipmentName,
	b.BorrowDate,
	b.DueDate,
	b.ReturnDate,
	CASE
		WHEN b.Status = 0 THEN 'Active'
		WHEN b.Status = 1 THEN 'Returned'
		WHEN b.Status = 2 THEN 'Overdue'
		ELSE 'Unknown'
	END AS BorrowStatus
FROM Borrows AS b
INNER JOIN Equipment AS e ON e.Id = b.EquipmentBorrowed_Id
WHERE b.StudentBorrower_StudentID = '2022303102'
ORDER BY b.BorrowDate DESC;