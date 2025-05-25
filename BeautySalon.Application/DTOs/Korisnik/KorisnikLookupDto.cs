using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs.Korisnik
{
    /// <summary>
    /// DTO za prikaz korisnika u padajućim listama (lookup).
    /// </summary>
    public class KorisnikLookupDto
    {
        public int Id { get; set; }
        public string ImePrezime { get; set; }
    }
}
