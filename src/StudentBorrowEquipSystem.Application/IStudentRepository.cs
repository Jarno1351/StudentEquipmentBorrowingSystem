using Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Applications
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Student?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Student?> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default);
        Task AddAsync(Student student, CancellationToken cancellationToken = default);
        Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    }
}