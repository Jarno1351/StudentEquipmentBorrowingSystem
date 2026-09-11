using Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applications
{
    public class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepository;

        public BorrowService(IBorrowRepository borrowRepository)
        {
            _borrowRepository = borrowRepository ?? throw new ArgumentNullException(nameof(borrowRepository));
        }

        public Borrow BorrowEquipment(Student student, Equipment equipment, DateTime dueDate)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            if (!student.CanBorrowEquipment())
                throw new InvalidOperationException($"Student {student.StudentID} has reached the maximum borrow limit.");

            if (!equipment.IsAvailable)
                throw new InvalidOperationException($"Equipment '{equipment.EquipmentName}' is not available for borrowing.");

            var borrow = new Borrow(student, equipment, DateTime.Now, dueDate);

            equipment.MarkAsBorrowed();
            student.IncrementBorrowedCount();

            _borrowRepository.Add(borrow);

            return borrow;
        }

        public void ReturnEquipment(Guid borrowId, DateTime returnDate)
        {
            var borrow = _borrowRepository.GetById(borrowId);
            if (borrow == null)
                throw new InvalidOperationException($"Borrow record {borrowId} was not found.");

            if (borrow.Status == BorrowStatusEnum.Returned)
                throw new InvalidOperationException("This equipment has already been returned.");

            borrow.MarkAsReturned(returnDate);
            borrow.EquipmentBorrowed.MarkAsReturned();
            borrow.StudentBorrower.DecrementBorrowedCount();

            _borrowRepository.Update(borrow);
        }

        public IEnumerable<Borrow> GetActiveBorrows(string studentID)
        {
            return _borrowRepository.GetByStudent(studentID)
                .Where(b => b.Status == BorrowStatusEnum.Active);
        }

        public IEnumerable<Borrow> GetBorrowHistory(string studentID)
        {
            return _borrowRepository.GetByStudent(studentID);
        }

        public IEnumerable<Borrow> GetOverdueBorrows()
        {
            var now = DateTime.Now;
            return _borrowRepository.GetAll()
                .Where(b => b.Status == BorrowStatusEnum.Active && b.DueDate < now);
        }
    }
}