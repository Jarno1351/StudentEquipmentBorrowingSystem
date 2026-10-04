using Applications;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class EfStudentRepository : IStudentRepository
    {
        private readonly EquipmentBorrowingDbContext _context;

        public EfStudentRepository(EquipmentBorrowingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<Student> GetAll()
        {
            return _context.Students.ToList();
        }

        public Student GetById(string id)
        {
            return _context.Students.FirstOrDefault(s => s.StudentID == id);
        }

        public Student GetByStudentNumber(string studentNumber)
        {
            return _context.Students.FirstOrDefault(s =>
                string.Equals(s.StudentID, studentNumber, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (Exists(student.StudentID))
                throw new InvalidOperationException($"Student with ID {student.StudentID} already exists.");

            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void Update(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var existing = GetById(student.StudentID);
            if (existing == null)
                throw new InvalidOperationException($"Student with ID {student.StudentID} was not found.");

            _context.Entry(existing).State = EntityState.Detached;
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var student = GetById(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
        }

        public bool Exists(string id)
        {
            return _context.Students.Any(s => s.StudentID == id);
        }
    }
}
