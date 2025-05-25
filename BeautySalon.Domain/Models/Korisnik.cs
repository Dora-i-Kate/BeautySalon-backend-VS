using System;
using System.Collections.Generic;

namespace BeautySalon.Domain.Models
{
    public class Korisnik
    {
        public int Id { get; private set; } // korisnik_id
        public string Ime { get; private set; }
        public string Prezime { get; private set; }
        public string Email { get; private set; }
        public string LozinkaHash { get; private set; } // Lozinka bi trebala biti hashirana
        public string Telefon { get; private set; }
        public DateTime DatumRegistracije { get; private set; }
        public DateTime? VrijemeZadnjePrijave { get; private set; }

        public int UlogaId { get; private set; } // strani ključ za Ulogu
        public Uloga Uloga { get; private set; } // Navigacijsko svojstvo

        // Prazan konstruktor za EF Core
        private Korisnik() { }

        /// <summary>
        /// Konstruktor za stvaranje novog korisnika.
        /// </summary>
        /// <param name="ime">Ime korisnika.</param>
        /// <param name="prezime">Prezime korisnika.</param>
        /// <param name="email">Email adresa korisnika.</param>
        /// <param name="lozinkaHash">Hashirana lozinka korisnika.</param>
        /// <param name="telefon">Telefonski broj korisnika.</param>
        /// <param name="ulogaId">ID uloge korisnika.</param>
        public Korisnik(string ime, string prezime, string email, string lozinkaHash, string telefon, int ulogaId)
        {
            if (string.IsNullOrWhiteSpace(ime)) throw new ArgumentException("Ime ne smije biti prazno.", nameof(ime));
            if (string.IsNullOrWhiteSpace(prezime)) throw new ArgumentException("Prezime ne smije biti prazno.", nameof(prezime));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email ne smije biti prazan.", nameof(email));
            if (string.IsNullOrWhiteSpace(lozinkaHash)) throw new ArgumentException("Lozinka hash ne smije biti prazna.", nameof(lozinkaHash));
            if (string.IsNullOrWhiteSpace(telefon)) throw new ArgumentException("Telefon ne smije biti prazan.", nameof(telefon));
            if (ulogaId <= 0) throw new ArgumentException("Uloga ID mora biti pozitivan broj.", nameof(ulogaId));

            Ime = ime;
            Prezime = prezime;
            Email = email;
            LozinkaHash = lozinkaHash;
            Telefon = telefon;
            DatumRegistracije = DateTime.UtcNow; // Postavlja se prilikom kreiranja
            UlogaId = ulogaId;
        }

        /// <summary>
        /// Ažurira podatke korisnika.
        /// </summary>
        public void Update(string ime, string prezime, string email, string telefon, int ulogaId)
        {
            if (string.IsNullOrWhiteSpace(ime)) throw new ArgumentException("Ime ne smije biti prazno.", nameof(ime));
            if (string.IsNullOrWhiteSpace(prezime)) throw new ArgumentException("Prezime ne smije biti prazno.", nameof(prezime));
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email ne smije biti prazan.", nameof(email));
            if (string.IsNullOrWhiteSpace(telefon)) throw new ArgumentException("Telefon ne smije biti prazan.", nameof(telefon));
            if (ulogaId <= 0) throw new ArgumentException("Uloga ID mora biti pozitivan broj.", nameof(ulogaId));

            Ime = ime;
            Prezime = prezime;
            Email = email;
            Telefon = telefon;
            UlogaId = ulogaId;
        }

        /// <summary>
        /// Ažurira hash lozinke korisnika.
        /// </summary>
        public void SetLozinkaHash(string lozinkaHash)
        {
            if (string.IsNullOrWhiteSpace(lozinkaHash)) throw new ArgumentException("Lozinka hash ne smije biti prazna.", nameof(lozinkaHash));
            LozinkaHash = lozinkaHash;
        }

        /// <summary>
        /// Postavlja vrijeme zadnje prijave korisnika.
        /// </summary>
        public void SetVrijemeZadnjePrijave(DateTime vrijemePrijave)
        {
            VrijemeZadnjePrijave = vrijemePrijave;
        }
    }
}
