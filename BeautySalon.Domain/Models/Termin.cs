using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    public class Termin
    {
        public int TerminId { get; set; }
        public DateTime DatumVrijeme { get; set; }
        public string Status { get; set; }
        public int KlijentId { get; set; }
        public int ZaposlenikId { get; set; }
        public string? KomentarZaposlenika { get; set; }

        public Klijent Klijent { get; set; }
        public Zaposlenik Zaposlenik { get; set; }
    }
}
