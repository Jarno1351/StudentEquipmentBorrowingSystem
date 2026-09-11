using System;
using Applications.Dto;

namespace Applications
{
    public interface IBorrowAppService
    {
        /// Borrow equipment by IDs. Returns a DTO describing success or failure.
        BorrowResultDto BorrowEquipment(string studentId, Guid equipmentId, DateTime dueDate);
    }
}
