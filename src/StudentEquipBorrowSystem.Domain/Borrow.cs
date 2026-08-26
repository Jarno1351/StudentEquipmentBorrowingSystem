using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Borrow
    {
        public Guid Id { get; private set; }
        public Student StudentBorrower { get; private set; }
        public Equipment EquipmentBorrowed { get; private set; }
        public DateTime BorrowDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime? ReturnDate { get; private set; }
        public BorrowStatusEnum Status { get; private set; }

        public Borrow(Student studentBorrower, Equipment equipmentBorrowed, DateTime borrowDate, DateTime dueDate)
        {
            Id = Guid.NewGuid();
            StudentBorrower = studentBorrower;
            EquipmentBorrowed = equipmentBorrowed;
            BorrowDate = borrowDate;
            DueDate = dueDate;
            Status = BorrowStatusEnum.Active;
        }

        public void MarkAsReturned(DateTime returnDate)
        {
            ReturnDate = returnDate;
            Status = BorrowStatusEnum.Returned;
        }

        public void MarkAsOverdue()
        {
            if (Status == BorrowStatusEnum.Active)
                Status = BorrowStatusEnum.Overdue;
        }
    }
}
