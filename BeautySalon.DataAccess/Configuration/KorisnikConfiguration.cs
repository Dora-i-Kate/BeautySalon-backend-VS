using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeautySalon.DataAccess.Configurations
{
    public class KorisnikConfiguration : IEntityTypeConfiguration<Korisnik>
    {
        public void Configure(EntityTypeBuilder<Korisnik> builder)
        {
            builder.ToTable("KORISNIK");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("korisnik_id");
            builder.Property(e => e.Ime).HasColumnName("ime").IsRequired().HasMaxLength(100);
            builder.Property(e => e.Prezime).HasColumnName("prezime").IsRequired().HasMaxLength(100);
            builder.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.LozinkaHash).HasColumnName("lozinka").IsRequired().HasMaxLength(255);
            builder.Property(e => e.Telefon).HasColumnName("telefon").HasMaxLength(50);
            builder.Property(e => e.DatumRegistracije).HasColumnName("datum_registracije").IsRequired();
            builder.Property(e => e.VrijemeZadnjePrijave).HasColumnName("vrijeme_zadnje_prijave");

            builder.HasOne(d => d.Uloga)
                  .WithMany()
                  .HasForeignKey(d => d.UlogaId)
                  .OnDelete(DeleteBehavior.Restrict);
            builder.Property(e => e.UlogaId).HasColumnName("uloga_id").IsRequired();
        }
    }
}