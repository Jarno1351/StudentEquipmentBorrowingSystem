using Applications;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class BorrowInMemoryRepository : IBorrowRepository
    {
        private readonly List<Borrow> _borrows = new List<Borrow>();

        public async Task<IEnumerable<Borrow>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_borrows.ToList());
        }

        public async Task<Borrow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_borrows.FirstOrDefault(b => b.Id == id));
        }

        public async Task<IEnumerable<Borrow>> GetByStudentAsync(string studentID, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_borrows.Where(b => b.StudentBorrower.StudentID == studentID).ToList());
        }

        public async Task<IEnumerable<Borrow>> GetByEquipmentAsync(Guid equipmentId, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_borrows.Where(b => b.EquipmentBorrowed.Id == equipmentId).ToList());
        }

        public async Task AddAsync(Borrow borrow, CancellationToken cancellationToken = default)
        {
            if (borrow == null)
                throw new ArgumentNullException(nameof(borrow));

            if (await ExistsAsync(borrow.Id, cancellationToken))
                throw new InvalidOperationException($"Borrow record {borrow.Id} already exists.");

            _borrows.Add(borrow);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Borrow borrow, CancellationToken cancellationToken = default)
        {
            if (borrow == null)
                throw new ArgumentNullException(nameof(borrow));

            var existing = await GetByIdAsync(borrow.Id, cancellationToken);
            if (existing == null)
                throw new InvalidOperationException($"Borrow record {borrow.Id} was not found.");

            _borrows.Remove(existing);
            _borrows.Add(borrow);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_borrows.Any(b => b.Id == id));
        }
    }
}
