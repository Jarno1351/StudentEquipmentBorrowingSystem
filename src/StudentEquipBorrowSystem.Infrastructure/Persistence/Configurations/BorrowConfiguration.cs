using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Configures the Borrow entity mapping to the Borrows table.
    /// Handles relationships to Student and Equipment, with enum conversion for Status.
    /// </summary>
    public class BorrowConfiguration : IEntityTypeConfiguration<Borrow>
    {
        public void Configure(EntityTypeBuilder<Borrow> builder)
        {
            // Table mapping
            builder.ToTable("Borrows");

            // Primary Key
            builder.HasKey(b => b.Id)
                .HasName("PK_Borrows_Id");

            // Properties with constraints
            builder.Property(b => b.Id)
                .HasColumnType("TEXT")
                .ValueGeneratedNever()
                .IsRequired()
                .HasColumnName("Id");

            builder.Property(b => b.BorrowDate)
                .HasColumnType("TEXT")
                .IsRequired()
                .HasColumnName("BorrowDate");

            builder.Property(b => b.DueDate)
                .HasColumnType("TEXT")
                .IsRequired()
                .HasColumnName("DueDate");

            builder.Property(b => b.ReturnDate)
                .HasColumnType("TEXT")
                .IsRequired(false)
                .HasColumnName("ReturnDate");

            // Enum conversion: BorrowStatusEnum stored as integer (0=Active, 1=Returned, 2=Overdue)
            builder.Property(b => b.Status)
                .HasColumnType("INTEGER")
                .HasConversion<int>()
                .HasDefaultValue(BorrowStatusEnum.Active)
                .IsRequired()
                .HasColumnName("Status");

            // Relationships and Foreign Keys
            builder.HasOne(b => b.StudentBorrower)
                .WithMany()
                .HasForeignKey("StudentBorrower_StudentID")
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Borrows_Students_StudentID");

            builder.HasOne(b => b.EquipmentBorrowed)
                .WithMany()
                .HasForeignKey("EquipmentBorrowed_Id")
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Borrows_Equipment_Id");

            // Indexes for common query patterns
            builder.HasIndex("StudentBorrower_StudentID")
                .HasDatabaseName("IX_Borrows_StudentID");

            builder.HasIndex("EquipmentBorrowed_Id")
                .HasDatabaseName("IX_Borrows_EquipmentId");

            builder.HasIndex(b => b.Status)
                .HasDatabaseName("IX_Borrows_Status");

            builder.HasIndex(b => b.DueDate)
                .HasDatabaseName("IX_Borrows_DueDate");

            // Composite index for finding active borrowings for a student
            // Note: Composite index on foreign key + status for common query pattern
            builder.HasIndex(b => new { StudentBorrowerId = b.StudentBorrower.StudentID, b.Status })
                .HasDatabaseName("IX_Borrows_StudentID_Status");
        }
    }
}
