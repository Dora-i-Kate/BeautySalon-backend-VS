// BeautySalon.Domain.Models.Materijal.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    /// <summary>
    /// Reprezentira materijal koji se koristi u salonu.
    /// Entitet za šifrarnik.
    /// </summary>
    public class Materijal
    {
        public int MaterijalId { get; private set; } // Primarni ključ
        public string Naziv { get; private set; }
        public decimal Cijena { get; private set; }
        public int MinimalnaKolicina { get; private set; }
        public int TrenutnaKolicina { get; private set; }
        public string JedinicaMjere { get; private set; }

        public int VrstaId { get; private set; } // Strani ključ za VrstaMaterijala
        public VrstaMaterijala VrstaMaterijala { get; private set; } // Navigacijsko svojstvo

        // Prazan konstruktor za EF Core
        private Materijal() { }

        /// <summary>
        /// Konstruktor za stvaranje novog materijala.
        /// </summary>
        public Materijal(string naziv, decimal cijena, int minimalnaKolicina, int trenutnaKolicina, string jedinicaMjere, int vrstaId)
        {
            if (string.IsNullOrWhiteSpace(naziv)) throw new ArgumentException("Naziv materijala ne smije biti prazan.", nameof(naziv));
            if (cijena <= 0) throw new ArgumentException("Cijena materijala mora biti pozitivna.", nameof(cijena));
            if (minimalnaKolicina < 0) throw new ArgumentException("Minimalna količina ne smije biti negativna.", nameof(minimalnaKolicina));
            if (trenutnaKolicina < 0) throw new ArgumentException("Trenutna količina ne smije biti negativna.", nameof(trenutnaKolicina));
            if (string.IsNullOrWhiteSpace(jedinicaMjere)) throw new ArgumentException("Jedinica mjere ne smije biti prazna.", nameof(jedinicaMjere));
            if (vrstaId <= 0) throw new ArgumentException("Vrsta ID mora biti pozitivan broj.", nameof(vrstaId));

            Naziv = naziv;
            Cijena = cijena;
            MinimalnaKolicina = minimalnaKolicina;
            TrenutnaKolicina = trenutnaKolicina;
            JedinicaMjere = jedinicaMjere;
            VrstaId = vrstaId;
        }

        /// <summary>
        /// Ažurira podatke materijala.
        /// </summary>
        public void Update(string naziv, decimal cijena, int minimalnaKolicina, int trenutnaKolicina, string jedinicaMjere, int vrstaId)
        {
            if (string.IsNullOrWhiteSpace(naziv)) throw new ArgumentException("Naziv materijala ne smije biti prazan.", nameof(naziv));
            if (cijena <= 0) throw new ArgumentException("Cijena materijala mora biti pozitivna.", nameof(cijena));
            if (minimalnaKolicina < 0) throw new ArgumentException("Minimalna količina ne smije biti negativna.", nameof(minimalnaKolicina));
            if (trenutnaKolicina < 0) throw new ArgumentException("Trenutna količina ne smije biti negativna.", nameof(trenutnaKolicina));
            if (string.IsNullOrWhiteSpace(jedinicaMjere)) throw new ArgumentException("Jedinica mjere ne smije biti prazna.", nameof(jedinicaMjere));
            if (vrstaId <= 0) throw new ArgumentException("Vrsta ID mora biti pozitivan broj.", nameof(vrstaId));

            Naziv = naziv;
            Cijena = cijena;
            MinimalnaKolicina = minimalnaKolicina;
            TrenutnaKolicina = trenutnaKolicina;
            JedinicaMjere = jedinicaMjere;
            VrstaId = vrstaId;
        }

        /// <summary>
        /// Ažurira trenutnu količinu materijala.
        /// </summary>
        public void SetTrenutnaKolicina(int kolicina)
        {
            if (kolicina < 0) throw new ArgumentException("Količina ne smije biti negativna.", nameof(kolicina));
            TrenutnaKolicina = kolicina;
        }
    }
}