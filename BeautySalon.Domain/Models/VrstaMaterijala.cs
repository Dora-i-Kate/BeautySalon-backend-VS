// BeautySalon.Domain.Models.VrstaMaterijala.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    /// <summary>
    /// Reprezentira vrstu materijala.
    /// Entitet za šifrarnik.
    /// </summary>
    public class VrstaMaterijala
    {
        public int VrstaId { get; private set; } // Primarni ključ
        public string Naziv { get; private set; }

        // Prazan konstruktor za EF Core
        private VrstaMaterijala() { }

        /// <summary>
        /// Konstruktor za stvaranje nove vrste materijala.
        /// </summary>
        public VrstaMaterijala(string naziv)
        {
            if (string.IsNullOrWhiteSpace(naziv)) throw new ArgumentException("Naziv vrste materijala ne smije biti prazan.", nameof(naziv));
            Naziv = naziv;
        }

        /// <summary>
        /// Ažurira naziv vrste materijala.
        /// </summary>
        public void Update(string naziv)
        {
            if (string.IsNullOrWhiteSpace(naziv)) throw new ArgumentException("Naziv vrste materijala ne smije biti prazan.", nameof(naziv));
            Naziv = naziv;
        }
    }
}