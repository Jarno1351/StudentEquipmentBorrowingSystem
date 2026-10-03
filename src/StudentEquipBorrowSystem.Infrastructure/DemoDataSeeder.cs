using System;
using Applications;
using Domain;

namespace Infrastructure
{
    /// Seeds the in-memory repositories so the desktop app has data to work with.
    /// Lives in Infrastructure so the Desktop project never needs a Domain reference.
    public static class DemoDataSeeder
    {
        public static void Seed(IStudentRepository students, IEquipmentRepository equipment)
        {
            if (!students.Exists("2022303102"))
            {
                students.Add(new Student(
                    "2022303102", "Brill Jarn T. Barazan",
                    "College of Information Science in Computing", "BS in Information Technology",
                    "3rd Year", "09171234567", "s.barazan.brilljarn@cmu.edu.ph", "Valencia City, Bukidnon"));
            }

            foreach (var (name, type) in new[]
            {
                ("DSLR Camera", "Photography"), ("Projector", "AV Equipment"),
                ("Laptop", "Computer"), ("Speaker", "AV Equipment")
            })
            {
                equipment.Add(new Equipment(Guid.NewGuid(), name, type));
            }
        }
    }
}
