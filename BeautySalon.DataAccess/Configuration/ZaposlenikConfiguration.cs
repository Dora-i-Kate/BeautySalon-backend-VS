using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.DataAccess.Configuration
{
    public class ZaposlenikConfiguration : IEntityTypeConfiguration<Zaposlenik>
    {
        public void Configure(EntityTypeBuilder<Zaposlenik> builder)
        {
            builder.HasBaseType<Korisnik>();
            builder.Property(z => z.Specijalizacija).IsRequired().HasMaxLength(100);
            builder.Property(z => z.Certifikat).IsRequired().HasMaxLength(100);
            builder.Property(z => z.DatumZaposlenja).IsRequired();
        }
    }
}
