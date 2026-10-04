using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Configures the Student entity mapping to the Students table.
    /// </summary>
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Table mapping
            builder.ToTable("Students");

            // Primary Key
            builder.HasKey(s => s.StudentID)
                .HasName("PK_Students_StudentID");

            // Properties with constraints
            builder.Property(s => s.StudentID)
                .HasColumnType("TEXT")
                .HasMaxLength(20)
                .IsRequired()
                .HasColumnName("StudentID");

            builder.Property(s => s.FullName)
                .HasColumnType("TEXT")
                .HasMaxLength(200)
                .IsRequired()
                .HasColumnName("FullName");

            builder.Property(s => s.College)
                .HasColumnType("TEXT")
                .HasMaxLength(200)
                .IsRequired()
                .HasColumnName("College");

            builder.Property(s => s.Course)
                .HasColumnType("TEXT")
                .HasMaxLength(200)
                .IsRequired()
                .HasColumnName("Course");

            builder.Property(s => s.YearLevel)
                .HasColumnType("TEXT")
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnName("YearLevel");

            builder.Property(s => s.ContactNumber)
                .HasColumnType("TEXT")
                .HasMaxLength(20)
                .IsRequired()
                .HasColumnName("ContactNumber");

            builder.Property(s => s.EmailAddress)
                .HasColumnType("TEXT")
                .HasMaxLength(100)
                .IsRequired(false)
                .HasColumnName("EmailAddress");

            builder.Property(s => s.Address)
                .HasColumnType("TEXT")
                .HasMaxLength(500)
                .IsRequired(false)
                .HasColumnName("Address");

            // Indexes for common query patterns
            builder.HasIndex(s => s.StudentID)
                .IsUnique()
                .HasDatabaseName("IX_Students_StudentID_Unique");

            builder.HasIndex(s => s.ContactNumber)
                .HasDatabaseName("IX_Students_ContactNumber");
        }
    }
}
