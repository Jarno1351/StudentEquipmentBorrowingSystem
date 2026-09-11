using Domain;
using System.Collections.Generic;

namespace Applications
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAll();
        Student GetById(string id);
        Student GetByStudentNumber(string studentNumber);
        void Add(Student student);
        void Update(Student student);
        void Delete(string id);
        bool Exists(string id);
    }
}