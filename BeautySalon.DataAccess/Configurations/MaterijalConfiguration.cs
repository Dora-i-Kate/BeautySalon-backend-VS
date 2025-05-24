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
    public class MaterijalConfiguration : IEntityTypeConfiguration<Materijal>
    {
        public void Configure(EntityTypeBuilder<Materijal> builder)
        {
            builder.ToTable("materijal");

            builder.HasKey(m => m.MaterijalId);
            builder.Property(m => m.Naziv).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Cijena).HasColumnType("numeric(10,2)");
            builder.Property(m => m.JedinicaMjere).HasMaxLength(20);
            // Prilaogodba za tablice u bazi čija su imena pisana malim slovom
            builder.Property(m => m.MaterijalId).HasColumnName("materijal_id");
            builder.Property(m => m.Naziv).HasColumnName("naziv");
            builder.Property(m => m.Cijena).HasColumnName("cijena");
            builder.Property(m => m.MinimalnaKolicina).HasColumnName("minimalna_kolicina");
            builder.Property(m => m.TrenutnaKolicina).HasColumnName("trenutna_kolicina");
            builder.Property(m => m.JedinicaMjere).HasColumnName("jedinica_mjere");
            builder.Property(m => m.VrstaId).HasColumnName("vrsta_id");

            builder.HasOne(m => m.VrstaMaterijala)
               .WithMany()           
               .HasForeignKey(m => m.VrstaId)
               .HasConstraintName("FK_Materijal_VrstaMaterijala")
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
