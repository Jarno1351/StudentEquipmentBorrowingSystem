using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence
{
    public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.EquipmentName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.EquipmentType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.IsAvailable)
                .HasDefaultValue(true);

            builder.ToTable("Equipment");
        }
    }
}
