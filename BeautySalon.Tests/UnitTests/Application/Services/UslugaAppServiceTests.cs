using NUnit.Framework;
using Moq;
using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services;
using BeautySalon.Application.DTOs.Usluga;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using BeautySalon.Domain.Services; // Pretpostavljam da je UslugaValidator ovdje
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using BeautySalon.Domain.Exceptions; // Dodano: Pretpostavljam da je ValidationException ovdje

namespace BeautySalon.UnitTests.Application.Services
{
    [TestFixture]
    public class UslugaAppServiceTests
    {
        private Mock<IUslugaRepository> _mockUslugaRepository;
        private Mock<UslugaValidator> _mockUslugaValidator;
        private UslugaAppService _uslugaAppService;

        [SetUp]
        public void Setup()
        {
            _mockUslugaRepository = new Mock<IUslugaRepository>();
            // Prosljeđujemo mock repozitorij jer validator ovisi o njemu
            _mockUslugaValidator = new Mock<UslugaValidator>(_mockUslugaRepository.Object);
            _uslugaAppService = new UslugaAppService(_mockUslugaRepository.Object, _mockUslugaValidator.Object);
        }

        [Test]
        public async Task GetUslugaByIdAsync_UslugaExists_ReturnsUslugaDto()
        {
            // Arrange
            var mockUsluga = new Mock<Usluga>("Test Usluga", "Opis test usluge", 100m, 60);
            mockUsluga.SetupGet(u => u.Id).Returns(1); // Mockiramo Id
            _mockUslugaRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockUsluga.Object);

