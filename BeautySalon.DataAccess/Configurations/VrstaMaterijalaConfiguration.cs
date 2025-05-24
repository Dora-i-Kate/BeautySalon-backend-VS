using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.DataAccess.Configurations
{
    public class VrstaMaterijalaConfiguration : IEntityTypeConfiguration<VrstaMaterijala>
    {
        public void Configure(EntityTypeBuilder<VrstaMaterijala> builder)
        {
            builder.ToTable("vrstamaterijala");

            builder.HasKey(v => v.VrstaId);

            builder.Property(v => v.VrstaId).HasColumnName("vrsta_id");

            builder.Property(v => v.Naziv)
                .HasColumnName("naziv")
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
