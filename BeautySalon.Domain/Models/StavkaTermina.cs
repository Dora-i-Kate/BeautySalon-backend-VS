using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    /// <summary>
    /// Reprezentira stavku unutar termina.
    /// Detail entitet za master-detail formu.
    /// </summary>
    public class StavkaTermina
    {
        public int Id { get; private set; } // stavka_id
        public int Kolicina { get; private set; }
        public decimal Cijena { get; private set; } // Cijena za tu stavku (može biti različita od Usluga.Cijena zbog popusta)

        public int UslugaId { get; private set; } // strani ključ za Uslugu
        public Usluga Usluga { get; private set; } // Navigacijsko svojstvo

        public int TerminId { get; private set; } // strani ključ za Termin
        // Termin Termin { get; private set; } // Navigacijsko svojstvo (ne treba ovdje zbog jednosmjerne navigacije od mastera)

        // Prazan konstruktor za EF Core
        private StavkaTermina() { }

        /// <summary>
        /// Konstruktor za stvaranje nove stavke termina.
        /// </summary>
        public StavkaTermina(int uslugaId, int kolicina, decimal cijena)
        {
            if (uslugaId <= 0) throw new ArgumentException("Usluga ID mora biti pozitivan broj.", nameof(uslugaId));
            if (kolicina <= 0) throw new ArgumentException("Količina mora biti pozitivna.", nameof(kolicina));
            if (cijena < 0) throw new ArgumentException("Cijena ne smije biti negativna.", nameof(cijena));

            UslugaId = uslugaId;
            Kolicina = kolicina;
            Cijena = cijena;
        }

        /// <summary>
        /// Ažurira podatke stavke termina.
        /// </summary>
        public void Update(int uslugaId, int kolicina, decimal cijena)
        {
            if (uslugaId <= 0) throw new ArgumentException("Usluga ID mora biti pozitivan broj.", nameof(uslugaId));
            if (kolicina <= 0) throw new ArgumentException("Količina mora biti pozitivna.", nameof(kolicina));
            if (cijena < 0) throw new ArgumentException("Cijena ne smije biti negativna.", nameof(cijena));

            UslugaId = uslugaId;
            Kolicina = kolicina;
            Cijena = cijena;
        }
    }
}
