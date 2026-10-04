using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Applications;
using Domain;

namespace Infrastructure
{
    public class StudentInMemoryRepository : IStudentRepository
    {
        private readonly List<Student> _students = new List<Student>();

        public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_students.ToList());
        }

        public async Task<Student?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_students.FirstOrDefault(s => s.StudentID == id));
        }

        public async Task<Student?> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_students.FirstOrDefault(s =>
                string.Equals(s.StudentID, studentNumber, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (await ExistsAsync(student.StudentID, cancellationToken))
                throw new InvalidOperationException($"Student with ID {student.StudentID} already exists.");

            _students.Add(student);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var existing = await GetByIdAsync(student.StudentID, cancellationToken);
            if (existing == null)
                throw new InvalidOperationException($"Student with ID {student.StudentID} was not found.");

            _students.Remove(existing);
            _students.Add(student);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var existing = await GetByIdAsync(id, cancellationToken);
            if (existing != null)
                _students.Remove(existing);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_students.Any(s => s.StudentID == id));
        }
    }
}
