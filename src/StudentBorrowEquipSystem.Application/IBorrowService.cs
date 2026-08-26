using Domain;
using System;
using System.Collections.Generic;

namespace Application
{
    public interface IBorrowService
    {
        Borrow BorrowEquipment(Student student, Equipment equipment, DateTime dueDate);
        void ReturnEquipment(Guid borrowId, DateTime returnDate);
        IEnumerable<Borrow> GetActiveBorrows(string studentID);
        IEnumerable<Borrow> GetBorrowHistory(string studentID);
        IEnumerable<Borrow> GetOverdueBorrows();
    }
}