            // Act
            var result = await _uslugaAppService.GetUslugaByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(mockUsluga.Object.Id));
            Assert.That(result.Naziv, Is.EqualTo(mockUsluga.Object.Naziv));
            _mockUslugaRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public async Task GetUslugaByIdAsync_UslugaDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockUslugaRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Usluga)null);

            // Act
            var result = await _uslugaAppService.GetUslugaByIdAsync(99);

            // Assert
            Assert.That(result, Is.Null);
            _mockUslugaRepository.Verify(repo => repo.GetByIdAsync(99), Times.Once);
        }

        [Test]
        public async Task GetAllUslugeAsync_ReturnsListOfUslugaDtos()
        {
            // Arrange
            var mockUsluga1 = new Mock<Usluga>("Usluga 1", "Opis 1", 50m, 30);
            mockUsluga1.SetupGet(u => u.Id).Returns(1);

            var mockUsluga2 = new Mock<Usluga>("Usluga 2", "Opis 2", 75m, 45);
            mockUsluga2.SetupGet(u => u.Id).Returns(2);

            var usluge = new List<Usluga>
            {
                mockUsluga1.Object,
                mockUsluga2.Object
            };
            _mockUslugaRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(usluge);

            // Act
            var result = await _uslugaAppService.GetAllUslugeAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Naziv, Is.EqualTo("Usluga 1"));
            _mockUslugaRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task SearchUslugeAsync_ReturnsFilteredListOfUslugaDtos()
        {
            // Arrange
            var mockUsluga1 = new Mock<Usluga>("Šišanje", "Kratko šišanje", 25m, 30);
            mockUsluga1.SetupGet(u => u.Id).Returns(1);

            var mockUsluga2 = new Mock<Usluga>("Feniranje", "Dugo feniranje", 20m, 20);
            mockUsluga2.SetupGet(u => u.Id).Returns(2);

            var usluge = new List<Usluga>
            {
                mockUsluga1.Object,
                mockUsluga2.Object
            };
            _mockUslugaRepository.Setup(repo => repo.SearchUslugeAsync("šišanje")).ReturnsAsync(usluge.Where(u => u.Naziv.Contains("Šišanje")));

            // Act
            var result = await _uslugaAppService.SearchUslugeAsync("šišanje");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Naziv, Is.EqualTo("Šišanje"));
            _mockUslugaRepository.Verify(repo => repo.SearchUslugeAsync("šišanje"), Times.Once);
        }

        [Test]
        public async Task CreateUslugaAsync_ValidDto_CreatesAndReturnsDto()
        {
            // Arrange
            var createDto = new CreateUslugaDto
            {
                Naziv = "Nova Usluga",
                Opis = "Opis nove usluge",
                Cijena = 120m,
                TrajanjeMinuta = 90
            };

            // Važno: Mockiramo domenski objekt koji će biti vraćen od repozitorija
            var mockCreatedUsluga = new Mock<Usluga>("Nova Usluga", "Opis nove usluge", 120m, 90);
            mockCreatedUsluga.SetupGet(u => u.Id).Returns(5);

            _mockUslugaRepository.Setup(repo => repo.AddAsync(It.IsAny<Usluga>()))
                                 .ReturnsAsync(mockCreatedUsluga.Object); // Vraćamo mockirani objekt

            _mockUslugaValidator.Setup(v => v.ValidateUslugaAsync(It.IsAny<Usluga>(), It.IsAny<bool>()))
                                 .Returns(Task.CompletedTask); // Pretpostavljamo da validacija prolazi

            // Act
            var result = await _uslugaAppService.CreateUslugaAsync(createDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(mockCreatedUsluga.Object.Id));
            Assert.That(result.Naziv, Is.EqualTo(mockCreatedUsluga.Object.Naziv));
            _mockUslugaRepository.Verify(repo => repo.AddAsync(It.IsAny<Usluga>()), Times.Once);
            _mockUslugaValidator.Verify(v => v.ValidateUslugaAsync(It.IsAny<Usluga>(), true), Times.Once);
        }

        [Test]
        public void CreateUslugaAsync_ValidationFails_ThrowsException()
        {
            // Arrange
            var createDto = new CreateUslugaDto
            {
                Naziv = "Invalid Usluga",
                Cijena = 0m, // Invalid price
                TrajanjeMinuta = 10
            };

            // Postavljanje validatora da baci iznimku za nevažeći unos
            _mockUslugaValidator.Setup(v => v.ValidateUslugaAsync(It.IsAny<Usluga>(), It.IsAny<bool>()))
                                 .ThrowsAsync(new DomainValidationException("Validation failed."));

            // Act & Assert
            var ex = Assert.ThrowsAsync<DomainValidationException>(() => _uslugaAppService.CreateUslugaAsync(createDto));
            Assert.That(ex.Message, Is.EqualTo("Validation failed."));
            _mockUslugaRepository.Verify(repo => repo.AddAsync(It.IsAny<Usluga>()), Times.Never); // Ne bi se trebalo pozvati
            _mockUslugaValidator.Verify(v => v.ValidateUslugaAsync(It.IsAny<Usluga>(), true), Times.Once);
        }

        [Test]
        public async Task UpdateUslugaAsync_UslugaExists_UpdatesUsluga()
        {
            // Arrange
            var mockExistingUsluga = new Mock<Usluga>("Stara Usluga", "Stari Opis", 50m, 30);
            mockExistingUsluga.SetupGet(u => u.Id).Returns(1); // Mockiramo Id postojećeg objekta

            var updateDto = new UpdateUslugaDto
            {
                Id = 1,
                Naziv = "Ažurirana Usluga",
                Opis = "Ažuriran opis",
                Cijena = 60m,
                TrajanjeMinuta = 40
            };

            _mockUslugaRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mockExistingUsluga.Object);
            _mockUslugaRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Usluga>())).Returns(Task.CompletedTask);
            _mockUslugaValidator.Setup(v => v.ValidateUslugaAsync(It.IsAny<Usluga>(), It.IsAny<bool>()))
                                 .Returns(Task.CompletedTask); // Pretpostavljamo da validacija prolazi

            // Act
            await _uslugaAppService.UpdateUslugaAsync(updateDto);

            // Assert
            _mockUslugaRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            // Provjeravamo da li je UpdateAsync pozvan s ispravnim ažuriranim podacima
            _mockUslugaRepository.Verify(repo => repo.UpdateAsync(It.Is<Usluga>(u => u.Id == 1 && u.Naziv == "Ažurirana Usluga" && u.Cijena == 60m)), Times.Once);
            _mockUslugaValidator.Verify(v => v.ValidateUslugaAsync(It.IsAny<Usluga>(), false), Times.Once);
        }

        [Test]
        public void UpdateUslugaAsync_UslugaDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var updateDto = new UpdateUslugaDto { Id = 99, Naziv = "Non Existent", Cijena = 10m, TrajanjeMinuta = 15 };
            _mockUslugaRepository.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Usluga)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _uslugaAppService.UpdateUslugaAsync(updateDto));
            Assert.That(ex.Message, Is.EqualTo("Usluga s ID-jem 99 nije pronađena."));
            _mockUslugaRepository.Verify(repo => repo.GetByIdAsync(99), Times.Once);
            _mockUslugaRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Usluga>()), Times.Never);
            _mockUslugaValidator.Verify(v => v.ValidateUslugaAsync(It.IsAny<Usluga>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task DeleteUslugaAsync_UslugaExists_DeletesUsluga()
        {
            // Arrange
            _mockUslugaRepository.Setup(repo => repo.UslugaExistsAsync(1)).ReturnsAsync(true);
            _mockUslugaRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            await _uslugaAppService.DeleteUslugaAsync(1);

            // Assert
            _mockUslugaRepository.Verify(repo => repo.UslugaExistsAsync(1), Times.Once);
            _mockUslugaRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Test]
        public void DeleteUslugaAsync_UslugaDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            _mockUslugaRepository.Setup(repo => repo.UslugaExistsAsync(99)).ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _uslugaAppService.DeleteUslugaAsync(99));
            Assert.That(ex.Message, Is.EqualTo("Usluga s ID-jem 99 nije pronađena."));
            _mockUslugaRepository.Verify(repo => repo.UslugaExistsAsync(99), Times.Once);
            _mockUslugaRepository.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}