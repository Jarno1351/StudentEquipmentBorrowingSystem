using Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Applications
{
    public class StudentService
    {
        private readonly IStudentRepository _studentRepo;

        public StudentService(IStudentRepository studentRepo)
        {
            _studentRepo = studentRepo;
        }

        public async Task RegisterStudentAsync(string studentId, string fullName, string college, string course, string yearLevel, string? contactNumber, string? emailAddress, string? address, CancellationToken cancellationToken = default)
        {
            if (!await isStudentInfoValid(studentId, fullName, college, course, yearLevel, contactNumber, emailAddress, address, cancellationToken))
            {
                Console.WriteLine("Failed to register student. Please check the provided information.");
                return;
            }

            var student = new Student(
                studentId,
                fullName,
                college,
                course,
                yearLevel,
                contactNumber ?? "",
                emailAddress, address);

            await _studentRepo.AddAsync(student, cancellationToken);
        }

        private async Task<bool> isStudentInfoValid(string studentId, string fullName, string college, string course, string yearLevel, string? contactNumber, string? emailAddress, string? address, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(fullName))
            {
                Console.WriteLine("Student ID and Full Name are required.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(contactNumber) && !IsValidContactNumber(contactNumber))
            {
                Console.WriteLine("Invalid contact number format.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(emailAddress) && !IsValidEmailAddress(emailAddress))
            {
                Console.WriteLine("Invalid email address format.");
                return false;
            }
            if (contactNumber == null)
            {
                Console.WriteLine("Contact number is required.");
                return false;
            }
            if (course == null || string.IsNullOrWhiteSpace(course))
            {
                Console.WriteLine("Course is required.");
                return false;
            }
            if (yearLevel == null || string.IsNullOrWhiteSpace(yearLevel))
            {
                Console.WriteLine("Year level is required.");
                return false;
            }
            if (college == null || string.IsNullOrWhiteSpace(college))
            {
                Console.WriteLine("College is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                Console.WriteLine("Address filled is blank.");
                return false;
            }
            if (await _studentRepo.GetByIdAsync(studentId, cancellationToken) != null)
            {
                Console.WriteLine("Student with the same ID already exists.");
                return false;
            }

            return true;
        }

        private bool IsValidEmailAddress(string emailAddress)
        // validates the email address format
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(emailAddress);
                return addr.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidContactNumber(string contactNumber)
        // validates the contact number format: 11 digits, starting with 0
        {
            return contactNumber.Length == 11
                && contactNumber.StartsWith("0")
                && contactNumber.All(char.IsDigit);
        }

        public async Task<Student?> GetStudentByIdAsync(string studentId, CancellationToken cancellationToken = default)
        {
            return await _studentRepo.GetByIdAsync(studentId, cancellationToken);
        }
    }
}