using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Applications;
using Domain;

namespace Infrastructure
{
    /// Seeds the repositories so the desktop app has data to work with.
    /// Safe to run on every launch: it only adds data that is missing.
    /// Lives in Infrastructure so the Desktop project never needs a Domain reference.
    public static class DemoDataSeeder
    {
        public static async Task SeedAsync(IStudentRepository students, IEquipmentRepository equipment, CancellationToken cancellationToken = default)
        {
            if (!await students.ExistsAsync("2022303102", cancellationToken))
            {
                await students.AddAsync(new Student(
                    "2022303102", "Brill Jarn T. Barazan",
                    "College of Information Science in Computing", "BS in Information Technology",
                    "3rd Year", "09171234567", "s.barazan.brilljarn@cmu.edu.ph", "Valencia City, Bukidnon"), cancellationToken);
            }

            // Equipment IDs are generated, so check for existing records instead of a specific ID.
            var existingEquipment = await equipment.GetAllAsync(cancellationToken);
            if (existingEquipment.Any())
                return;

            foreach (var (name, type) in new[]
            {
                ("DSLR Camera", "Photography"), ("Projector", "AV Equipment"),
                ("Laptop", "Computer"), ("Speaker", "AV Equipment")
            })
            {
                await equipment.AddAsync(new Equipment(Guid.NewGuid(), name, type), cancellationToken);
            }
        }
    }
}