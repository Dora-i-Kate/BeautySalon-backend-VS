using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    /// <summary>
    /// Reprezentira uslugu koju pruža kozmetički salon.
    /// Entitet za šifrarnik.
    /// </summary>
    public class Usluga
    {
        public int Id { get; private set; } // usluga_id
        public string Naziv { get; private set; } // naziv_usluge
        public string Opis { get; private set; } // opis_usluge
        public decimal Cijena { get; private set; } // cijena
        public int TrajanjeMinuta { get; private set; } // trajanje u minutama

        // Prazan konstruktor za EF Core
        private Usluga() { }

        /// <summary>
        /// Konstruktor za stvaranje nove usluge.
        /// </summary>
        public Usluga(string naziv, string opis, decimal cijena, int trajanjeMinuta)
        {
            if (string.IsNullOrWhiteSpace(naziv)) throw new ArgumentException("Naziv usluge ne smije biti prazan.", nameof(naziv));
            if (cijena <= 0) throw new ArgumentException("Cijena usluge mora biti pozitivna.", nameof(cijena));
            if (trajanjeMinuta <= 0) throw new ArgumentException("Trajanje usluge mora biti pozitivno.", nameof(trajanjeMinuta));

            Naziv = naziv;
            Opis = opis;
            Cijena = cijena;
            TrajanjeMinuta = trajanjeMinuta;
        }

        /// <summary>
        /// Ažurira podatke usluge.
        /// </summary>
        public void Update(string naziv, string opis, decimal cijena, int trajanjeMinuta)
        {
            if (string.IsNullOrWhiteSpace(naziv)) throw new ArgumentException("Naziv usluge ne smije biti prazan.", nameof(naziv));
            if (cijena <= 0) throw new ArgumentException("Cijena usluge mora biti pozitivna.", nameof(cijena));
            if (trajanjeMinuta <= 0) throw new ArgumentException("Trajanje usluge mora biti pozitivno.", nameof(trajanjeMinuta));

            Naziv = naziv;
            Opis = opis;
            Cijena = cijena;
            TrajanjeMinuta = trajanjeMinuta;
        }
    }
}
