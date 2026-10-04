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
    public class EfEquipmentRepository : IEquipmentRepository
    {
        private readonly EquipmentBorrowingDbContext _context;

        public EfEquipmentRepository(EquipmentBorrowingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Equipment.ToListAsync(cancellationToken);
        }

        public async Task<Equipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Equipment.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            if (await ExistsAsync(equipment.Id, cancellationToken))
                throw new InvalidOperationException($"Equipment {equipment.Id} already exists.");

            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            var existing = await GetByIdAsync(equipment.Id, cancellationToken);
            if (existing == null)
                throw new InvalidOperationException($"Equipment {equipment.Id} was not found.");

            _context.Entry(existing).State = EntityState.Detached;
            _context.Equipment.Update(equipment);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Equipment.AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}
