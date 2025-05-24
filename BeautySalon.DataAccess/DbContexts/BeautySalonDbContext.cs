using BeautySalon.DataAccess.Configuration;
using BeautySalon.DataAccess.Configurations;
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.DataAccess
{
    public class BeautySalonDbContext : DbContext
    {
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Klijent> Klijenti { get; set; }
        public DbSet<Zaposlenik> Zaposlenici { get; set; }
        public DbSet<Termin> Termini { get; set; }

        public BeautySalonDbContext(DbContextOptions<BeautySalonDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeautySalonDbContext).Assembly);

            modelBuilder.ApplyConfiguration(new KorisnikConfiguration());
            modelBuilder.ApplyConfiguration(new KlijentConfiguration());
            modelBuilder.ApplyConfiguration(new ZaposlenikConfiguration());
            modelBuilder.ApplyConfiguration(new TerminConfiguration());
        }
    }
}
