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
    public class KorisnikConfiguration : IEntityTypeConfiguration<Korisnik>
    {
        public void Configure(EntityTypeBuilder<Korisnik> builder)
        {
            builder.HasKey(k => k.KorisnikId);

            builder.Property(k => k.Ime).IsRequired().HasMaxLength(100);
            builder.Property(k => k.Prezime).IsRequired().HasMaxLength(100);
            builder.Property(k => k.Email).IsRequired().HasMaxLength(100);
            builder.Property(k => k.Lozinka).IsRequired().HasMaxLength(255);
            builder.Property(k => k.BrojTelefona).HasMaxLength(20);
            builder.Property(k => k.Uloga).IsRequired();
        }
    }
}
