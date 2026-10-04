using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Provides a DbContext instance at design-time for EF Core tools to use
    /// when creating and applying migrations.
    /// </summary>
    public class EquipmentBorrowingDbContextFactory : IDesignTimeDbContextFactory<EquipmentBorrowingDbContext>
    {
        public EquipmentBorrowingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>();

            // Use SQLite with a database file in the application directory
            // This path will be relative to the current working directory when running migrations
            var connectionString = "Data Source=EquipmentBorrowing.db";

            optionsBuilder.UseSqlite(connectionString);

            return new EquipmentBorrowingDbContext(optionsBuilder.Options);
        }
    }
}
