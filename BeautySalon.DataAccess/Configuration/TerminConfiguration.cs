using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; // Potrebno za EnumToStringConverter

namespace BeautySalon.DataAccess.Configurations
{
    public class TerminConfiguration : IEntityTypeConfiguration<Termin>
    {
        public void Configure(EntityTypeBuilder<Termin> builder)
        {
            builder.ToTable("TERMIN");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("termin_id");
            builder.Property(e => e.Datum).HasColumnName("datum").IsRequired().HasColumnType("date");
            builder.Property(e => e.Vrijeme).HasColumnName("vrijeme").IsRequired().HasColumnType("time");
            builder.Property(e => e.TrajanjeMinuta).HasColumnName("trajanje").IsRequired();

            var terminStatusConverter = new EnumToStringConverter<TerminStatus>();
            builder.Property(e => e.Status)
                  .HasColumnName("status")
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasConversion(terminStatusConverter);

            builder.HasOne(d => d.Klijent)
                  .WithMany()
                  .HasForeignKey(d => d.KlijentId)
                  .OnDelete(DeleteBehavior.Restrict);
            builder.Property(e => e.KlijentId).HasColumnName("korisnik_id").IsRequired();

            builder.HasOne(d => d.Zaposlenik)
                  .WithMany()
                  .HasForeignKey(d => d.ZaposlenikId)
                  .OnDelete(DeleteBehavior.Restrict);
            builder.Property(e => e.ZaposlenikId).HasColumnName("zaposlenik_id").IsRequired();

            builder.HasMany(t => t.StavkeTermina)
                  .WithOne()
                  .HasForeignKey(st => st.TerminId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}