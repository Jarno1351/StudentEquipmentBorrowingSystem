using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.StudentID);

            builder.Property(s => s.StudentID)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.College)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.Course)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.YearLevel)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.ContactNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.EmailAddress)
                .HasMaxLength(100);

            builder.Property(s => s.Address)
                .HasMaxLength(500);

            builder.ToTable("Students");
        }
    }
}
