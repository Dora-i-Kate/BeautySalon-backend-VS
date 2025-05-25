using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BeautySalon.Domain.Models;

namespace BeautySalon.DataAccess.Configurations
{
    public class UslugaConfiguration : IEntityTypeConfiguration<Usluga>
    {
        public void Configure(EntityTypeBuilder<Usluga> builder)
        {
            builder.ToTable("USLUGA");
            builder.HasKey(u => u.UslugaId);

            builder.Property(v => v.UslugaId).HasColumnName("usluga_id");
            builder.Property(v => v.Naziv).HasColumnName("naziv");
            builder.Property(v => v.Opis).HasColumnName("opis");
            builder.Property(v => v.Cijena).HasColumnName("cijena");
            builder.Property(v => v.Trajanje).HasColumnName("trajanje");
            builder.Property(u => u.Naziv).IsRequired().HasMaxLength(100);
        }
    }
}
