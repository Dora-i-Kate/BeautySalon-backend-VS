// BeautySalon.DataAccess.BeautySalonDbContext.cs
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BeautySalon.DataAccess.Configurations; // OVU LINIJU JE KLJUČNO DODATI!

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
        public DbSet<Materijal> Materijali { get; set; } // NOVO: Dodano za šifrarnik Materijala
        public DbSet<VrstaMaterijala> VrsteMaterijala { get; set; } // NOVO: Dodano za šifrarnik VrsteMaterijala

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- KORIŠTENJE IZOLIRANIH KONFIGURACIJA ZA SVE ENTITETE ---
            // Umjesto da ovdje pišemo Fluent API konfiguraciju za svaki entitet,
            // koristimo ApplyConfiguration koji učitava konfiguraciju iz zasebnih klasa.
            modelBuilder.ApplyConfiguration(new KorisnikConfiguration());
            modelBuilder.ApplyConfiguration(new UlogaConfiguration());
            modelBuilder.ApplyConfiguration(new TerminConfiguration());
            modelBuilder.ApplyConfiguration(new StavkaTerminaConfiguration());
            modelBuilder.ApplyConfiguration(new UslugaConfiguration()); // Sada se koristi izdvojena konfiguracija
            modelBuilder.ApplyConfiguration(new MaterijalConfiguration()); // Dodano za Materijal
            modelBuilder.ApplyConfiguration(new VrstaMaterijalaConfiguration()); // Dodano za VrstuMaterijala

            // Dodatna konfiguracija za inicijalne podatke (seed data)
            modelBuilder.Entity<Uloga>().HasData(
                new Uloga("Klijent") { Id = (int)UlogaNaziv.Klijent },
                new Uloga("Zaposlenik") { Id = (int)UlogaNaziv.Zaposlenik },
                new Uloga("Administrator") { Id = (int)UlogaNaziv.Administrator }
            );

            // Ovdje možete dodati seed data i za VrsteMaterijala ako želite da budu unaprijed definirane
            // modelBuilder.Entity<VrstaMaterijala>().HasData(
            //     new VrstaMaterijala("Kozmetika") { VrstaId = 1 },
            //     new VrstaMaterijala("Oprema") { VrstaId = 2 },
            //     new VrstaMaterijala("Potrošni materijal") { VrstaId = 3 }
            // );
        }
    }
}