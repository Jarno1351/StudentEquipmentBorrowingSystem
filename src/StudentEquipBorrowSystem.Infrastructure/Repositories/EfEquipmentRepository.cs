using Applications;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class EfEquipmentRepository : IEquipmentRepository
    {
        private readonly EquipmentBorrowingDbContext _context;

        public EfEquipmentRepository(EquipmentBorrowingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<Equipment> GetAll()
        {
            return _context.Equipment.ToList();
        }

        public Equipment GetById(Guid id)
        {
            return _context.Equipment.FirstOrDefault(e => e.Id == id);
        }

        public void Add(Equipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            if (Exists(equipment.Id))
                throw new InvalidOperationException($"Equipment {equipment.Id} already exists.");

            _context.Equipment.Add(equipment);
            _context.SaveChanges();
        }

        public void Update(Equipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            var existing = GetById(equipment.Id);
            if (existing == null)
                throw new InvalidOperationException($"Equipment {equipment.Id} was not found.");

            _context.Entry(existing).State = EntityState.Detached;
            _context.Equipment.Update(equipment);
            _context.SaveChanges();
        }

        public bool Exists(Guid id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }
    }
}
