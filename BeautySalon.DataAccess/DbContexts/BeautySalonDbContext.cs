using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySalon.DataAccess.Configurations;

namespace BeautySalon.DataAccess.DbContexts
{
    public class SalonDbContext : DbContext
    {
        public SalonDbContext(DbContextOptions<SalonDbContext> options) : base(options) { }

        public DbSet<Usluga> Usluge { get; set; }
        public DbSet<Materijal> Materijal { get; set; }
        public DbSet<VrstaMaterijala> VrstaMaterijala { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UslugaConfiguration());
            modelBuilder.ApplyConfiguration(new MaterijalConfiguration());
            modelBuilder.ApplyConfiguration(new VrstaMaterijalaConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
