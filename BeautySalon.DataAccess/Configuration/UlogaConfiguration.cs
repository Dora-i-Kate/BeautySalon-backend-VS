using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BeautySalon.DataAccess.Configurations
{
    public class UlogaConfiguration : IEntityTypeConfiguration<Uloga>
    {
        public void Configure(EntityTypeBuilder<Uloga> builder)
        {
            builder.ToTable("ULOGA");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("uloga_id");
            builder.Property(e => e.NazivUloge).HasColumnName("naziv_uloge").IsRequired().HasMaxLength(50);
            builder.HasIndex(e => e.NazivUloge).IsUnique();
        }
    }
}