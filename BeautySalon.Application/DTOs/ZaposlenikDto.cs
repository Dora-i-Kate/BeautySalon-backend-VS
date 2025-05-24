using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs
{
    public class ZaposlenikDto
    {
        public int KorisnikId { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string BrojTelefona { get; set; }
        public string Specijalizacija { get; set; }
        public string Certifikat { get; set; }
        public DateTime DatumZaposlenja { get; set; }

        public string FullName => $"{Ime} {Prezime}";
    }

}
