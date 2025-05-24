using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs
{
    public class TerminDto
    {
        public int TerminId { get; set; }
        public DateTime DatumVrijeme { get; set; }
        public string Status { get; set; }
        public int KlijentId { get; set; }
        public int ZaposlenikId { get; set; }
        public string? KomentarZaposlenika { get; set; }
    }
}
