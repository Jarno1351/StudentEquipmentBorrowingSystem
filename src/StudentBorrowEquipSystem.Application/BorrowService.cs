using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Applications
{
    public class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepository;

        public BorrowService(IBorrowRepository borrowRepository)
        {
            _borrowRepository = borrowRepository ?? throw new ArgumentNullException(nameof(borrowRepository));
        }

        public async Task<Borrow> BorrowEquipmentAsync(Student student, Equipment equipment, DateTime dueDate, CancellationToken cancellationToken = default)
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

            await _borrowRepository.AddAsync(borrow, cancellationToken);

            return borrow;
        }

        public async Task ReturnEquipmentAsync(Guid borrowId, DateTime returnDate, CancellationToken cancellationToken = default)
        {
            var borrow = await _borrowRepository.GetByIdAsync(borrowId, cancellationToken);
            if (borrow == null)
                throw new InvalidOperationException($"Borrow record {borrowId} was not found.");

            if (borrow.Status == BorrowStatusEnum.Returned)
                throw new InvalidOperationException("This equipment has already been returned.");

            borrow.MarkAsReturned(returnDate);
            borrow.EquipmentBorrowed.MarkAsReturned();
            borrow.StudentBorrower.DecrementBorrowedCount();

            await _borrowRepository.UpdateAsync(borrow, cancellationToken);
        }

        public async Task<IEnumerable<Borrow>> GetActiveBorrowsAsync(string studentID, CancellationToken cancellationToken = default)
        {
            var borrows = await _borrowRepository.GetByStudentAsync(studentID, cancellationToken);
            return borrows.Where(b => b.Status == BorrowStatusEnum.Active);
        }

        public async Task<IEnumerable<Borrow>> GetBorrowHistoryAsync(string studentID, CancellationToken cancellationToken = default)
        {
            return await _borrowRepository.GetByStudentAsync(studentID, cancellationToken);
        }

        public async Task<IEnumerable<Borrow>> GetOverdueBorrowsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            var borrows = await _borrowRepository.GetAllAsync(cancellationToken);
            return borrows.Where(b => b.Status == BorrowStatusEnum.Active && b.DueDate < now);
        }
    }
}