using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeautySalon.DataAccess.Configurations
{
    public class VrstaMaterijalaConfiguration : IEntityTypeConfiguration<VrstaMaterijala>
    {
        public void Configure(EntityTypeBuilder<VrstaMaterijala> builder)
        {
            builder.ToTable("VRSTA_MATERIJALA");
            builder.HasKey(v => v.VrstaId);
            builder.Property(v => v.VrstaId).HasColumnName("vrsta_id");

            builder.Property(v => v.Naziv)
                .HasColumnName("naziv")
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(e => e.Naziv).IsUnique();
        }
    }
}