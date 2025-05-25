using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeautySalon.DataAccess.Configurations
{
    public class MaterijalConfiguration : IEntityTypeConfiguration<Materijal>
    {
        public void Configure(EntityTypeBuilder<Materijal> builder)
        {
            builder.ToTable("MATERIJAL");
            builder.HasKey(m => m.MaterijalId);
            builder.Property(m => m.MaterijalId).HasColumnName("materijal_id");

            builder.Property(m => m.Naziv).HasColumnName("naziv").IsRequired().HasMaxLength(255);
            builder.Property(m => m.Cijena).HasColumnName("cijena").IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(m => m.JedinicaMjere).HasColumnName("jedinica_mjere").IsRequired().HasMaxLength(50);

            builder.Property(m => m.MinimalnaKolicina).HasColumnName("minimalna_kolicina").IsRequired();
            builder.Property(m => m.TrenutnaKolicina).HasColumnName("trenutna_kolicina").IsRequired();

            builder.HasIndex(e => e.Naziv).IsUnique();

            builder.Property(m => m.VrstaId).HasColumnName("vrsta_id").IsRequired();

            builder.HasOne(m => m.VrstaMaterijala)
               .WithMany()
               .HasForeignKey(m => m.VrstaId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}