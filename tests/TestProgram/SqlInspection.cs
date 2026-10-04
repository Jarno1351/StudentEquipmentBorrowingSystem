using System;
using System.IO;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ConsoleDemo
{
    /// <summary>
    /// Prints the SQL that EF Core generates for the three LINQ queries used by the app.
    /// Run with:  dotnet run --project tests/TestProgram -- --sql
    ///
    /// It uses a throw-away SQLite database in the temp folder, so the real
    /// EquipmentBorrowing.db is never touched.
    /// </summary>
    public static class SqlInspection
    {
        public static async Task RunAsync()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"sql-inspection-{Guid.NewGuid():N}.db");
            var connectionString = $"Data Source={dbPath}";

            // SQL is only printed while this flag is true, so migrations and seeding stay quiet.
            var logging = false;

            var options = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>()
                .UseSqlite(connectionString)
                .EnableSensitiveDataLogging()   // shows parameter values, e.g. @studentID = '...'
                .LogTo(
                    message =>
                    {
                        if (logging)
                        {
                            Console.WriteLine(message);
                            Console.WriteLine();
                        }
                    },
                    new[] { RelationalEventId.CommandExecuted },
                    LogLevel.Information,
                    DbContextLoggerOptions.None)
                .Options;

            try
            {
                using var context = new EquipmentBorrowingDbContext(options);
                await context.Database.MigrateAsync();

                var students = new EfStudentRepository(context);
                var equipment = new EfEquipmentRepository(context);
                var borrows = new EfBorrowRepository(context);

                // Demo data (Travis 2024303110 has 3 active borrows).
                await DemoDataSeeder.SeedAsync(students, equipment, borrows);

                logging = true;

                Console.WriteLine("=== Query 1 - Available equipment (EfEquipmentRepository.GetAvailableAsync) ===\n");
                var available = await equipment.GetAvailableAsync();
                Console.WriteLine($"-> {System.Linq.Enumerable.Count(available)} row(s)\n");

                Console.WriteLine("=== Query 2 - Active borrowings (EfBorrowRepository.GetActiveAsync) ===\n");
                var active = await borrows.GetActiveAsync();
                Console.WriteLine($"-> {System.Linq.Enumerable.Count(active)} row(s)\n");

                Console.WriteLine("=== Query 3 - Active borrow count for a student (EfBorrowRepository.CountActiveByStudentAsync) ===\n");
                var count = await borrows.CountActiveByStudentAsync("2024303110");
                Console.WriteLine($"-> {count}\n");

                logging = false;
            }
            finally
            {
                // Release the SQLite file handle, then remove the temp database.
                Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                try { if (File.Exists(dbPath)) File.Delete(dbPath); } catch { /* temp file, ignore */ }
            }
        }
    }
}