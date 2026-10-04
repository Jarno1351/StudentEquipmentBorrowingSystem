using Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Applications
{
    public interface IBorrowService
    {
        Task<Borrow> BorrowEquipmentAsync(Student student, Equipment equipment, DateTime dueDate, CancellationToken cancellationToken = default);
        Task ReturnEquipmentAsync(Guid borrowId, DateTime returnDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<Borrow>> GetActiveBorrowsAsync(string studentID, CancellationToken cancellationToken = default);
        Task<IEnumerable<Borrow>> GetBorrowHistoryAsync(string studentID, CancellationToken cancellationToken = default);
        Task<IEnumerable<Borrow>> GetOverdueBorrowsAsync(CancellationToken cancellationToken = default);
    }
}