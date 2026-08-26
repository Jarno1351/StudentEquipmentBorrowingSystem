using System;
using Domain;
using Application;
using Infrastructure;

namespace ConsoleDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Student Equipment Borrowing System — Demo ===\n");

            // ---- Composition root: repositories & service ----
            IStudentRepository studentRepository = new StudentInMemoryRepository();
            IEquipmentRepository equipmentRepository = new EquipmentInMemoryRepository();
            IBorrowRepository borrowRepository = new BorrowInMemoryRepository();
            IBorrowService borrowService = new BorrowService(borrowRepository);

            // ---- Seed data ----
            var student = new Student(
                studentID: "2022303102",
                fullName: "Brill Jarn T. Barazan",
                college: "College of Information Science in Computing",
                course: "BS in Information Technology",
                yearLevel: "3rd Year",
                contactNumber: "09171234567",
                emailAddress: "s.barazan.brilljarn@cmu.edu.ph",
                address: "Valencia City, Bukidnon");
            studentRepository.Add(student);

            var camera = new Equipment(Guid.NewGuid(), "DSLR Camera", "Photography");
            var projector = new Equipment(Guid.NewGuid(), "Projector", "AV Equipment");
            var laptop = new Equipment(Guid.NewGuid(), "Laptop", "Computer");
            var speaker = new Equipment(Guid.NewGuid(), "Speaker", "AV Equipment");
            equipmentRepository.Add(camera);
            equipmentRepository.Add(projector);
            equipmentRepository.Add(laptop);
            equipmentRepository.Add(speaker);

            // ============================================================
            // SUCCESS CASE: Student requests AVAILABLE equipment
            // ============================================================
            Console.WriteLine("--- SUCCESS CASE ---");
            TryBorrow(borrowService, studentRepository, equipmentRepository,
                student.StudentID, camera.Id, DateTime.Now.AddDays(3));

            // ============================================================
            // FAILURE CASE: Equipment does not exist
            // ============================================================
            Console.WriteLine("\n--- FAILURE CASE: Equipment does not exist ---");
            TryBorrow(borrowService, studentRepository, equipmentRepository,
                student.StudentID, Guid.NewGuid(), DateTime.Now.AddDays(3));

            // ============================================================
            // FAILURE CASE: Equipment is unavailable (already borrowed above)
            // ============================================================
            Console.WriteLine("\n--- FAILURE CASE: Equipment unavailable ---");
            TryBorrow(borrowService, studentRepository, equipmentRepository,
                student.StudentID, camera.Id, DateTime.Now.AddDays(3));

            // ============================================================
            // FAILURE CASE: Student not allowed to borrow (limit reached)
            // ============================================================
            Console.WriteLine("\n--- FAILURE CASE: Student borrow limit reached ---");
            TryBorrow(borrowService, studentRepository, equipmentRepository,
                student.StudentID, projector.Id, DateTime.Now.AddDays(3)); // 2nd active borrow
            TryBorrow(borrowService, studentRepository, equipmentRepository,
                student.StudentID, laptop.Id, DateTime.Now.AddDays(3));    // 3rd active borrow (limit reached)
            TryBorrow(borrowService, studentRepository, equipmentRepository,
                student.StudentID, speaker.Id, DateTime.Now.AddDays(3));   // should fail: limit exceeded

            Console.WriteLine("\n=== Demo complete ===");
        }

        // Simulates a single borrow request: fetches records via repositories,
        // then delegates the business rules to the application service.
        private static void TryBorrow(
            IBorrowService borrowService,
            IStudentRepository studentRepository,
            IEquipmentRepository equipmentRepository,
            string studentId,
            Guid equipmentId,
            DateTime dueDate)
        {
            try
            {
                var student = studentRepository.GetById(studentId);
                if (student == null)
                {
                    Console.WriteLine($"[FAILED] Student '{studentId}' was not found.");
                    return;
                }

                var equipment = equipmentRepository.GetById(equipmentId);
                if (equipment == null)
                {
                    Console.WriteLine($"[FAILED] Equipment '{equipmentId}' does not exist.");
                    return;
                }

                var borrow = borrowService.BorrowEquipment(student, equipment, dueDate);

                Console.WriteLine($"[SUCCESS] {student.FullName} borrowed '{equipment.EquipmentName}'.");
                Console.WriteLine($"          Borrow ID: {borrow.Id}");
                Console.WriteLine($"          Due: {borrow.DueDate:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FAILED] {ex.Message}");
            }
        }
    }
}