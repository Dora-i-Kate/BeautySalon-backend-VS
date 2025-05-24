using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs
{
    public class UpdateTerminDto
    {
        public int TerminId { get; set; }
        public DateTime DatumVrijeme { get; set; }
        public int ZaposlenikId { get; set; }
        public string Status { get; set; }

        // Dodaj ovo:
        public int KlijentId { get; set; }
        public string KomentarZaposlenika { get; set; }
    }


}
