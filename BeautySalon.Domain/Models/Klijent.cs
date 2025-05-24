using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    public class Klijent : Korisnik
    {
        public string? Zahtjevi { get; set; }  // Npr. opis zahtjeva
        public int BrojBodova { get; set; }
    }
}
