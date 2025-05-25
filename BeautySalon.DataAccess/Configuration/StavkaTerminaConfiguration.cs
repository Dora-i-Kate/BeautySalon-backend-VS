using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeautySalon.DataAccess.Configurations
{
    public class StavkaTerminaConfiguration : IEntityTypeConfiguration<StavkaTermina>
    {
        public void Configure(EntityTypeBuilder<StavkaTermina> builder)
        {
            builder.ToTable("STAVKA_TERMINA");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("stavka_id");
            builder.Property(e => e.Kolicina).HasColumnName("kolicina").IsRequired();
            builder.Property(e => e.Cijena).HasColumnName("cijena").IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne(d => d.Usluga)
                  .WithMany()
                  .HasForeignKey(d => d.UslugaId)
                  .OnDelete(DeleteBehavior.Restrict);
            builder.Property(e => e.UslugaId).HasColumnName("usluga_id").IsRequired();

            builder.Property(e => e.TerminId).HasColumnName("termin_id").IsRequired();
        }
    }
}