using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence
{
    public class BorrowConfiguration : IEntityTypeConfiguration<Borrow>
    {
        public void Configure(EntityTypeBuilder<Borrow> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedNever();

            builder.Property(b => b.BorrowDate)
                .IsRequired();

            builder.Property(b => b.DueDate)
                .IsRequired();

            builder.Property(b => b.ReturnDate);

            builder.Property(b => b.Status)
                .HasConversion<int>()
                .HasDefaultValue(BorrowStatusEnum.Active);

            // Foreign key to Student via StudentID
            builder.HasOne(b => b.StudentBorrower)
                .WithMany()
                .HasForeignKey(b => b.StudentBorrower.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            // Foreign key to Equipment
            builder.HasOne(b => b.EquipmentBorrowed)
                .WithMany()
                .HasForeignKey(b => b.EquipmentBorrowed.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Borrows");
        }
    }
}
