using Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Applications
{
    public interface IEquipmentRepository
    {
        Task<IEnumerable<Equipment>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Equipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Equipment that can be borrowed right now, ordered by name.
        /// Used so students are only offered items they can actually borrow.
        /// </summary>
        Task<IEnumerable<Equipment>> GetAvailableAsync(CancellationToken cancellationToken = default);

        Task AddAsync(Equipment equipment, CancellationToken cancellationToken = default);
        Task UpdateAsync(Equipment equipment, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}