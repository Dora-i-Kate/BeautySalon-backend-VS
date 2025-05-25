using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeautySalon.DataAccess.Configurations
{
    public class UslugaConfiguration : IEntityTypeConfiguration<Usluga>
    {
        public void Configure(EntityTypeBuilder<Usluga> builder)
        {
            builder.ToTable("USLUGA");
            builder.HasKey(u => u.Id); // Koristi Id umjesto UslugaId
            builder.Property(u => u.Id).HasColumnName("usluga_id");

            builder.Property(u => u.Naziv).HasColumnName("naziv").IsRequired().HasMaxLength(255);
            builder.HasIndex(u => u.Naziv).IsUnique();

            builder.Property(u => u.Opis).HasColumnName("opis").HasMaxLength(1000);
            builder.Property(u => u.Cijena).HasColumnName("cijena").IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(u => u.TrajanjeMinuta).HasColumnName("trajanje").IsRequired(); // Koristi TrajanjeMinuta
        }
    }
}