using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Configures the Equipment entity mapping to the Equipment table.
    /// </summary>
    public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            // Table mapping
            builder.ToTable("Equipment");

            // Primary Key
            builder.HasKey(e => e.Id)
                .HasName("PK_Equipment_Id");

            // Properties with constraints
            builder.Property(e => e.Id)
                .HasColumnType("TEXT")
                .ValueGeneratedNever()
                .IsRequired()
                .HasColumnName("Id");

            builder.Property(e => e.EquipmentName)
                .HasColumnType("TEXT")
                .HasMaxLength(200)
                .IsRequired()
                .HasColumnName("EquipmentName");

            builder.Property(e => e.EquipmentType)
                .HasColumnType("TEXT")
                .HasMaxLength(100)
                .IsRequired()
                .HasColumnName("EquipmentType");

            builder.Property(e => e.IsAvailable)
                .HasColumnType("INTEGER")
                .HasDefaultValue(true)
                .IsRequired()
                .HasColumnName("IsAvailable");

            // Indexes for common query patterns
            builder.HasIndex(e => e.IsAvailable)
                .HasDatabaseName("IX_Equipment_IsAvailable");

            builder.HasIndex(e => e.EquipmentType)
                .HasDatabaseName("IX_Equipment_EquipmentType");
        }
    }
}
