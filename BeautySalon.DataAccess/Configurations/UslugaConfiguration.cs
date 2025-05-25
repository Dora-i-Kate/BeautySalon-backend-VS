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
            builder.HasKey(u => u.UslugaId);
            builder.Property(u => u.Naziv).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Cijena).HasColumnType("numeric(10, 2)");
        }
    }
}
