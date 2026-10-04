using Applications;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class EfBorrowRepository : IBorrowRepository
    {
        private readonly EquipmentBorrowingDbContext _context;

        public EfBorrowRepository(EquipmentBorrowingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<Borrow> GetAll()
        {
            return _context.Borrows
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .ToList();
        }

        public Borrow GetById(Guid id)
        {
            return _context.Borrows
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .FirstOrDefault(b => b.Id == id);
        }

        public IEnumerable<Borrow> GetByStudent(string studentID)
        {
            return _context.Borrows
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .Where(b => b.StudentBorrower.StudentID == studentID)
                .ToList();
        }

        public IEnumerable<Borrow> GetByEquipment(Guid equipmentId)
        {
            return _context.Borrows
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .Where(b => b.EquipmentBorrowed.Id == equipmentId)
                .ToList();
        }

        public void Add(Borrow borrow)
        {
            if (borrow == null)
                throw new ArgumentNullException(nameof(borrow));

            if (Exists(borrow.Id))
                throw new InvalidOperationException($"Borrow record {borrow.Id} already exists.");

            _context.Borrows.Add(borrow);
            _context.SaveChanges();
        }

        public void Update(Borrow borrow)
        {
            if (borrow == null)
                throw new ArgumentNullException(nameof(borrow));

            var existing = GetById(borrow.Id);
            if (existing == null)
                throw new InvalidOperationException($"Borrow record {borrow.Id} was not found.");

            _context.Entry(existing).State = EntityState.Detached;
            _context.Borrows.Update(borrow);
            _context.SaveChanges();
        }

        public bool Exists(Guid id)
        {
            return _context.Borrows.Any(b => b.Id == id);
        }
    }
}
