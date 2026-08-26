using System.Security.Cryptography.X509Certificates;

namespace Domain
{
    public class Student
    {
        public string StudentID { get; private set; }
        public string FullName { get; private set; }
        public string College { get; private set; }
        public string Course { get; private set; }
        public string YearLevel { get; private set; }
        public string ContactNumber { get; private set; }
        public string? EmailAddress { get; private set; }
        public string? Address { get; private set; }

        private int currentBorrowedEquipmentCount = 0;

        public bool CanBorrowEquipment()
        {
            int maxBorrowLimit = 3; // Set the maximum borrow limit for students
            return currentBorrowedEquipmentCount < maxBorrowLimit;
        }

        public void IncrementBorrowedCount()
        {
            currentBorrowedEquipmentCount++;
        }

        public void DecrementBorrowedCount()
        {
            if (currentBorrowedEquipmentCount > 0)
                currentBorrowedEquipmentCount--;
        }

        public Student(string studentID, string fullName, string college, string course, string yearLevel, string contactNumber, string? emailAddress, string? address)
        {
            StudentID = studentID;
            FullName = fullName;
            College = college;
            Course = course;
            YearLevel = yearLevel;
            ContactNumber = contactNumber;
            EmailAddress = emailAddress;
            Address = address;
        }
    }
}
