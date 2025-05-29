using Xunit;
using Moq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection; // Potrebno za GetPrivatePropertyValue i SetPrivateProperty
using BeautySalon.Application.Services;
using BeautySalon.Application.DTOs.Korisnik;
using BeautySalon.Application.DTOs.Termin;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using BeautySalon.Domain.Exceptions;
using BeautySalon.Domain.Services; // Za TerminValidator

namespace BeautySalon.Tests.Application
{
    public class ApplicationServiceTests
    {
        // Pomoćna metoda za dohvaćanje privatnih/internih svojstava
        private TValue GetPrivatePropertyValue<TValue>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                // Ako svojstvo nije pronađeno kao Public/NonPublic property, pokušajte ga kao private field
                var field = obj.GetType().GetField($"_{char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1)}", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    return (TValue)field.GetValue(obj);
                }
                throw new InvalidOperationException($"Property or Field '{propertyName}' not found on type '{obj.GetType().Name}'.");
            }
            return (TValue)property.GetValue(obj);
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


        // Test za KorisnikAppService
        [Fact]
        public async Task GetKlijentiForLookupAsync_ShouldReturnCorrectDtoList()
        {
            // Arrange
            var mockKorisnikRepository = new Mock<IKorisnikRepository>();

            // Stvaranje lažnih Korisnik entiteta s privatnim setterima i konstruktorima
            var klijent1 = CreateInstanceWithPrivateConstructor<Korisnik>();
            SetPrivateProperty(klijent1, "Id", 1);
            SetPrivateProperty(klijent1, "Ime", "Pero");
            SetPrivateProperty(klijent1, "Prezime", "Perić");
            SetPrivateProperty(klijent1, "Email", "pero@example.com");
            SetPrivateProperty(klijent1, "LozinkaHash", "hash1");
            SetPrivateProperty(klijent1, "DatumRegistracije", DateTime.Now);
            SetPrivateProperty(klijent1, "Telefon", "123456");
            SetPrivateProperty(klijent1, "UlogaId", (int)UlogaNaziv.Klijent);

            var klijent2 = CreateInstanceWithPrivateConstructor<Korisnik>();
            SetPrivateProperty(klijent2, "Id", 2);
            SetPrivateProperty(klijent2, "Ime", "Ana");
            SetPrivateProperty(klijent2, "Prezime", "Anić");
            SetPrivateProperty(klijent2, "Email", "ana@example.com");
            SetPrivateProperty(klijent2, "LozinkaHash", "hash2");
            SetPrivateProperty(klijent2, "DatumRegistracije", DateTime.Now);
            SetPrivateProperty(klijent2, "Telefon", "654321");
            SetPrivateProperty(klijent2, "UlogaId", (int)UlogaNaziv.Klijent);

            var klijentiList = new List<Korisnik> { klijent1, klijent2 };

            mockKorisnikRepository.Setup(r => r.GetByUlogaAsync(UlogaNaziv.Klijent))
                                  .ReturnsAsync(klijentiList);

            var service = new KorisnikAppService(mockKorisnikRepository.Object);

            // Act
            var result = await service.GetKlijentiForLookupAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, k => k.Id == 1 && k.ImePrezime == "Pero Perić");
            Assert.Contains(result, k => k.Id == 2 && k.ImePrezime == "Ana Anić");

            // Provjera da je GetByUlogaAsync pozvan točno jednom s ispravnim argumentom
            mockKorisnikRepository.Verify(r => r.GetByUlogaAsync(UlogaNaziv.Klijent), Times.Once);
        }

        [Fact]
        public async Task GetZaposleniciForLookupAsync_ShouldReturnCorrectDtoList()
        {
            // Arrange
            var mockKorisnikRepository = new Mock<IKorisnikRepository>();

            var zaposlenik1 = CreateInstanceWithPrivateConstructor<Korisnik>();
            SetPrivateProperty(zaposlenik1, "Id", 101);
            SetPrivateProperty(zaposlenik1, "Ime", "Marko");
            SetPrivateProperty(zaposlenik1, "Prezime", "Marković");
            SetPrivateProperty(zaposlenik1, "Email", "marko@example.com");
            SetPrivateProperty(zaposlenik1, "LozinkaHash", "hash3");
            SetPrivateProperty(zaposlenik1, "DatumRegistracije", DateTime.Now);
            SetPrivateProperty(zaposlenik1, "Telefon", "789012");
            SetPrivateProperty(zaposlenik1, "UlogaId", (int)UlogaNaziv.Zaposlenik);

            var zaposleniciList = new List<Korisnik> { zaposlenik1 };

            mockKorisnikRepository.Setup(r => r.GetByUlogaAsync(UlogaNaziv.Zaposlenik))
                                  .ReturnsAsync(zaposleniciList);

            var service = new KorisnikAppService(mockKorisnikRepository.Object);

            // Act
            var result = await service.GetZaposleniciForLookupAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Should contain only one employee
            Assert.Contains(result, k => k.Id == 101 && k.ImePrezime == "Marko Marković");

            mockKorisnikRepository.Verify(r => r.GetByUlogaAsync(UlogaNaziv.Zaposlenik), Times.Once);
        }


        
    }
}