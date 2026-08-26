using Application;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public class BorrowInMemoryRepository : IBorrowRepository
    {
        private readonly List<Borrow> _borrows = new List<Borrow>();

        public IEnumerable<Borrow> GetAll()
        {
            return _borrows.ToList();
        }

        public Borrow GetById(Guid id)
        {
            return _borrows.FirstOrDefault(b => b.Id == id);
        }

        public IEnumerable<Borrow> GetByStudent(string studentID)
        {
            return _borrows.Where(b => b.StudentBorrower.StudentID == studentID).ToList();
        }

        public IEnumerable<Borrow> GetByEquipment(Guid equipmentId)
        {
            return _borrows.Where(b => b.EquipmentBorrowed.Id == equipmentId).ToList();
        }

        public void Add(Borrow borrow)
        {
            if (borrow == null)
                throw new ArgumentNullException(nameof(borrow));

            if (Exists(borrow.Id))
                throw new InvalidOperationException($"Borrow record {borrow.Id} already exists.");

            _borrows.Add(borrow);
        }

        public void Update(Borrow borrow)
        {
            if (borrow == null)
                throw new ArgumentNullException(nameof(borrow));

            var existing = GetById(borrow.Id);
            if (existing == null)
                throw new InvalidOperationException($"Borrow record {borrow.Id} was not found.");

            // Borrow's properties are private-set, so replace rather than mutate in place.
            _borrows.Remove(existing);
            _borrows.Add(borrow);
        }

        public bool Exists(Guid id)
        {
            return _borrows.Any(b => b.Id == id);
        }
    }
}