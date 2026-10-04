namespace Domain
{
    public class Student
    {
        public const int MaxBorrowLimit = 3;

        public string StudentID { get; private set; }
        public string FullName { get; private set; }
        public string College { get; private set; }
        public string Course { get; private set; }
        public string YearLevel { get; private set; }
        public string ContactNumber { get; private set; }
        public string? EmailAddress { get; private set; }
        public string? Address { get; private set; }

        // Parameterless constructor for EF Core
        private Student() { }

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

        /// <summary>
        /// The borrow limit rule. The active count is supplied by the Application layer
        /// (derived from stored Borrow records), so the rule works with any storage
        /// and survives application restarts.
        /// </summary>
        public bool CanBorrowEquipment(int activeBorrowCount)
        {
            return activeBorrowCount < MaxBorrowLimit;
        }
    }
}