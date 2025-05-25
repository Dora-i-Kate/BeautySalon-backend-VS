using System.Collections.Generic;

namespace BeautySalon.Domain.Models
{
    public class Uloga
    {
        public int Id { get; set; } // uloga_id
        public string NazivUloge { get; private set; }

        // Prazan konstruktor za EF Core
        private Uloga() { }

        /// <summary>
        /// Konstruktor za stvaranje nove uloge.
        /// </summary>
        /// <param name="nazivUloge">Naziv uloge.</param>
        public Uloga(string nazivUloge)
        {
            if (string.IsNullOrWhiteSpace(nazivUloge)) throw new ArgumentException("Naziv uloge ne smije biti prazan.", nameof(nazivUloge));
            NazivUloge = nazivUloge;
        }

        /// <summary>
        /// Ažurira naziv uloge.
        /// </summary>
        public void UpdateNaziv(string nazivUloge)
        {
            if (string.IsNullOrWhiteSpace(nazivUloge)) throw new ArgumentException("Naziv uloge ne smije biti prazan.", nameof(nazivUloge));
            NazivUloge = nazivUloge;
        }
    }
}
