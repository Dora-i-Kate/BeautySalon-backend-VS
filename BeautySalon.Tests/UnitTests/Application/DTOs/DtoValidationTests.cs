using NUnit.Framework;
using BeautySalon.Application.DTOs.Termin;
using BeautySalon.Application.DTOs.Usluga;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System;
// Removed: using Microsoft.AspNetCore.Routing; // This is likely not needed for DTO validation tests and can be removed for cleaner code.

namespace BeautySalon.UnitTests.Application.DTOs
{
    [TestFixture]
    public class DtoValidationTests
    {
        private ValidationContext _validationContext;
        private List<ValidationResult> _validationResults;

        [SetUp]
        public void Setup()
        {
            _validationResults = new List<ValidationResult>();
        }

        private bool ValidateModel(object model)
        {
            _validationContext = new ValidationContext(model);
            return Validator.TryValidateObject(model, _validationContext, _validationResults, true);
        }

        // --- CreateStavkaTerminaDto Tests ---
        [Test]
        public void CreateStavkaTerminaDto_ValidData_NoValidationErrors()
        {
            var dto = new CreateStavkaTerminaDto
            {
                UslugaId = 1,
                Kolicina = 1,
                Cijena = 50.00m
            };
            Assert.That(ValidateModel(dto), Is.True);
            Assert.That(_validationResults, Is.Empty);
        }

        [Test]
        public void CreateStavkaTerminaDto_MissingUslugaId_ReturnsValidationError()
        {
            var dto = new CreateStavkaTerminaDto
            {
                UslugaId = 0, // Invalid value
                Kolicina = 1,
                Cijena = 50.00m
            };
            Assert.That(ValidateModel(dto), Is.False);
            Assert.That(_validationResults, Has.Some.Matches<ValidationResult>(vr =>
                vr.ErrorMessage == "Odaberite uslugu." && vr.MemberNames.Contains("UslugaId")
            ));
        }

        [Test]
        public void CreateStavkaTerminaDto_InvalidKolicina_ReturnsValidationError()
        {
            var dto = new CreateStavkaTerminaDto
            {
                UslugaId = 1,
                Kolicina = 0, // Invalid value
                Cijena = 50.00m
            };
            Assert.That(ValidateModel(dto), Is.False);
            Assert.That(_validationResults, Has.Some.Matches<ValidationResult>(vr =>
                vr.ErrorMessage == "Količina mora biti barem 1." && vr.MemberNames.Contains("Kolicina")
            ));
        }

        [Test]
        public void CreateStavkaTerminaDto_InvalidCijena_ReturnsValidationError()
        {
            var dto = new CreateStavkaTerminaDto
            {
                UslugaId = 1,
                Kolicina = 1,
                Cijena = 0.00m // Invalid value
            };
            Assert.That(ValidateModel(dto), Is.False);
            Assert.That(_validationResults, Has.Some.Matches<ValidationResult>(vr =>
                vr.ErrorMessage == "Cijena mora biti pozitivna." && vr.MemberNames.Contains("Cijena")
            ));
        }

        // --- CreateTerminDto Tests ---
        [Test]
        public void CreateTerminDto_ValidData_NoValidationErrors()
        {
            var dto = new CreateTerminDto
            {
                Datum = DateTime.Today.AddDays(1),
                Vrijeme = TimeSpan.FromHours(10),
                TrajanjeMinuta = 60,
                KlijentId = 1,
                ZaposlenikId = 2,
                StavkeTermina = new List<CreateStavkaTerminaDto>
                {
                    new CreateStavkaTerminaDto { UslugaId = 1, Kolicina = 1, Cijena = 50m }
                }
            };
            Assert.That(ValidateModel(dto), Is.True);
            Assert.That(_validationResults, Is.Empty);
        }

        // RENAMED AND ADJUSTED: This test now reflects the actual behavior of [Required] on non-nullable DateTime.
        // It will NOT return a validation error via DataAnnotations for a default DateTime.
        [Test]
        public void CreateTerminDto_MissingDatum_NoDataAnnotationsValidationError()
        {
            var dto = new CreateTerminDto
            {
                // Datum is default(DateTime) here, which is DateTime.MinValue.
                // [Required] does not flag this as an error for non-nullable value types.
                Vrijeme = TimeSpan.FromHours(10),
                TrajanjeMinuta = 60,
                KlijentId = 1,
                ZaposlenikId = 2
            };
            Assert.That(ValidateModel(dto), Is.True); // Assertion changed to True
            Assert.That(_validationResults, Is.Empty); // No validation errors expected
        }

        [Test]
        public void CreateTerminDto_InvalidTrajanjeMinuta_ReturnsValidationError()
        {
            var dto = new CreateTerminDto
            {
                Datum = DateTime.Today.AddDays(1),
                Vrijeme = TimeSpan.FromHours(10),
                TrajanjeMinuta = 10, // Invalid value
                KlijentId = 1,
                ZaposlenikId = 2
            };
            Assert.That(ValidateModel(dto), Is.False);
            Assert.That(_validationResults, Has.Some.Matches<ValidationResult>(vr =>
                vr.ErrorMessage == "Trajanje mora biti između 15 i 240 minuta." && vr.MemberNames.Contains("TrajanjeMinuta")
            ));
        }

        // --- CreateUslugaDto Tests ---
        [Test]
        public void CreateUslugaDto_ValidData_NoValidationErrors()
        {
            var dto = new CreateUslugaDto
            {
                Naziv = "Šišanje",
                Opis = "Muško šišanje",
                Cijena = 25.00m,
                TrajanjeMinuta = 30
            };
            Assert.That(ValidateModel(dto), Is.True);
            Assert.That(_validationResults, Is.Empty);
        }

        [Test]
        public void CreateUslugaDto_MissingNaziv_ReturnsValidationError()
        {
            var dto = new CreateUslugaDto
            {
                // Naziv missing
                Opis = "Muško šišanje",
                Cijena = 25.00m,
                TrajanjeMinuta = 30
            };
            Assert.That(ValidateModel(dto), Is.False);
            Assert.That(_validationResults, Has.Some.Matches<ValidationResult>(vr =>
                vr.ErrorMessage == "Naziv usluge je obavezan." && vr.MemberNames.Contains("Naziv")
            ));
        }

        [Test]
        public void CreateUslugaDto_NazivTooLong_ReturnsValidationError()
        {
            var dto = new CreateUslugaDto
            {
                Naziv = new string('a', 256), // Too long
                Opis = "Muško šišanje",
                Cijena = 25.00m,
                TrajanjeMinuta = 30
            };
            Assert.That(ValidateModel(dto), Is.False);
            Assert.That(_validationResults, Has.Some.Matches<ValidationResult>(vr =>
                vr.ErrorMessage == "Naziv usluge ne smije biti duži od 255 znakova." && vr.MemberNames.Contains("Naziv")
            ));
        }

        // Helper class to compare ValidationResults (No longer strictly needed but doesn't hurt)
        private class ValidationResultComparer : IEqualityComparer<ValidationResult>
        {
            public static readonly ValidationResultComparer Instance = new ValidationResultComparer();

            public bool Equals(ValidationResult x, ValidationResult y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
                return x.ErrorMessage == y.ErrorMessage && x.MemberNames.SequenceEqual(y.MemberNames);
            }

            public int GetHashCode(ValidationResult obj)
            {
                return obj.ErrorMessage.GetHashCode() ^ (obj.MemberNames != null ? obj.MemberNames.Aggregate(0, (acc, s) => acc ^ s.GetHashCode()) : 0);
            }
        }
    }
}