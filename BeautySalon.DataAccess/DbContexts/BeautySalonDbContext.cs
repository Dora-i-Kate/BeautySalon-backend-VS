using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracija entiteta Korisnik
            modelBuilder.Entity<Korisnik>(entity =>
            {
                entity.ToTable("KORISNIK"); // Mapira entitet Korisnik na tablicu KORISNIK
                entity.HasKey(e => e.Id); // Primarni ključ
                entity.Property(e => e.Id).HasColumnName("korisnik_id");
                entity.Property(e => e.Ime).HasColumnName("ime").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Prezime).HasColumnName("prezime").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique(); // Email mora biti jedinstven
                entity.Property(e => e.LozinkaHash).HasColumnName("lozinka").IsRequired().HasMaxLength(255); // Lozinka u bazi je hash
                entity.Property(e => e.Telefon).HasColumnName("telefon").HasMaxLength(50);
                entity.Property(e => e.DatumRegistracije).HasColumnName("datum_registracije").IsRequired();
                entity.Property(e => e.VrijemeZadnjePrijave).HasColumnName("vrijeme_zadnje_prijave");

                // Veza s Ulogom (jedan-prema-mnogi)
                entity.HasOne(d => d.Uloga)
                      .WithMany() // Uloga nema kolekciju Korisnika, ali je moguće dodati ako je potrebno
                      .HasForeignKey(d => d.UlogaId)
                      .OnDelete(DeleteBehavior.Restrict); // Ne dozvoli brisanje uloge ako postoje korisnici
                entity.Property(e => e.UlogaId).HasColumnName("uloga_id").IsRequired();
            });

            // Konfiguracija entiteta Uloga
            modelBuilder.Entity<Uloga>(entity =>
            {
                entity.ToTable("ULOGA"); // Mapira entitet Uloga na tablicu ULOGA
                entity.HasKey(e => e.Id); // Primarni ključ
                entity.Property(e => e.Id).HasColumnName("uloga_id");
                entity.Property(e => e.NazivUloge).HasColumnName("naziv_uloge").IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.NazivUloge).IsUnique(); // Naziv uloge mora biti jedinstven
            });

            // Konfiguracija entiteta Termin
            modelBuilder.Entity<Termin>(entity =>
            {
                entity.ToTable("TERMIN"); // Mapira entitet Termin na tablicu TERMIN
                entity.HasKey(e => e.Id); // Primarni ključ
                entity.Property(e => e.Id).HasColumnName("termin_id");
                entity.Property(e => e.Datum).HasColumnName("datum").IsRequired().HasColumnType("date"); // Samo datum
                entity.Property(e => e.Vrijeme).HasColumnName("vrijeme").IsRequired().HasColumnType("time"); // Samo vrijeme
                entity.Property(e => e.TrajanjeMinuta).HasColumnName("trajanje").IsRequired();

                // Konverter za TerminStatus enum u string u bazi podataka
                var terminStatusConverter = new EnumToStringConverter<TerminStatus>();
                entity.Property(e => e.Status)
                      .HasColumnName("status")
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasConversion(terminStatusConverter);

                // Veza s Klijentom (Korisnik)
                entity.HasOne(d => d.Klijent)
                      .WithMany() // Klijent nema kolekciju termina, ali je moguće dodati
                      .HasForeignKey(d => d.KlijentId)
                      .OnDelete(DeleteBehavior.Restrict); // Ne dozvoli brisanje klijenta ako ima termine
                entity.Property(e => e.KlijentId).HasColumnName("korisnik_id").IsRequired(); // strani ključ za klijenta

                // Veza sa Zaposlenikom (Korisnik)
                entity.HasOne(d => d.Zaposlenik)
                      .WithMany() // Zaposlenik nema kolekciju termina, ali je moguće dodati
                      .HasForeignKey(d => d.ZaposlenikId)
                      .OnDelete(DeleteBehavior.Restrict); // Ne dozvoli brisanje zaposlenika ako ima termine
                entity.Property(e => e.ZaposlenikId).HasColumnName("zaposlenik_id").IsRequired(); // strani ključ za zaposlenika

                // Konfiguracija za kolekciju StavkeTermina
                entity.HasMany(t => t.StavkeTermina)
                      .WithOne() // StavkaTermina ima jedan Termin
                      .HasForeignKey(st => st.TerminId)
                      .OnDelete(DeleteBehavior.Cascade); // Brisanje termina briše i njegove stavke
            });

            // Konfiguracija entiteta StavkaTermina
            modelBuilder.Entity<StavkaTermina>(entity =>
            {
                entity.ToTable("STAVKA_TERMINA"); // Mapira entitet StavkaTermina na tablicu STAVKA_TERMINA
                entity.HasKey(e => e.Id); // Primarni ključ
                entity.Property(e => e.Id).HasColumnName("stavka_id");
                entity.Property(e => e.Kolicina).HasColumnName("kolicina").IsRequired();
                entity.Property(e => e.Cijena).HasColumnName("cijena").IsRequired().HasColumnType("decimal(18,2)");

                // Veza s Uslugom
                entity.HasOne(d => d.Usluga)
                      .WithMany() // Usluga nema kolekciju StavkiTermina, ali je moguće dodati
                      .HasForeignKey(d => d.UslugaId)
                      .OnDelete(DeleteBehavior.Restrict); // Ne dozvoli brisanje usluge ako je vezana za stavku termina
                entity.Property(e => e.UslugaId).HasColumnName("usluga_id").IsRequired(); // strani ključ za uslugu

                entity.Property(e => e.TerminId).HasColumnName("termin_id").IsRequired(); // strani ključ za termin
            });

            // Konfiguracija entiteta Usluga
            modelBuilder.Entity<Usluga>(entity =>
            {
                entity.ToTable("USLUGA"); // Mapira entitet Usluga na tablicu USLUGA
                entity.HasKey(e => e.Id); // Primarni ključ
                entity.Property(e => e.Id).HasColumnName("usluga_id");
                entity.Property(e => e.Naziv).HasColumnName("naziv").IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Naziv).IsUnique(); // Naziv usluge mora biti jedinstven
                entity.Property(e => e.Opis).HasColumnName("opis").HasMaxLength(1000);
                entity.Property(e => e.Cijena).HasColumnName("cijena").IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.TrajanjeMinuta).HasColumnName("trajanje").IsRequired();
            });

            // Dodatna konfiguracija za inicijalne podatke (seed data)
            modelBuilder.Entity<Uloga>().HasData(
                new Uloga("Klijent") { Id = (int)UlogaNaziv.Klijent },
                new Uloga("Zaposlenik") { Id = (int)UlogaNaziv.Zaposlenik },
                new Uloga("Administrator") { Id = (int)UlogaNaziv.Administrator }
            );
        }
    }
}
