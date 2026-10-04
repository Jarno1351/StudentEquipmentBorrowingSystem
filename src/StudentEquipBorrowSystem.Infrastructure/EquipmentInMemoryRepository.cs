using Applications;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class EquipmentInMemoryRepository : IEquipmentRepository
    {
        private readonly List<Equipment> _equipment = new List<Equipment>();

        public async Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_equipment.ToList());
        }

        public async Task<Equipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_equipment.FirstOrDefault(e => e.Id == id));
        }

        public async Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            if (await ExistsAsync(equipment.Id, cancellationToken))
                throw new InvalidOperationException($"Equipment {equipment.Id} already exists.");

            _equipment.Add(equipment);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));

            var existing = await GetByIdAsync(equipment.Id, cancellationToken);
            if (existing == null)
                throw new InvalidOperationException($"Equipment {equipment.Id} was not found.");

            _equipment.Remove(existing);
            _equipment.Add(equipment);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_equipment.Any(e => e.Id == id));
        }
    }
}
