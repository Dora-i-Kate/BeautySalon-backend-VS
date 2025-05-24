using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeautySalon.DataAccess.Configurations
{
    public class TerminConfiguration : IEntityTypeConfiguration<Termin>
    {
        public void Configure(EntityTypeBuilder<Termin> builder)
        {
            builder.HasKey(t => t.TerminId);
            builder.Property(t => t.Status).IsRequired().HasMaxLength(50);

            builder.HasOne(t => t.Klijent)
                   .WithMany()
                   .HasForeignKey(t => t.KlijentId);

            builder.HasOne(t => t.Zaposlenik)
                   .WithMany()
                   .HasForeignKey(t => t.ZaposlenikId);
        }
    }
}
