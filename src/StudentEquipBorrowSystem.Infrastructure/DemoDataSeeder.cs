using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Applications;
using Domain;

namespace Infrastructure
{
    /// <summary>
    /// Controlled startup seeding. Safe to run on every launch: it only ADDS records that
    /// are missing and never deletes or overwrites existing data.
    ///
    /// Works through the repository interfaces, so it behaves the same for SQLite and for
    /// the in-memory repositories.
    ///
    /// Demo data produced (on an empty database):
    ///   Students   : 5 (one with no email, to show optional fields)
    ///   Equipment  : 8 (4 available, 4 borrowed)
    ///   Borrowings : Juan  - 3 active borrows (AT the limit  -> a 4th borrow fails)
    ///                Maria - 1 active borrow, past its due date (OVERDUE)
    ///                Pedro - 1 returned borrow (history; the projector is available again)
    ///                Brill and Ana - no borrows (use them for the successful borrow demo)
    /// </summary>
    public static class DemoDataSeeder
    {
        // Fixed equipment IDs make seeding idempotent (each item can be checked by ID)
        // and keep the IDs in database-queries.sql stable.
        private static Guid EquipmentId(int n) => new Guid($"00000000-0000-0000-0000-{n:D12}");

        private static readonly Guid Camera = EquipmentId(1);
        private static readonly Guid Projector = EquipmentId(2);
        private static readonly Guid Laptop = EquipmentId(3);
        private static readonly Guid Speaker = EquipmentId(4);
        private static readonly Guid Tripod = EquipmentId(5);

        public static async Task SeedAsync(
            IStudentRepository students,
            IEquipmentRepository equipment,
            IBorrowRepository borrows,
            CancellationToken cancellationToken = default)
        {
            await SeedStudentsAsync(students, cancellationToken);
            await SeedEquipmentAsync(equipment, cancellationToken);
            await SeedBorrowsAsync(students, equipment, borrows, cancellationToken);
        }

        private static async Task SeedStudentsAsync(IStudentRepository students, CancellationToken ct)
        {
            const string cics = "College of Information Science in Computing";

            var seed = new (string Id, string Name, string College, string Course, string Year, string Contact, string? Email, string Address)[]
            {
                ("2022303102", "Brill Jarn T. Barazan", cics, "BS in Information Technology", "3rd Year", "09171234567", "s.barazan.brilljarn@cmu.edu.ph", "Valencia City, Bukidnon"),
                ("2024303110", "Travis S. Vergel de Dios",        cics, "BS in Information Technology", "3rd Year", "09181234501", "s.vergeldedios.travis_2024@cmu.edu.ph",         "Valencia, Bukidnon"),
                ("2023304215", "Maria Santos",          cics, "BS in Computer Science",       "2nd Year", "09191234502", "maria.santos@example.com",          "Cagayan de Oro City"),
                ("2021302087", "Pedro Reyes",           "College of Engineering", "BS in Electronics Engineering", "4th Year", "09201234503", "pedro.reyes@example.com", "Valencia City, Bukidnon"),
                ("2024305331", "Ana Garcia",            cics, "BS in Information Technology", "1st Year", "09211234504", null,                                 "Maramag, Bukidnon"),
            };

            foreach (var s in seed)
            {
                if (await students.ExistsAsync(s.Id, ct))
                    continue;

                await students.AddAsync(new Student(
                    s.Id, s.Name, s.College, s.Course, s.Year, s.Contact, s.Email, s.Address), ct);
            }
        }

        private static async Task SeedEquipmentAsync(IEquipmentRepository equipment, CancellationToken ct)
        {
            var seed = new (int N, string Name, string Type)[]
            {
                (1, "DSLR Camera",         "Photography"),
                (2, "Projector",           "AV Equipment"),
                (3, "Laptop",              "Computer"),
                (4, "Speaker",             "AV Equipment"),
                (5, "Tripod",              "Photography"),
                (6, "Wireless Microphone", "AV Equipment"),
                (7, "Webcam",              "Computer"),
                (8, "Extension Cord",      "Electrical"),
            };

            // Equipment is always inserted as available. Borrowed status is applied afterwards
            // through MarkAsBorrowed + UpdateAsync (see SeedBorrowsAsync).
            foreach (var e in seed)
            {
                var id = EquipmentId(e.N);
                if (await equipment.ExistsAsync(id, ct))
                    continue;

                await equipment.AddAsync(new Equipment(id, e.Name, e.Type), ct);
            }
        }

        private static async Task SeedBorrowsAsync(
            IStudentRepository students,
            IEquipmentRepository equipment,
            IBorrowRepository borrows,
            CancellationToken ct)
        {
            // Borrow IDs are generated, so borrow records are seeded only into a database
            // that has none yet. Returned borrows stay as history, so this never re-seeds
            // after the first run.
            if ((await borrows.GetAllAsync(ct)).Any())
                return;

            var juan = await students.GetByIdAsync("2022303110", ct);
            var maria = await students.GetByIdAsync("2023304215", ct);
            var pedro = await students.GetByIdAsync("2021302087", ct);

            var camera = await equipment.GetByIdAsync(Camera, ct);
            var projector = await equipment.GetByIdAsync(Projector, ct);
            var laptop = await equipment.GetByIdAsync(Laptop, ct);
            var speaker = await equipment.GetByIdAsync(Speaker, ct);
            var tripod = await equipment.GetByIdAsync(Tripod, ct);

            if (juan == null || maria == null || pedro == null ||
                camera == null || projector == null || laptop == null || speaker == null || tripod == null)
                return;

            var now = DateTime.Now;

            // Juan: 3 active borrows = at the borrow limit. Borrowing a 4th item must fail.
            await AddActiveBorrowAsync(juan, laptop, now.AddDays(-1), now.AddDays(2), equipment, borrows, ct);
            await AddActiveBorrowAsync(juan, speaker, now.AddDays(-1), now.AddDays(2), equipment, borrows, ct);
            await AddActiveBorrowAsync(juan, tripod, now.AddDays(-1), now.AddDays(2), equipment, borrows, ct);

            // Maria: 1 active borrow that is past its due date (overdue).
            await AddActiveBorrowAsync(maria, camera, now.AddDays(-5), now.AddDays(-2), equipment, borrows, ct);

            // Pedro: 1 borrow that has already been returned (history). The projector stays available.
            var returned = new Borrow(pedro, projector, now.AddDays(-7), now.AddDays(-4));
            returned.MarkAsReturned(now.AddDays(-4));
            await borrows.AddAsync(returned, ct);
        }

        private static async Task AddActiveBorrowAsync(
            Student student,
            Equipment item,
            DateTime borrowDate,
            DateTime dueDate,
            IEquipmentRepository equipment,
            IBorrowRepository borrows,
            CancellationToken ct)
        {
            await borrows.AddAsync(new Borrow(student, item, borrowDate, dueDate), ct);

            item.MarkAsBorrowed();
            await equipment.UpdateAsync(item, ct);
        }
    }
}