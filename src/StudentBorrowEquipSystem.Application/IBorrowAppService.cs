using System;
using Applications.Dto;

namespace Applications
{
    public interface IBorrowAppService
    {
        /// Borrow equipment by IDs. Returns a DTO describing success or failure.
        BorrowResultDto BorrowEquipment(string studentId, Guid equipmentId, DateTime dueDate);

        /// Return equipment by borrow id and return date. Returns result DTO.
        BorrowResultDto ReturnEquipment(Guid borrowId, DateTime returnDate);

        /// Get all active borrowings in DTO form.
        System.Collections.Generic.IEnumerable<BorrowDto> GetActiveBorrowings();
    }
}
