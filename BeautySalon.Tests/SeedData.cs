using BeautySalon.Domain.Models; // KLJUČNO: Osigurava pristup Uloga, Korisnik, Termin, TerminStatus, UlogaNaziv
using BeautySalon.DataAccess;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BeautySalon.Tests
{
    public static class SeedData
    {
        public static void Initialize(BeautySalonDbContext context)
        {
            // Oprezno s RemoveRange ako imate vanjske ključeve i ograničenja
            // Najbolje je resetirati bazu putem EnsureDeleted/EnsureCreated u CustomWebApplicationFactory
            // ali ostavit ćemo ovo za sada ako je bila namjera.
            context.Termini.RemoveRange(context.Termini);
            context.StavkeTermina.RemoveRange(context.StavkeTermina);
            context.Usluge.RemoveRange(context.Usluge);
            context.Korisnici.RemoveRange(context.Korisnici);
            context.Uloge.RemoveRange(context.Uloge); // Obriši uloge
            context.SaveChanges();

            // Seed Uloge - koristiti javni konstruktor s parametrom
            if (!context.Uloge.Any())
            {
                context.Uloge.AddRange(
                    new Uloga("Klijent"), // Koristi konstruktor: public Uloga(string nazivUloge)
                    new Uloga("Zaposlenik"),
                    new Uloga("Administrator")
                );
                context.SaveChanges();
            }

            // Seed Usluge - koristiti javni konstruktor s parametrima
            if (!context.Usluge.Any())
            {
                context.Usluge.AddRange(
                    new Usluga("Šišanje", "Muško šišanje", 50.00m, 30), // Koristi konstruktor: public Usluga(string naziv, string opis, decimal cijena, int trajanjeMinuta)
                    new Usluga("Boja", "Farbanje kose", 150.00m, 90),
                    new Usluga("Manikura", "Klasična manikura", 80.00m, 60)
                );
                context.SaveChanges();
            }

            // Seed Korisnici - koristiti javni konstruktor s parametrima
            if (!context.Korisnici.Any())
            {
                // Dohvaćamo ID-eve uloga na temelju njihovih naziva
                var klijentUlogaId = context.Uloge.First(u => u.NazivUloge == UlogaNaziv.Klijent.ToString()).Id;
                var zaposlenikUlogaId = context.Uloge.First(u => u.NazivUloge == UlogaNaziv.Zaposlenik.ToString()).Id;
                var administratorUlogaId = context.Uloge.First(u => u.NazivUloge == UlogaNaziv.Administrator.ToString()).Id;

                context.Korisnici.AddRange(
                    new Korisnik("Ivan", "Horvat", "ivan.h@example.com", "hash_lozinke_1", "091123456", klijentUlogaId),
                    new Korisnik("Ana", "Marić", "ana.m@example.com", "hash_lozinke_2", "092123456", zaposlenikUlogaId),
                    new Korisnik("Marko", "Petrović", "marko.p@example.com", "hash_lozinke_3", "091987654", klijentUlogaId),
                    new Korisnik("Jelena", "Kovač", "jelena.k@example.com", "hash_lozinke_4", "095543210", zaposlenikUlogaId),
                    new Korisnik("Admin", "Admin", "admin@example.com", "hash_lozinke_admin", "099111222", administratorUlogaId)
                );
                context.SaveChanges();
            }

            // Seed Termini
            if (!context.Termini.Any())
            {
                var klijentIvan = context.Korisnici.First(k => k.Email == "ivan.h@example.com");
                var zaposlenikAna = context.Korisnici.First(k => k.Email == "ana.m@example.com");
                var klijentMarko = context.Korisnici.First(k => k.Email == "marko.p@example.com");
                var zaposlenikJelena = context.Korisnici.First(k => k.Email == "jelena.k@example.com");

                var sisanjeUsluga = context.Usluge.First(u => u.Naziv == "Šišanje");
                var bojaUsluga = context.Usluge.First(u => u.Naziv == "Boja");
                var manikuraUsluga = context.Usluge.First(u => u.Naziv == "Manikura");

                // Prvi termin
                var termin1 = new Termin(DateTime.Today.AddDays(1), new TimeSpan(10, 0, 0), sisanjeUsluga.TrajanjeMinuta, klijentIvan.Id, zaposlenikAna.Id);
                // Dodaj stavke termina koristeći AddStavka metodu
                termin1.AddStavka(sisanjeUsluga.Id, 1, sisanjeUsluga.Cijena);
                context.Termini.Add(termin1);
                context.SaveChanges(); // Potrebno je prvo spremiti termin da bi se dobio Id za stavke

                // Drugi termin
                var termin2 = new Termin(DateTime.Today.AddDays(2), new TimeSpan(14, 30, 0), bojaUsluga.TrajanjeMinuta, klijentMarko.Id, zaposlenikJelena.Id);
                termin2.AddStavka(bojaUsluga.Id, 1, bojaUsluga.Cijena);
                context.Termini.Add(termin2);
                context.SaveChanges();
            }
        }
    }
}