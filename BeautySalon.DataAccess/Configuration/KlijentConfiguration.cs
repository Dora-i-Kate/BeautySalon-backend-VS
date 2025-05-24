using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeautySalon.DataAccess.Configurations
{
    public class KlijentConfiguration : IEntityTypeConfiguration<Klijent>
    {
        public void Configure(EntityTypeBuilder<Klijent> builder)
        {
            builder.HasBaseType<Korisnik>();
            builder.Property(k => k.Zahtjevi).HasMaxLength(500);
        }
    }
}
