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
                .AsNoTracking()                       // read-only list
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .ToListAsync(cancellationToken);
        }

        // Intentionally TRACKED: BorrowService modifies the returned Borrow (MarkAsReturned)
        // and its Equipment, then saves. The change tracker records exactly what changed.
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
                .AsNoTracking()                       // read-only history
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .Where(b => b.StudentBorrower.StudentID == studentID)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Borrow>> GetByEquipmentAsync(Guid equipmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Borrows
                .AsNoTracking()                       // read-only lookup
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .Where(b => b.EquipmentBorrowed.Id == equipmentId)
                .ToListAsync(cancellationToken);
        }

        // LINQ query 2 - current borrowings together with the related student and equipment.
        // Include() becomes SQL joins to Students and Equipment; the filter and ordering run in
        // the database, so returned borrows and the history rows never reach the application:
        //   SELECT ... FROM Borrows JOIN Equipment ... JOIN Students ...
        //   WHERE Status <> 1 ORDER BY DueDate
        public async Task<IEnumerable<Borrow>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Borrows
                .AsNoTracking()                       // read-only list for display
                .Include(b => b.StudentBorrower)
                .Include(b => b.EquipmentBorrowed)
                .Where(b => b.Status != BorrowStatusEnum.Returned)
                .OrderBy(b => b.DueDate)
                .ToListAsync(cancellationToken);
        }

        // LINQ query 3 - how many items a student currently has on loan (the borrow-limit rule).
        // Translated to a single aggregate, instead of loading the student's whole history:
        //   SELECT COUNT(*) FROM Borrows WHERE StudentBorrower_StudentID = @id AND Status <> 1
        public async Task<int> CountActiveByStudentAsync(string studentID, CancellationToken cancellationToken = default)
        {
            return await _context.Borrows
                .CountAsync(b => b.StudentBorrower.StudentID == studentID
                                 && b.Status != BorrowStatusEnum.Returned,
                            cancellationToken);
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

            // Tracked entity (loaded by GetByIdAsync): the change tracker already knows
            // what changed, so SaveChanges writes only those columns.
            if (_context.Entry(borrow).State == EntityState.Detached)
            {
                if (!await ExistsAsync(borrow.Id, cancellationToken))
                    throw new InvalidOperationException($"Borrow record {borrow.Id} was not found.");

                // Detached copy: mark only the Borrow as modified, not the Student/Equipment graph.
                _context.Entry(borrow).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Borrows.AnyAsync(b => b.Id == id, cancellationToken);
        }
    }
}