using Xunit;
using Moq;
using BeautySalon.Domain.Models;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Services;
using BeautySalon.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection; // Potrebno za postavljanje private set ID-eva

public class TerminValidatorTests
{
    private readonly Mock<ITerminRepository> _mockTerminRepository;
    private readonly Mock<IKorisnikRepository> _mockKorisnikRepository;
    private readonly Mock<IUslugaRepository> _mockUslugaRepository;
    private readonly TerminValidator _terminValidator;

    public TerminValidatorTests()
    {
        _mockTerminRepository = new Mock<ITerminRepository>();
        _mockKorisnikRepository = new Mock<IKorisnikRepository>();
        _mockUslugaRepository = new Mock<IUslugaRepository>();
        _terminValidator = new TerminValidator(
            _mockTerminRepository.Object,
            _mockKorisnikRepository.Object,
            _mockUslugaRepository.Object);
    }

    // Pomoćna metoda za postavljanje privatnih settera ID-a i navigacijskih svojstava
    // Oprez: Korištenje Reflectiona u testovima je prihvatljivo za zaobilaženje privatnih settera
    // kada ne želite mijenjati produkcijski kod samo zbog testiranja.
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

    [Fact]
    public async Task ValidateTerminAsync_ThrowsException_WhenTerminHasOverlap()
    {
        // Arrange
        var termin = new Termin(DateTime.Today.AddDays(1), TimeSpan.FromHours(9), 60, 1, 10);
        bool isNew = true;

        _mockTerminRepository.Setup(r => r.HasOverlappingTerminAsync(
            termin.ZaposlenikId,
            termin.Datum,
            termin.Vrijeme,
            termin.TrajanjeMinuta,
            It.IsAny<int?>()))
            .ReturnsAsync(true);

        // Stvaramo STVARNE Uloga objekte
        var klijentUloga = new Uloga(UlogaNaziv.Klijent.ToString());
        SetPrivateProperty(klijentUloga, "Id", (int)UlogaNaziv.Klijent); // Postavljamo ID za Ulogu

        var zaposlenikUloga = new Uloga(UlogaNaziv.Zaposlenik.ToString());
        SetPrivateProperty(zaposlenikUloga, "Id", (int)UlogaNaziv.Zaposlenik); // Postavljamo ID za Ulogu

        // Kreiramo STVARNE Korisnik objekte, a ne mockove
        // Zatim koristimo Reflection da postavimo njihove privatne ID-ove i navigacijska svojstva
        var klijent = new Korisnik("KlijentIme", "KlijentPrezime", "klijent@example.com", "hash1", "12345", (int)UlogaNaziv.Klijent);
        SetPrivateProperty(klijent, "Id", termin.KlijentId); // Postavljamo ID klijenta
        SetPrivateProperty(klijent, "Uloga", klijentUloga); // Postavljamo Uloga navigacijsko svojstvo

        var zaposlenik = new Korisnik("ZaposlenikIme", "ZaposlenikPrezime", "zaposlenik@example.com", "hash2", "67890", (int)UlogaNaziv.Zaposlenik);
        SetPrivateProperty(zaposlenik, "Id", termin.ZaposlenikId); // Postavljamo ID zaposlenika
        SetPrivateProperty(zaposlenik, "Uloga", zaposlenikUloga); // Postavljamo Uloga navigacijsko svojstvo

        // Simulirajmo da repozitorij vraća ove STVARNE objekte
        _mockKorisnikRepository.Setup(r => r.GetByIdAsync(termin.KlijentId)).ReturnsAsync(klijent);
        _mockKorisnikRepository.Setup(r => r.GetByIdAsync(termin.ZaposlenikId)).ReturnsAsync(zaposlenik);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainValidationException>(() =>
            _terminValidator.ValidateTerminAsync(termin, isNew));

        Assert.NotNull(exception.Errors);
        Assert.True(exception.Errors.ContainsKey(nameof(termin.ZaposlenikId)));
        Assert.Contains("Odabrani zaposlenik već ima preklapajući termin u tom vremenskom razdoblju.", exception.Errors[nameof(termin.ZaposlenikId)][0]);
    }

    [Fact]
    public async Task ValidateTerminAsync_DoesNotThrowException_WhenTerminIsValid()
    {
        // Arrange
        var termin = new Termin(DateTime.Today.AddDays(1), TimeSpan.FromHours(14), 45, 1, 10);
        bool isNew = true;

        _mockTerminRepository.Setup(r => r.HasOverlappingTerminAsync(
            termin.ZaposlenikId,
            termin.Datum,
            termin.Vrijeme,
            termin.TrajanjeMinuta,
            It.IsAny<int?>()))
            .ReturnsAsync(false);

        // Stvaramo STVARNE Uloga objekte
        var klijentUloga = new Uloga(UlogaNaziv.Klijent.ToString());
        SetPrivateProperty(klijentUloga, "Id", (int)UlogaNaziv.Klijent);

        var zaposlenikUloga = new Uloga(UlogaNaziv.Zaposlenik.ToString());
        SetPrivateProperty(zaposlenikUloga, "Id", (int)UlogaNaziv.Zaposlenik);

        // Kreiramo STVARNE Korisnik objekte
        var klijent = new Korisnik("KlijentIme", "KlijentPrezime", "klijent@example.com", "hash1", "12345", (int)UlogaNaziv.Klijent);
        SetPrivateProperty(klijent, "Id", termin.KlijentId);
        SetPrivateProperty(klijent, "Uloga", klijentUloga);

        var zaposlenik = new Korisnik("ZaposlenikIme", "ZaposlenikPrezime", "zaposlenik@example.com", "hash2", "67890", (int)UlogaNaziv.Zaposlenik);
        SetPrivateProperty(zaposlenik, "Id", termin.ZaposlenikId);
        SetPrivateProperty(zaposlenik, "Uloga", zaposlenikUloga);

        _mockKorisnikRepository.Setup(r => r.GetByIdAsync(termin.KlijentId)).ReturnsAsync(klijent);
        _mockKorisnikRepository.Setup(r => r.GetByIdAsync(termin.ZaposlenikId)).ReturnsAsync(zaposlenik);

        // Simulirajmo da usluga postoji za stavku termina
        termin.AddStavka(101, 1, 50m);
        // Ovdje također, ako Usluga.Id ima private setter, morali biste koristiti SetPrivateProperty
        // No, pretpostavljam da Usluga ima javni setter za Id ili je automatski postavljen
        // za potrebe testiranja. Ako ne, SetPrivateProperty bi bio potreban i ovdje.
        var usluga = new Usluga("Šišanje", null, 50m, 30);
        SetPrivateProperty(usluga, "Id", 101); // Ako je Id private set

        _mockUslugaRepository.Setup(r => r.GetByIdAsync(101))
            .ReturnsAsync(usluga);


        // Act & Assert
        await _terminValidator.ValidateTerminAsync(termin, isNew);
    }
}