using Domain;
using System;
using System.Collections.Generic;

namespace Application
{
    public interface IBorrowRepository
    {
        IEnumerable<Borrow> GetAll();
        Borrow GetById(Guid id);
        IEnumerable<Borrow> GetByStudent(string studentID);
        IEnumerable<Borrow> GetByEquipment(Guid equipmentId);
        void Add(Borrow borrow);
        void Update(Borrow borrow);
        bool Exists(Guid id);
    }
}