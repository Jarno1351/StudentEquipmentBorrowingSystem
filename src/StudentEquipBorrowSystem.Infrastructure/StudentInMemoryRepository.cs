using System;
using System.Collections.Generic;
using System.Linq;
using Application;
using Domain;

namespace Infrastructure
{
    public class StudentInMemoryRepository : IStudentRepository
    {
        private readonly List<Student> _students = new List<Student>();

        public IEnumerable<Student> GetAll()
        {
            return _students.ToList();
        }

        public Student GetById(string studentID)
        {
            return _students.FirstOrDefault(s => s.StudentID == studentID);
        }

        public Student GetByStudentNumber(string studentID)
        {
            return _students.FirstOrDefault(s =>
                string.Equals(s.StudentID, studentID, StringComparison.OrdinalIgnoreCase));
        }

        public void Add(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (Exists(student.StudentID))
                throw new InvalidOperationException($"Student with ID {student.StudentID} already exists.");

            _students.Add(student);
        }

        public void Update(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var existing = GetById(student.StudentID);
            if (existing == null)
                throw new InvalidOperationException($"Student with ID {student.StudentID} was not found.");

            _students.Remove(existing);
            _students.Add(student);
        }

        public void Delete(string studentID)
        {
            var existing = GetById(studentID);
            if (existing != null)
                _students.Remove(existing);
        }

        public bool Exists(string studentID)
        {
            return _students.Any(s => s.StudentID == studentID);
        }
    }
}