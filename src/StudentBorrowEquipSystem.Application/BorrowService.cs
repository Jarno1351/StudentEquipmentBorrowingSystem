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
        private readonly IEquipmentRepository _equipmentRepository;

        public BorrowService(IBorrowRepository borrowRepository, IEquipmentRepository equipmentRepository)
        {
            _borrowRepository = borrowRepository ?? throw new ArgumentNullException(nameof(borrowRepository));
            _equipmentRepository = equipmentRepository ?? throw new ArgumentNullException(nameof(equipmentRepository));
        }

        public async Task<Borrow> BorrowEquipmentAsync(Student student, Equipment equipment, DateTime dueDate, CancellationToken cancellationToken = default)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            // The active count comes from the stored borrow records (one aggregate query),
            // not an in-memory counter, so the limit is still enforced after a restart.
            var activeCount = await _borrowRepository.CountActiveByStudentAsync(student.StudentID, cancellationToken);

            if (!student.CanBorrowEquipment(activeCount))
                throw new InvalidOperationException($"Student {student.StudentID} has reached the maximum borrow limit of {Student.MaxBorrowLimit}.");

            if (!equipment.IsAvailable)
                throw new InvalidOperationException($"Equipment '{equipment.EquipmentName}' is not available for borrowing.");

            var borrow = new Borrow(student, equipment, DateTime.Now, dueDate);
            await _borrowRepository.AddAsync(borrow, cancellationToken);

            // Persist the equipment status through its own repository.
            equipment.MarkAsBorrowed();
            await _equipmentRepository.UpdateAsync(equipment, cancellationToken);

            return borrow;
        }

        public async Task ReturnEquipmentAsync(Guid borrowId, DateTime returnDate, CancellationToken cancellationToken = default)
        {
            var borrow = await _borrowRepository.GetByIdAsync(borrowId, cancellationToken);
            if (borrow == null)
                throw new InvalidOperationException($"Borrow record {borrowId} was not found.");

            if (borrow.Status == BorrowStatusEnum.Returned)
                throw new InvalidOperationException("This equipment has already been returned.");

            var equipment = borrow.EquipmentBorrowed;

            borrow.MarkAsReturned(returnDate);
            await _borrowRepository.UpdateAsync(borrow, cancellationToken);

            // Persist the equipment status through its own repository.
            equipment.MarkAsReturned();
            await _equipmentRepository.UpdateAsync(equipment, cancellationToken);
        }

        // "Active" means not yet returned, the same definition used by the repository queries.
        public async Task<IEnumerable<Borrow>> GetActiveBorrowsAsync(string studentID, CancellationToken cancellationToken = default)
        {
            var borrows = await _borrowRepository.GetByStudentAsync(studentID, cancellationToken);
            return borrows.Where(b => b.Status != BorrowStatusEnum.Returned);
        }

        public async Task<IEnumerable<Borrow>> GetBorrowHistoryAsync(string studentID, CancellationToken cancellationToken = default)
        {
            return await _borrowRepository.GetByStudentAsync(studentID, cancellationToken);
        }

        // Overdue is computed from the due date: not yet returned and past its due date.
        public async Task<IEnumerable<Borrow>> GetOverdueBorrowsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;
            var active = await _borrowRepository.GetActiveAsync(cancellationToken);
            return active.Where(b => b.DueDate < now);
        }
    }
}