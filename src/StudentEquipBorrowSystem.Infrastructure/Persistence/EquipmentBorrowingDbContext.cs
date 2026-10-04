using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class EquipmentBorrowingDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Borrow> Borrows { get; set; }

        public EquipmentBorrowingDbContext(DbContextOptions<EquipmentBorrowingDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new EquipmentConfiguration());
            modelBuilder.ApplyConfiguration(new BorrowConfiguration());
        }
    }
}
