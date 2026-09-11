using Domain;
using System;
using System.Collections.Generic;

namespace Applications
{
    public interface IEquipmentRepository
    {
        IEnumerable<Equipment> GetAll();
        Equipment GetById(Guid id);
        void Add(Equipment equipment);
        void Update(Equipment equipment);
        bool Exists(Guid id);
    }
}