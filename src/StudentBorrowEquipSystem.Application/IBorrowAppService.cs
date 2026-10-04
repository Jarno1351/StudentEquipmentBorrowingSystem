using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Applications.Dto;

namespace Applications
{
    public interface IBorrowAppService
    {
        /// Borrow equipment by IDs. Returns a DTO describing success or failure.
        Task<BorrowResultDto> BorrowEquipmentAsync(string studentId, Guid equipmentId, DateTime dueDate, CancellationToken cancellationToken = default);

        /// Return equipment by borrow id and return date. Returns result DTO.
        Task<BorrowResultDto> ReturnEquipmentAsync(Guid borrowId, DateTime returnDate, CancellationToken cancellationToken = default);

        /// Get all active borrowings in DTO form.
        Task<IEnumerable<BorrowDto>> GetActiveBorrowingsAsync(CancellationToken cancellationToken = default);
    }
}
