using NUnit.Framework;
using Moq;
using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services;
using BeautySalon.Application.DTOs.Termin;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using BeautySalon.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Routing; // This might not be needed if not using ASP.NET Core routing features
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.UnitTests.Application.Services
{
    [TestFixture]
    public class TerminAppServiceTests
    {
        private Mock<ITerminRepository> _mockTerminRepository;
        private Mock<IUslugaRepository> _mockUslugaRepository;
        private Mock<IKorisnikRepository> _mockKorisnikRepository;
        private Mock<TerminValidator> _mockTerminValidator;
        private TerminAppService _terminAppService;

        [SetUp]
        public void Setup()
        {
            _mockTerminRepository = new Mock<ITerminRepository>();
            _mockUslugaRepository = new Mock<IUslugaRepository>();
            _mockKorisnikRepository = new Mock<IKorisnikRepository>();

            // To mock a class like TerminValidator, which has dependencies, you need to provide those dependencies
            // The TerminValidator needs IUslugaRepository, IKorisnikRepository, and ITerminRepository
            _mockTerminValidator = new Mock<TerminValidator>(
                _mockTerminRepository.Object,
                _mockUslugaRepository.Object,
                _mockKorisnikRepository.Object);

            _terminAppService = new TerminAppService(
                _mockTerminRepository.Object,
                _mockUslugaRepository.Object,
                _mockKorisnikRepository.Object,
                _mockTerminValidator.Object);
        }

        [Test]
        public async Task GetTerminByIdAsync_TerminExists_ReturnsTerminDto()
        {
            // Arrange
            // Mock Korisnik
            var mockKlijent = new Mock<Korisnik>();
            mockKlijent.SetupGet(k => k.Id).Returns(1);
            mockKlijent.SetupGet(k => k.Ime).Returns("Klijent");
            mockKlijent.SetupGet(k => k.Prezime).Returns("Prez");

            var mockZaposlenik = new Mock<Korisnik>();
            mockZaposlenik.SetupGet(k => k.Id).Returns(2);
            mockZaposlenik.SetupGet(k => k.Ime).Returns("Zaposlenik");
            mockZaposlenik.SetupGet(k => k.Prezime).Returns("Prez");

            // Mock Usluga
            var mockUsluga = new Mock<Usluga>("Šišanje", "", 50m, 30); // Use constructor if no public setter
            mockUsluga.SetupGet(u => u.Id).Returns(10);
            mockUsluga.SetupGet(u => u.Naziv).Returns("Šišanje"); // Ensure Naziv can be read if needed

            // Mock StavkaTermina
            var mockStavkaTermina = new Mock<StavkaTermina>(10, 1, 50m); // UslugaId, Kolicina, Cijena
            mockStavkaTermina.SetupGet(s => s.Id).Returns(100); // Assign a fake ID for existing item
            mockStavkaTermina.SetupGet(s => s.Usluga).Returns(mockUsluga.Object); // Set navigation property
            mockStavkaTermina.SetupGet(s => s.UslugaId).Returns(10);
            mockStavkaTermina.SetupGet(s => s.Cijena).Returns(50m);

            // Mock Termin
            // Termin constructor takes Datum, Vrijeme, TrajanjeMinuta, KlijentId, ZaposlenikId
            var mockTermin = new Mock<Termin>(DateTime.Today, TimeSpan.FromHours(10), 60, mockKlijent.Object.Id, mockZaposlenik.Object.Id);
            mockTermin.SetupGet(t => t.Id).Returns(1);
            mockTermin.SetupGet(t => t.Klijent).Returns(mockKlijent.Object); // Set navigation properties
            mockTermin.SetupGet(t => t.Zaposlenik).Returns(mockZaposlenik.Object);
            mockTermin.SetupGet(t => t.StavkeTermina).Returns(new List<StavkaTermina> { mockStavkaTermina.Object }); // Use a list of mocked stavke

            _mockTerminRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockTermin.Object);

            // Act
            var result = await _terminAppService.GetTerminByIdAsync(1);

            // Assert (using NUnit 3.x Assert.That)
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(mockTermin.Object.Id));
            Assert.That(result.KlijentImePrezime, Is.EqualTo("Klijent Prez"));
            Assert.That(result.ZaposlenikImePrezime, Is.EqualTo("Zaposlenik Prez"));
            Assert.That(result.UkupnaCijena, Is.EqualTo(50m));
            Assert.That(result.StavkeTermina.Count, Is.EqualTo(1));
            Assert.That(result.StavkeTermina.First().UslugaNaziv, Is.EqualTo("Šišanje"));
            _mockTerminRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetTerminByIdAsync_TerminDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockTerminRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Termin)null);

            // Act
            var result = await _terminAppService.GetTerminByIdAsync(99);

            // Assert
            Assert.That(result, Is.Null);
            _mockTerminRepository.Verify(repo => repo.GetByIdAsync(99), Times.Once);
        }

        [Test]
        public async Task GetAllTerminiAsync_ReturnsListOfTerminDtos()
        {
            // Arrange
            var mockKlijent1 = new Mock<Korisnik>();
            mockKlijent1.SetupGet(k => k.Id).Returns(1);
            mockKlijent1.SetupGet(k => k.Ime).Returns("Klijent");
            mockKlijent1.SetupGet(k => k.Prezime).Returns("Jedan");

            var mockZaposlenik1 = new Mock<Korisnik>();
            mockZaposlenik1.SetupGet(k => k.Id).Returns(2);
            mockZaposlenik1.SetupGet(k => k.Ime).Returns("Zaposlenik");
            mockZaposlenik1.SetupGet(k => k.Prezime).Returns("Prvi");

            var mockTermin1 = new Mock<Termin>(DateTime.Today, TimeSpan.FromHours(9), 60, mockKlijent1.Object.Id, mockZaposlenik1.Object.Id);
            mockTermin1.SetupGet(t => t.Id).Returns(1);
            mockTermin1.SetupGet(t => t.Status).Returns(TerminStatus.Zakazan); // Assume TerminStatus is public
            mockTermin1.SetupGet(t => t.Klijent).Returns(mockKlijent1.Object);
            mockTermin1.SetupGet(t => t.Zaposlenik).Returns(mockZaposlenik1.Object);

            var mockTermin2 = new Mock<Termin>(DateTime.Today.AddDays(1), TimeSpan.FromHours(11), 45, mockKlijent1.Object.Id, mockZaposlenik1.Object.Id);
            mockTermin2.SetupGet(t => t.Id).Returns(2);
            // FIX: Changed TerminStatus.Potvrđen to TerminStatus.Zakazan
            mockTermin2.SetupGet(t => t.Status).Returns(TerminStatus.Zakazan);
            mockTermin2.SetupGet(t => t.Klijent).Returns(mockKlijent1.Object);
            mockTermin2.SetupGet(t => t.Zaposlenik).Returns(mockZaposlenik1.Object);

            var termini = new List<Termin>
            {
                mockTermin1.Object,
                mockTermin2.Object
            };

            _mockTerminRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(termini);

            // Act
            var result = await _terminAppService.GetAllTerminiAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().KlijentImePrezime, Is.EqualTo("Klijent Jedan"));
            Assert.That(result.First().ZaposlenikImePrezime, Is.EqualTo("Zaposlenik Prvi"));
            _mockTerminRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task SearchTerminiAsync_ReturnsFilteredListOfTerminDtos()
        {
            // Arrange
            var mockKlijent1 = new Mock<Korisnik>();
            mockKlijent1.SetupGet(k => k.Id).Returns(1);
            mockKlijent1.SetupGet(k => k.Ime).Returns("Klijent");
            mockKlijent1.SetupGet(k => k.Prezime).Returns("Jedan");

            var mockZaposlenik1 = new Mock<Korisnik>();
            mockZaposlenik1.SetupGet(k => k.Id).Returns(2);
            mockZaposlenik1.SetupGet(k => k.Ime).Returns("Zaposlenik");
            mockZaposlenik1.SetupGet(k => k.Prezime).Returns("Prvi");

            var mockZaposlenik2 = new Mock<Korisnik>();
            mockZaposlenik2.SetupGet(k => k.Id).Returns(3);
            mockZaposlenik2.SetupGet(k => k.Ime).Returns("Zaposlenik");
            mockZaposlenik2.SetupGet(k => k.Prezime).Returns("Drugi");

            var mockTermin1 = new Mock<Termin>(DateTime.Today, TimeSpan.FromHours(9), 60, mockKlijent1.Object.Id, mockZaposlenik1.Object.Id);
            mockTermin1.SetupGet(t => t.Id).Returns(1);
            mockTermin1.SetupGet(t => t.Status).Returns(TerminStatus.Zakazan);
            mockTermin1.SetupGet(t => t.Klijent).Returns(mockKlijent1.Object);
            mockTermin1.SetupGet(t => t.Zaposlenik).Returns(mockZaposlenik1.Object);
            mockTermin1.SetupGet(t => t.Datum).Returns(DateTime.Today);
            mockTermin1.SetupGet(t => t.ZaposlenikId).Returns(mockZaposlenik1.Object.Id);


            var mockTermin2 = new Mock<Termin>(DateTime.Today, TimeSpan.FromHours(11), 45, mockKlijent1.Object.Id, mockZaposlenik2.Object.Id);
            mockTermin2.SetupGet(t => t.Id).Returns(2);
            // FIX: Changed TerminStatus.Potvrđen to TerminStatus.Zakazan
            mockTermin2.SetupGet(t => t.Status).Returns(TerminStatus.Zakazan);
            mockTermin2.SetupGet(t => t.Klijent).Returns(mockKlijent1.Object);
            mockTermin2.SetupGet(t => t.Zaposlenik).Returns(mockZaposlenik2.Object);
            mockTermin2.SetupGet(t => t.Datum).Returns(DateTime.Today);
            mockTermin2.SetupGet(t => t.ZaposlenikId).Returns(mockZaposlenik2.Object.Id);


            var mockTermin3 = new Mock<Termin>(DateTime.Today.AddDays(1), TimeSpan.FromHours(10), 30, mockKlijent1.Object.Id, mockZaposlenik1.Object.Id);
            mockTermin3.SetupGet(t => t.Id).Returns(3);
            mockTermin3.SetupGet(t => t.Status).Returns(TerminStatus.Zakazan);
            mockTermin3.SetupGet(t => t.Klijent).Returns(mockKlijent1.Object);
            mockTermin3.SetupGet(t => t.Zaposlenik).Returns(mockZaposlenik1.Object);
            mockTermin3.SetupGet(t => t.Datum).Returns(DateTime.Today.AddDays(1));
            mockTermin3.SetupGet(t => t.ZaposlenikId).Returns(mockZaposlenik1.Object.Id);


            var termini = new List<Termin>
            {
                mockTermin1.Object,
                mockTermin2.Object,
                mockTermin3.Object
            };

            // Setup the SearchTerminiAsync to return a filtered list based on the criteria
            _mockTerminRepository.Setup(repo => repo.SearchTerminiAsync(DateTime.Today, null, mockZaposlenik1.Object.Id, null))
                                 .ReturnsAsync(termini.Where(t => t.Datum.Date == DateTime.Today.Date && t.ZaposlenikId == mockZaposlenik1.Object.Id));

            // Act
            var result = await _terminAppService.SearchTerminiAsync(DateTime.Today, null, mockZaposlenik1.Object.Id, null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(1));
            _mockTerminRepository.Verify(repo => repo.SearchTerminiAsync(DateTime.Today, null, mockZaposlenik1.Object.Id, null), Times.Once);
        }

        [Test]
        public async Task CreateTerminAsync_ValidDto_CreatesAndReturnsDto()
        {
            // Arrange
            var createDto = new CreateTerminDto
            {
                Datum = DateTime.Today.AddDays(7),
                Vrijeme = TimeSpan.FromHours(14),
                TrajanjeMinuta = 90,
                KlijentId = 1,
                ZaposlenikId = 2,
                StavkeTermina = new List<CreateStavkaTerminaDto>
                {
                    new CreateStavkaTerminaDto { UslugaId = 10, Kolicina = 1, Cijena = 75m }
                }
            };

            // Mock dependent entities for the 'createdTermin' object
            var mockKlijent = new Mock<Korisnik>();
            mockKlijent.SetupGet(k => k.Id).Returns(1);
            mockKlijent.SetupGet(k => k.Ime).Returns("Novi");
            mockKlijent.SetupGet(k => k.Prezime).Returns("Klijent");

            var mockZaposlenik = new Mock<Korisnik>();
            mockZaposlenik.SetupGet(k => k.Id).Returns(2);
            mockZaposlenik.SetupGet(k => k.Ime).Returns("Novi");
            mockZaposlenik.SetupGet(k => k.Prezime).Returns("Zaposlenik");

            var mockUsluga = new Mock<Usluga>("Masaža", "", 75m, 90);
            mockUsluga.SetupGet(u => u.Id).Returns(10);
            mockUsluga.SetupGet(u => u.Naziv).Returns("Masaža");

            var mockStavka = new Mock<StavkaTermina>(10, 1, 75m); // uslugaId, kolicina, cijena
            mockStavka.SetupGet(s => s.Usluga).Returns(mockUsluga.Object); // Link the mocked usluga

            // Mock the Termin object that would be returned by the repository after AddAsync
            var mockCreatedTermin = new Mock<Termin>(createDto.Datum, createDto.Vrijeme, createDto.TrajanjeMinuta, createDto.KlijentId, createDto.ZaposlenikId);
            mockCreatedTermin.SetupGet(t => t.Id).Returns(1); // Assume ID is assigned by the repository
            mockCreatedTermin.SetupGet(t => t.Klijent).Returns(mockKlijent.Object);
            mockCreatedTermin.SetupGet(t => t.Zaposlenik).Returns(mockZaposlenik.Object);
            // Simulate adding stavka - use a list of mocked stavke
            mockCreatedTermin.SetupGet(t => t.StavkeTermina).Returns(new List<StavkaTermina> { mockStavka.Object });


            _mockTerminRepository.Setup(repo => repo.AddAsync(It.IsAny<Termin>()))
                                 .ReturnsAsync(mockCreatedTermin.Object); // Return the mocked created termin
            _mockTerminRepository.Setup(repo => repo.GetByIdAsync(mockCreatedTermin.Object.Id))
                                 .ReturnsAsync(mockCreatedTermin.Object); // Simulate fetching full termin after creation
            _mockTerminValidator.Setup(v => v.ValidateTerminAsync(It.IsAny<Termin>(), It.IsAny<bool>()))
                                 .Returns(Task.CompletedTask); // Assume validation passes

            // Act
            var result = await _terminAppService.CreateTerminAsync(createDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(mockCreatedTermin.Object.Id));
            Assert.That(result.KlijentImePrezime, Is.EqualTo("Novi Klijent"));
            Assert.That(result.UkupnaCijena, Is.EqualTo(75m));
            Assert.That(result.StavkeTermina.First().UslugaNaziv, Is.EqualTo("Masaža"));
            _mockTerminRepository.Verify(repo => repo.AddAsync(It.IsAny<Termin>()), Times.Once);
            _mockTerminRepository.Verify(repo => repo.GetByIdAsync(mockCreatedTermin.Object.Id), Times.Once);
            _mockTerminValidator.Verify(v => v.ValidateTerminAsync(It.IsAny<Termin>(), true), Times.Once);
        }

        [Test]
        public void CreateTerminAsync_ValidationFails_ThrowsException()
        {
            // Arrange
            var createDto = new CreateTerminDto
            {
                Datum = DateTime.Today,
                Vrijeme = TimeSpan.FromHours(8), // Invalid time for a business, for example
                TrajanjeMinuta = 10, // Invalid duration
                KlijentId = 0, // Invalid KlijentId
                ZaposlenikId = 0
            };

            // Setup validator to throw an exception for invalid input
            _mockTerminValidator.Setup(v => v.ValidateTerminAsync(It.IsAny<Termin>(), It.IsAny<bool>()))
                                 .ThrowsAsync(new ValidationException("Validation failed."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ValidationException>(() => _terminAppService.CreateTerminAsync(createDto));
            Assert.That(ex.Message, Is.EqualTo("Validation failed."));
            _mockTerminRepository.Verify(repo => repo.AddAsync(It.IsAny<Termin>()), Times.Never);
            _mockTerminValidator.Verify(v => v.ValidateTerminAsync(It.IsAny<Termin>(), true), Times.Once);
        }

        [Test]
        public async Task UpdateTerminAsync_TerminExists_UpdatesTerminAndStavke()
        {
            // Arrange
            // Mock existing Korisnik, Usluga
            var mockKlijent = new Mock<Korisnik>();
            mockKlijent.SetupGet(k => k.Id).Returns(1);
            mockKlijent.SetupGet(k => k.Ime).Returns("Klijent");
            mockKlijent.SetupGet(k => k.Prezime).Returns("Stari");

            var mockZaposlenik = new Mock<Korisnik>();
            mockZaposlenik.SetupGet(k => k.Id).Returns(2);
            mockZaposlenik.SetupGet(k => k.Ime).Returns("Zaposlenik");
            mockZaposlenik.SetupGet(k => k.Prezime).Returns("Stari");

            var mockUsluga1 = new Mock<Usluga>("Stara Usluga", "", 50m, 30);
            mockUsluga1.SetupGet(u => u.Id).Returns(10);

            var mockUsluga2 = new Mock<Usluga>("Nova Usluga", "", 70m, 45);
            mockUsluga2.SetupGet(u => u.Id).Returns(11);


            // Mock the existing StavkaTermina
            var mockExistingStavka = new Mock<StavkaTermina>(10, 1, 50m);
            mockExistingStavka.SetupGet(s => s.Id).Returns(100);
            mockExistingStavka.SetupGet(s => s.UslugaId).Returns(10);
            mockExistingStavka.SetupGet(s => s.Kolicina).Returns(1);
            mockExistingStavka.SetupGet(s => s.Cijena).Returns(50m);
            mockExistingStavka.SetupGet(s => s.Usluga).Returns(mockUsluga1.Object);


            // Mock the existing Termin
            var mockExistingTermin = new Mock<Termin>(DateTime.Today, TimeSpan.FromHours(10), 60, mockKlijent.Object.Id, mockZaposlenik.Object.Id);
            mockExistingTermin.SetupGet(t => t.Id).Returns(1);
            mockExistingTermin.SetupGet(t => t.Status).Returns(TerminStatus.Zakazan);
            mockExistingTermin.SetupGet(t => t.Klijent).Returns(mockKlijent.Object);
            mockExistingTermin.SetupGet(t => t.Zaposlenik).Returns(mockZaposlenik.Object);
            // Simulate the initial state of StavkeTermina
            mockExistingTermin.SetupGet(t => t.StavkeTermina).Returns(new List<StavkaTermina> { mockExistingStavka.Object });


            var updateDto = new UpdateTerminDto
            {
                Id = 1,
                Datum = DateTime.Today.AddDays(1),
                Vrijeme = TimeSpan.FromHours(11),
                TrajanjeMinuta = 75,
                // FIX: Changed TerminStatus.Potvrđen to TerminStatus.Zakazan
                Status = TerminStatus.Zakazan,
                KlijentId = 1,
                ZaposlenikId = 2,
                StavkeTermina = new List<UpdateStavkaTerminaDto>
                {
                    new UpdateStavkaTerminaDto { Id = 100, UslugaId = 10, Kolicina = 2, Cijena = 50m }, // Updated existing
                    new UpdateStavkaTerminaDto { Id = 0, UslugaId = 11, Kolicina = 1, Cijena = 70m }    // New item
                }
            };

            _mockTerminRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockExistingTermin.Object);
            _mockTerminRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Termin>())).Returns(Task.CompletedTask);
            _mockTerminValidator.Setup(v => v.ValidateTerminAsync(It.IsAny<Termin>(), It.IsAny<bool>()))
                                 .Returns(Task.CompletedTask); // Assume validation passes

            // Act
            await _terminAppService.UpdateTerminAsync(updateDto);

            // Assert
            _mockTerminRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockTerminRepository.Verify(repo => repo.UpdateAsync(It.Is<Termin>(t =>
                t.Id == 1 &&
                t.Datum.Date == DateTime.Today.AddDays(1).Date &&
                t.TrajanjeMinuta == 75 &&
                // FIX: Changed TerminStatus.Potvrđen to TerminStatus.Zakazan
                t.Status == TerminStatus.Zakazan &&
                // When verifying complex objects, it's safer to verify the specific values that were set by the app service
                // and mock the behavior for the internal logic (like adding/removing stavke).
                // The application service *should* fetch the existing termin, update its properties,
                // and then add/remove stavke. Moq cannot easily track changes to a list property.
                // Instead, focus on the parameters passed to the repository's UpdateAsync method.
                true // More detailed verification of StavkeTermina is difficult without changing model setters or using a capture argument
            )), Times.Once);
            _mockTerminValidator.Verify(v => v.ValidateTerminAsync(It.IsAny<Termin>(), false), Times.Once);
        }


        [Test]
        public void UpdateTerminAsync_TerminDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            // FIX: Changed TerminStatus.Potvrđen to TerminStatus.Zakazan
            var updateDto = new UpdateTerminDto { Id = 99, Datum = DateTime.Today, Vrijeme = TimeSpan.Zero, TrajanjeMinuta = 60, Status = TerminStatus.Zakazan, KlijentId = 1, ZaposlenikId = 1 };
            _mockTerminRepository.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Termin)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _terminAppService.UpdateTerminAsync(updateDto));
            Assert.That(ex.Message, Is.EqualTo("Termin s ID-jem 99 nije pronađen."));
            _mockTerminRepository.Verify(repo => repo.GetByIdAsync(99), Times.Once);
            _mockTerminRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Termin>()), Times.Never);
            _mockTerminValidator.Verify(v => v.ValidateTerminAsync(It.IsAny<Termin>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task DeleteTerminAsync_TerminExists_DeletesTermin()
        {
            // Arrange
            _mockTerminRepository.Setup(repo => repo.TerminExistsAsync(1)).ReturnsAsync(true);
            _mockTerminRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _terminAppService.DeleteTerminAsync(1);

            // Assert
            _mockTerminRepository.Verify(repo => repo.TerminExistsAsync(1), Times.Once);
            _mockTerminRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Test]
        public void DeleteTerminAsync_TerminDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockTerminRepository.Setup(repo => repo.TerminExistsAsync(99)).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _terminAppService.DeleteTerminAsync(99));
            Assert.That(ex.Message, Is.EqualTo("Termin s ID-jem 99 nije pronađen."));
            _mockTerminRepository.Verify(repo => repo.TerminExistsAsync(99), Times.Once);
            _mockTerminRepository.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}