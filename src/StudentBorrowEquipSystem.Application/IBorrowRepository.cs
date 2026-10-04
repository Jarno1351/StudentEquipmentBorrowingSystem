using Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Applications
{
    public interface IBorrowRepository
    {
        Task<IEnumerable<Borrow>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Borrow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Borrow>> GetByStudentAsync(string studentID, CancellationToken cancellationToken = default);
        Task<IEnumerable<Borrow>> GetByEquipmentAsync(Guid equipmentId, CancellationToken cancellationToken = default);
        Task AddAsync(Borrow borrow, CancellationToken cancellationToken = default);
        Task UpdateAsync(Borrow borrow, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}