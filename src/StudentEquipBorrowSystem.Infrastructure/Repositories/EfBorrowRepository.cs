using Applications;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EfBorrowRepository : IBorrowRepository
    {
        private readonly EquipmentBorrowingDbContext _context;

        public EfBorrowRepository(EquipmentBorrowingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Borrow>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Borrows
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .ToListAsync(cancellationToken);
        }

        public async Task<Borrow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Borrows
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Borrow>> GetByStudentAsync(string studentID, CancellationToken cancellationToken = default)
        {
            return await _context.Borrows
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .Where(b => b.StudentBorrower.StudentID == studentID)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Borrow>> GetByEquipmentAsync(Guid equipmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Borrows
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .Where(b => b.EquipmentBorrowed.Id == equipmentId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Borrow borrow, CancellationToken cancellationToken = default)
        {
            if (borrow == null)
                throw new ArgumentNullException(nameof(borrow));

            if (await ExistsAsync(borrow.Id, cancellationToken))
                throw new InvalidOperationException($"Borrow record {borrow.Id} already exists.");

            _context.Borrows.Add(borrow);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Borrow borrow, CancellationToken cancellationToken = default)
        {
            if (borrow == null)
                throw new ArgumentNullException(nameof(borrow));

            var existing = await GetByIdAsync(borrow.Id, cancellationToken);
            if (existing == null)
                throw new InvalidOperationException($"Borrow record {borrow.Id} was not found.");

            _context.Entry(existing).State = EntityState.Detached;
            _context.Borrows.Update(borrow);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Borrows.AnyAsync(b => b.Id == id, cancellationToken);
        }
    }
}
