using Domain;

namespace Application
{
    public class StudentService
    {
        private readonly IStudentRepository _studentRepo;

        public StudentService(IStudentRepository studentRepo)
        {
            _studentRepo = studentRepo;
        }
        public void RegisterStudent(string studentId, string fullName, string college, string course, string yearLevel, string? contactNumber, string? emailAddress, string? address)
        {
            if (!isStudentInfoValid(studentId, fullName, college, course, yearLevel, contactNumber, emailAddress, address))
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

            _studentRepo.Add(student);

        }

        private bool isStudentInfoValid(string studentId, string fullName, string college, string course, string yearLevel, string? contactNumber, string? emailAddress, string? address)
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
            if (_studentRepo.GetById(studentId) != null)
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
        // validates the contact number format
        {
            if (contactNumber.Length != 11 && !contactNumber.StartsWith("0"))
            { return false; }
            return true;
        }

        public Student? GetStudentById(string studentId)
        {
            return _studentRepo.GetById(studentId);
        }

    }
}
