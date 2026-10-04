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
    public class EfStudentRepository : IStudentRepository
    {
        private readonly EquipmentBorrowingDbContext _context;

        public EfStudentRepository(EquipmentBorrowingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AsNoTracking()                       // only feeds the student dropdown
                .ToListAsync(cancellationToken);
        }

        // Intentionally TRACKED: the Student is attached to a new Borrow in BorrowService.
        public async Task<Student?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.StudentID == id, cancellationToken);
        }

        public async Task<Student?> GetByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Students.FirstOrDefaultAsync(s =>
                string.Equals(s.StudentID, studentNumber, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (await ExistsAsync(student.StudentID, cancellationToken))
                throw new InvalidOperationException($"Student with ID {student.StudentID} already exists.");

            _context.Students.Add(student);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (_context.Entry(student).State == EntityState.Detached)
            {
                if (!await ExistsAsync(student.StudentID, cancellationToken))
                    throw new InvalidOperationException($"Student with ID {student.StudentID} was not found.");

                _context.Students.Update(student);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var student = await GetByIdAsync(id, cancellationToken);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Students.AnyAsync(s => s.StudentID == id, cancellationToken);
        }
    }
}