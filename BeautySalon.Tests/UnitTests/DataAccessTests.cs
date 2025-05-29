using BeautySalon.DataAccess;
using BeautySalon.DataAccess.Repositories;
using BeautySalon.Domain.Models; // Ključno za Korisnik, Uloga, Materijal, Termin, StavkaTermina, VrstaMaterijala, TerminStatus
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.Collections.Generic; // Za List<T>

namespace BeautySalon.Tests.DataAccess
{
    public class DataAccessTests : IDisposable
    {
        private readonly DbContextOptions<BeautySalonDbContext> _dbContextOptions;
        private readonly BeautySalonDbContext _context;

        public DataAccessTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BeautySalonDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Jedinstveno ime baze za svaki test
                .Options;

            _context = new BeautySalonDbContext(_dbContextOptions);
            _context.Database.EnsureCreated(); // Osigurava da se baza kreira i primjenjuju konfiguracije
        }

        // Osigurava čišćenje baze podataka nakon svakog testa
        public void Dispose()
        {
            _context.Database.EnsureDeleted(); // Briše bazu nakon svakog testa
            _context.Dispose();
        }

        // Pomoćna metoda za postavljanje privatnih/internih settera svojstava
        private T SetPrivateProperty<T, TValue>(T obj, string propertyName, TValue value)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{typeof(T).Name}'.");
            }
            property.SetValue(obj, value);
            return obj;
        }

        // Pomoćna metoda za dohvaćanje privatnih/internih svojstava
        private TValue GetPrivatePropertyValue<TValue>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new InvalidOperationException($"Property '{propertyName}' not found on type '{obj.GetType().Name}'.");
            }
            return (TValue)property.GetValue(obj);
        }

        // Pomoćna metoda za pozivanje privatnih/internih konstruktora
        private T CreateInstanceWithPrivateConstructor<T>(params object[] args)
        {
            var constructors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            ConstructorInfo constructor = null;
            foreach (var c in constructors)
            {
                var parameters = c.GetParameters();
                if (parameters.Length == args.Length)
                {
                    bool parametersMatch = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (args[i] == null)
                        {
                            if (parameters[i].ParameterType.IsValueType && Nullable.GetUnderlyingType(parameters[i].ParameterType) == null)
                            {
                                parametersMatch = false;
                                break;
                            }
                        }
                        else if (!parameters[i].ParameterType.IsInstanceOfType(args[i]) && !(parameters[i].ParameterType.IsEnum && args[i] is int))
                        {
                            parametersMatch = false;
                            break;
                        }
                    }
                    if (parametersMatch)
                    {
                        constructor = c;
                        break;
                    }
                }
            }

            if (constructor == null)
            {
                // Fallback za bezparametarski konstruktor
                if (args.Length == 0)
                {
                    constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                }
            }

            if (constructor == null)
            {
                throw new InvalidOperationException($"No matching constructor found for type {typeof(T).Name} with provided arguments.");
            }

            return (T)constructor.Invoke(args);
        }


        [Fact]
        public async Task KorisnikRepository_AddAndGetByIdAsync_ShouldReturnCorrectKorisnik()
        {
            // Arrange
            // Uloga ima public string konstruktor
            var uloga = new Uloga("Klijent");
            // Neka EF Core generira ID za Ulogu. Ne postavljamo ga ručno ako je auto-increment.
            await _context.Uloge.AddAsync(uloga);
            await _context.SaveChangesAsync(); // ID za uloga.Id će biti generiran nakon ovog poziva

            var korisnikRepository = new KorisnikRepository(_context);

            // Stvaranje Korisnik objekta koristeći CreateInstanceWithPrivateConstructor
            var noviKorisnik = CreateInstanceWithPrivateConstructor<Korisnik>();

            // Postavite sva obavezna svojstva
            SetPrivateProperty(noviKorisnik, "Ime", "Test");
            SetPrivateProperty(noviKorisnik, "Prezime", "Korisnik");
            SetPrivateProperty(noviKorisnik, "Email", "test@example.com");
            // VAŽNO: Provjerite točan naziv svojstva za lozinku u vašem Korisnik modelu (Lozinka ili LozinkaHash)
            SetPrivateProperty(noviKorisnik, "LozinkaHash", "hashedpassword");
            SetPrivateProperty(noviKorisnik, "DatumRegistracije", DateTime.Now);
            SetPrivateProperty(noviKorisnik, "Telefon", "0912345678"); // DODANO: Obavezno svojstvo
            SetPrivateProperty(noviKorisnik, "UlogaId", GetPrivatePropertyValue<int>(uloga, "Id")); // Koristite generirani ID uloge
            SetPrivateProperty(noviKorisnik, "Uloga", uloga); // Navigacijsko svojstvo

            // NE POSTAVLJAJTE ID KORISNIKA RUČNO ako je auto-increment u bazi
            // SetPrivateProperty(noviKorisnik, "Id", 1); 

            // Act
            var dodaniKorisnik = await korisnikRepository.AddAsync(noviKorisnik);
            await _context.SaveChangesAsync(); // ID za dodaniKorisnik će biti generiran nakon ovog poziva

            // Dohvati korisnika koristeći ID koji je dodijeljen od baze
            var dohvaceniKorisnik = await korisnikRepository.GetByIdAsync(GetPrivatePropertyValue<int>(dodaniKorisnik, "Id"));

            // Assert
            Assert.NotNull(dohvaceniKorisnik);
            Assert.Equal(GetPrivatePropertyValue<string>(noviKorisnik, "Ime"), GetPrivatePropertyValue<string>(dohvaceniKorisnik, "Ime"));
            Assert.Equal(GetPrivatePropertyValue<string>(noviKorisnik, "Email"), GetPrivatePropertyValue<string>(dohvaceniKorisnik, "Email"));
            Assert.Equal(GetPrivatePropertyValue<string>(noviKorisnik, "Telefon"), GetPrivatePropertyValue<string>(dohvaceniKorisnik, "Telefon"));
            Assert.NotNull(GetPrivatePropertyValue<Uloga>(dohvaceniKorisnik, "Uloga"));
            Assert.Equal(uloga.NazivUloge, GetPrivatePropertyValue<Uloga>(dohvaceniKorisnik, "Uloga").NazivUloge);
        }


        [Fact]
        public async Task TerminRepository_HasOverlappingTerminAsync_ShouldDetectOverlapCorrectly()
        {
            // Arrange
            // Uloge se mogu ručno postaviti ID-je jer su ključni entiteti za mnoge druge
            var ulogaKlijent = new Uloga("Klijent");
            SetPrivateProperty(ulogaKlijent, "Id", 10);
            var ulogaZaposlenik = new Uloga("Zaposlenik");
            SetPrivateProperty(ulogaZaposlenik, "Id", 11);
            await _context.Uloge.AddRangeAsync(ulogaKlijent, ulogaZaposlenik);
            await _context.SaveChangesAsync();

            var klijent = CreateInstanceWithPrivateConstructor<Korisnik>();
            SetPrivateProperty(klijent, "Ime", "Klijent");
            SetPrivateProperty(klijent, "Prezime", "Test");
            SetPrivateProperty(klijent, "Email", "klijent@example.com");
            SetPrivateProperty(klijent, "LozinkaHash", "hash1");
            SetPrivateProperty(klijent, "DatumRegistracije", DateTime.Now);
            SetPrivateProperty(klijent, "Telefon", "0911112222"); // DODANO: Obavezno svojstvo
            SetPrivateProperty(klijent, "UlogaId", ulogaKlijent.Id);
            SetPrivateProperty(klijent, "Id", 100);

            var zaposlenik = CreateInstanceWithPrivateConstructor<Korisnik>();
            SetPrivateProperty(zaposlenik, "Ime", "Zaposlenik");
            SetPrivateProperty(zaposlenik, "Prezime", "Test");
            SetPrivateProperty(zaposlenik, "Email", "zaposlenik@example.com");
            SetPrivateProperty(zaposlenik, "LozinkaHash", "hash2");
            SetPrivateProperty(zaposlenik, "DatumRegistracije", DateTime.Now);
            SetPrivateProperty(zaposlenik, "Telefon", "0923334444"); // DODANO: Obavezno svojstvo
            SetPrivateProperty(zaposlenik, "UlogaId", ulogaZaposlenik.Id);
            SetPrivateProperty(zaposlenik, "Id", 200);

            await _context.Korisnici.AddRangeAsync(klijent, zaposlenik);
            await _context.SaveChangesAsync();

            var terminRepository = new TerminRepository(_context);

            // Postojeći termin: 10:00 - 11:00
            var postojeciTermin = CreateInstanceWithPrivateConstructor<Termin>();
            SetPrivateProperty(postojeciTermin, "Datum", DateTime.Today);
            SetPrivateProperty(postojeciTermin, "Vrijeme", new TimeSpan(10, 0, 0));
            SetPrivateProperty(postojeciTermin, "TrajanjeMinuta", 60);
            SetPrivateProperty(postojeciTermin, "Status", TerminStatus.Zakazan);
            SetPrivateProperty(postojeciTermin, "KlijentId", GetPrivatePropertyValue<int>(klijent, "Id"));
            SetPrivateProperty(postojeciTermin, "ZaposlenikId", GetPrivatePropertyValue<int>(zaposlenik, "Id"));
            SetPrivateProperty(postojeciTermin, "Id", 1); // Ručno postavite ID za testiranje isključenja (ako je private set)
            // LINIJA ZA "StavkeTermina" JE UKLONJENA JER NEMA PUBLIC/PRIVATE SETTER
            // Prema vašoj definiciji, _stavkeTermina je privatno polje, a StavkeTermina je ReadOnlyCollection.

            await terminRepository.AddAsync(postojeciTermin);
            await _context.SaveChangesAsync(); // Ne zaboravite SaveChangesAsync nakon dodavanja termina

            // Act & Assert

            // Slučaj 1: Nema preklapanja (prije postojećeg termina)
            var nemaPreklapanjaPrije = await terminRepository.HasOverlappingTerminAsync(
                GetPrivatePropertyValue<int>(zaposlenik, "Id"), DateTime.Today, new TimeSpan(9, 0, 0), 30);
            Assert.False(nemaPreklapanjaPrije);

            // Slučaj 2: Nema preklapanja (poslije postojećeg termina)
            var nemaPreklapanjaPoslije = await terminRepository.HasOverlappingTerminAsync(
                GetPrivatePropertyValue<int>(zaposlenik, "Id"), DateTime.Today, new TimeSpan(11, 0, 1), 30);
            Assert.False(nemaPreklapanjaPoslije);

            // Slučaj 3: Potpuno preklapanje
            var potpunoPreklapanje = await terminRepository.HasOverlappingTerminAsync(
                GetPrivatePropertyValue<int>(zaposlenik, "Id"), DateTime.Today, new TimeSpan(10, 15, 0), 30);
            Assert.True(potpunoPreklapanje);

            // Slučaj 4: Preklapanje s početkom postojećeg termina
            var preklapanjePocetak = await terminRepository.HasOverlappingTerminAsync(
                GetPrivatePropertyValue<int>(zaposlenik, "Id"), DateTime.Today, new TimeSpan(9, 30, 0), 45);
            Assert.True(preklapanjePocetak);

            // Slučaj 5: Preklapanje s krajem postojećeg termina
            var preklapanjeKraj = await terminRepository.HasOverlappingTerminAsync(
                GetPrivatePropertyValue<int>(zaposlenik, "Id"), DateTime.Today, new TimeSpan(10, 30, 0), 45);
            Assert.True(preklapanjeKraj);

            // Slučaj 6: Točno isti termin
            var istiTermin = await terminRepository.HasOverlappingTerminAsync(
                GetPrivatePropertyValue<int>(zaposlenik, "Id"), DateTime.Today, new TimeSpan(10, 0, 0), 60);
            Assert.True(istiTermin);

            // Slučaj 7: Točno isti termin, ali s isključenjem (kod ažuriranja)
            var istiTerminIskljucen = await terminRepository.HasOverlappingTerminAsync(
                GetPrivatePropertyValue<int>(zaposlenik, "Id"), DateTime.Today, new TimeSpan(10, 0, 0), 60, GetPrivatePropertyValue<int>(postojeciTermin, "Id"));
            Assert.False(istiTerminIskljucen);
        }
    }
}