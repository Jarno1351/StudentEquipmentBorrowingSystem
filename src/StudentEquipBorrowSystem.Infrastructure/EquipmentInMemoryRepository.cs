using Application;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public class EquipmentInMemoryRepository : IEquipmentRepository
    {
        private readonly List<Equipment> _equipment = new List<Equipment>();

        public IEnumerable<Equipment> GetAll()
        {
            return _equipment.ToList();
        }

        public Equipment GetById(Guid id)
        {
            return _equipment.FirstOrDefault(e => e.Id == id);
        }

        public void Add(Equipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            if (Exists(equipment.Id))
                throw new InvalidOperationException($"Equipment {equipment.Id} already exists.");

            _equipment.Add(equipment);
        }

        public void Update(Equipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            var existing = GetById(equipment.Id);
            if (existing == null)
                throw new InvalidOperationException($"Equipment {equipment.Id} was not found.");

            _equipment.Remove(existing);
            _equipment.Add(equipment);
        }

        public bool Exists(Guid id)
        {
            return _equipment.Any(e => e.Id == id);
        }
    }
}