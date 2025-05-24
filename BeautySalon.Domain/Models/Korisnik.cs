using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    public class Korisnik
    {
        public int KorisnikId { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Lozinka { get; set; }
        public string BrojTelefona { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public DateTime? PosljednjaPrijava { get; set; }
        public Uloga Uloga { get; set; }
    }

}
