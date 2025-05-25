// BeautySalon.DataAccess.BeautySalonDbContext.cs
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BeautySalon.DataAccess.Configurations;

namespace BeautySalon.DataAccess
{
    /// <summary>
    /// DbContext za Entity Framework Core, predstavlja sesiju s bazom podataka.
    /// Konfigurira mapiranje domenskih entiteta u tablice baze podataka.
    /// </summary>
    public class BeautySalonDbContext : DbContext
    {
        public BeautySalonDbContext(DbContextOptions<BeautySalonDbContext> options) : base(options)
        {
        }

        // DbSet svojstva za svaki entitet koji želimo mapirati u tablicu
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Uloga> Uloge { get; set; }
        public DbSet<Termin> Termini { get; set; }
        public DbSet<StavkaTermina> StavkeTermina { get; set; }
        public DbSet<Usluga> Usluge { get; set; }
        public DbSet<Materijal> Materijali { get; set; }
        public DbSet<VrstaMaterijala> VrsteMaterijala { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new KorisnikConfiguration());
            modelBuilder.ApplyConfiguration(new UlogaConfiguration());
            modelBuilder.ApplyConfiguration(new TerminConfiguration());
            modelBuilder.ApplyConfiguration(new StavkaTerminaConfiguration());
            modelBuilder.ApplyConfiguration(new UslugaConfiguration());
            modelBuilder.ApplyConfiguration(new MaterijalConfiguration());
            modelBuilder.ApplyConfiguration(new VrstaMaterijalaConfiguration());

            modelBuilder.Entity<Uloga>().HasData(
                new Uloga("Klijent") { Id = (int)UlogaNaziv.Klijent },
                new Uloga("Zaposlenik") { Id = (int)UlogaNaziv.Zaposlenik },
                new Uloga("Administrator") { Id = (int)UlogaNaziv.Administrator }
            );

            // Ako želiš dodati seed data za VrsteMaterijala, možeš ovdje.
        }
    }
}
