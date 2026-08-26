using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Staff
    {
        public string StaffID { get; private set; }
        public string FullName { get; private set; }
        public string? ContactNumber { get; private set; }
        public string? EmailAddress { get; private set; }
        public Staff(string staffID, string fullName, string? contactNumber, string? emailAddress)
        {
            StaffID = staffID;
            FullName = fullName;
            ContactNumber = contactNumber;
            EmailAddress = emailAddress;
        }
    }
}
