using NUnit.Framework;
using Moq;
using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services; // Ensure this points to your KorisnikAppService
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection; // Needed for the reflection workaround in CreateSimulatedKorisnik

namespace BeautySalon.UnitTests.Application.Services
{
    [TestFixture]
    public class KorisnikAppServiceTests
    {
        private Mock<IKorisnikRepository> _mockKorisnikRepository;
        private KorisnikAppService _korisnikAppService;

        [SetUp]
        public void Setup()
        {
            _mockKorisnikRepository = new Mock<IKorisnikRepository>();
            _korisnikAppService = new KorisnikAppService(_mockKorisnikRepository.Object);
        }

        [Test]
        public async Task GetKlijentiForLookupAsync_ReturnsCorrectDtoList()
        {
            // Arrange
            // Create a real Uloga instance as your domain model expects
            var ulogaKlijent = new Uloga(UlogaNaziv.Klijent.ToString());

            // Create real Korisnik instances with IDs, simulating data from a database.
            // These are CONCRETE objects, not Moq-generated mocks of Korisnik.
            var klijent1 = CreateSimulatedKorisnik(1, "Test", "Klijent1", "test1@example.com", "hash1", "123", (int)UlogaNaziv.Klijent, ulogaKlijent);
            var klijent2 = CreateSimulatedKorisnik(2, "Drugi", "Klijent2", "test2@example.com", "hash2", "456", (int)UlogaNaziv.Klijent, ulogaKlijent);

            var klijenti = new List<Korisnik> { klijent1, klijent2 };

            // Setup the mock repository to return the list of clients.
            // Using Task.FromResult explicitly ensures Moq doesn't try to proxy
            // or deeply inspect your concrete Korisnik objects, which is where
            // the 'Non-overridable members' error comes from with non-virtual properties.
            _mockKorisnikRepository.Setup(repo => repo.GetByUlogaAsync(UlogaNaziv.Klijent))
                                   .Returns(Task.FromResult<IEnumerable<Korisnik>>(klijenti));

            // Act
            var result = await _korisnikAppService.GetKlijentiForLookupAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            Assert.That(result.First().ImePrezime, Is.EqualTo("Test Klijent1"));
            Assert.That(result.Last().Id, Is.EqualTo(2));
            Assert.That(result.Last().ImePrezime, Is.EqualTo("Drugi Klijent2"));

            // Verify that the repository method was called exactly once with the correct enum value.
            _mockKorisnikRepository.Verify(repo => repo.GetByUlogaAsync(UlogaNaziv.Klijent), Times.Once);
        }

        [Test]
        public async Task GetZaposleniciForLookupAsync_ReturnsCorrectDtoList()
        {
            // Arrange
            // Create a real Uloga instance
            var ulogaZaposlenik = new Uloga(UlogaNaziv.Zaposlenik.ToString());

            // Create real Korisnik instances
            var zaposlenik1 = CreateSimulatedKorisnik(3, "Test", "Zaposlenik1", "test3@example.com", "hash3", "789", (int)UlogaNaziv.Zaposlenik, ulogaZaposlenik);
            var zaposlenik2 = CreateSimulatedKorisnik(4, "Drugi", "Zaposlenik2", "test4@example.com", "hash4", "012", (int)UlogaNaziv.Zaposlenik, ulogaZaposlenik);

            var zaposlenici = new List<Korisnik> { zaposlenik1, zaposlenik2 };

            // Setup the mock repository to return the list of employees.
            // Using Task.FromResult to prevent Moq's property inspection issues.
            _mockKorisnikRepository.Setup(repo => repo.GetByUlogaAsync(UlogaNaziv.Zaposlenik))
                                   .Returns(Task.FromResult<IEnumerable<Korisnik>>(zaposlenici));

            // Act
            var result = await _korisnikAppService.GetZaposleniciForLookupAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(3));
            Assert.That(result.First().ImePrezime, Is.EqualTo("Test Zaposlenik1"));
            Assert.That(result.Last().Id, Is.EqualTo(4));
            Assert.That(result.Last().ImePrezime, Is.EqualTo("Drugi Zaposlenik2"));

            // Verify that the repository method was called exactly once with the correct enum value.
            _mockKorisnikRepository.Verify(repo => repo.GetByUlogaAsync(UlogaNaziv.Zaposlenik), Times.Once);
        }

        /// <summary>
        /// Helper method to create a Korisnik instance for testing, simulating an object loaded from a database.
        /// This directly creates a Korisnik using its public constructor and then, importantly,
        /// uses reflection to set the private-set Id property.
        /// This is the *only* way to assign an Id to a Korisnik if its 'Id' property
        /// has a private setter and no public way to set it, *and* you cannot modify the Korisnik class itself.
        /// </summary>
        private Korisnik CreateSimulatedKorisnik(int id, string ime, string prezime, string email, string lozinkaHash, string telefon, int ulogaId, Uloga uloga)
        {
            // Create the Korisnik instance using its public constructor
            var korisnik = new Korisnik(ime, prezime, email, lozinkaHash, telefon, ulogaId);

            // Use reflection to set the Id property (which has a private setter)
            var idProperty = typeof(Korisnik).GetProperty("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (idProperty != null)
            {
                idProperty.SetValue(korisnik, id);
            }

            // Also set the Uloga navigation property, as the constructor only sets UlogaId
            var ulogaProperty = typeof(Korisnik).GetProperty("Uloga", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (ulogaProperty != null)
            {
                ulogaProperty.SetValue(korisnik, uloga);
            }

            return korisnik;
        }
    }
}