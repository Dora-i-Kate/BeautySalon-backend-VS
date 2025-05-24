using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    public class Zaposlenik : Korisnik
    {
        public string Specijalizacija { get; set; }
        public string Certifikat { get; set; }
        public DateTime DatumZaposlenja { get; set; }
    }

}
