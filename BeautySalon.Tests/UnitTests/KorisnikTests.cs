using BeautySalon.Domain.Models;
using System;
using Xunit; // Koristimo samo Xunit

public class KorisnikTests
{
    [Fact]
    public void Korisnik_Constructor_ThrowsArgumentException_WhenImeIsEmpty()
    {
        // Arrange
        string invalidIme = "";
        string prezime = "Prezime";
        string email = "test@example.com";
        string lozinkaHash = "hash123";
        string telefon = "123456789";
        int ulogaId = 1;

        // Act & Assert
        // Xunit.Assert.Throws metoda
        var exception = Xunit.Assert.Throws<ArgumentException>(() =>
            new Korisnik(invalidIme, prezime, email, lozinkaHash, telefon, ulogaId));

        // Xunit.Assert.Contains metoda
        Xunit.Assert.Contains("Ime ne smije biti prazno.", exception.Message);
        // Xunit.Assert.Equal metoda
        Xunit.Assert.Equal("ime", exception.ParamName);
    }

    [Fact]
    public void Korisnik_Constructor_SetsPropertiesCorrectly_WhenValid()
    {
        // Arrange
        string ime = "Test";
        string prezime = "Korisnik";
        string email = "test@example.com";
        string lozinkaHash = "validhash";
        string telefon = "0987654321";
        int ulogaId = 2;

        // Act
        var korisnik = new Korisnik(ime, prezime, email, lozinkaHash, telefon, ulogaId);

        // Assert
        // Xunit.Assert.Equal metoda
        Xunit.Assert.Equal(ime, korisnik.Ime);
        Xunit.Assert.Equal(prezime, korisnik.Prezime);
        Xunit.Assert.Equal(email, korisnik.Email);
        Xunit.Assert.Equal(lozinkaHash, korisnik.LozinkaHash);
        Xunit.Assert.Equal(telefon, korisnik.Telefon);
        Xunit.Assert.Equal(ulogaId, korisnik.UlogaId);
        // Xunit.Assert.NotEqual metoda
        Xunit.Assert.NotEqual(default(DateTime), korisnik.DatumRegistracije); // Trebalo bi biti postavljeno
    }
}