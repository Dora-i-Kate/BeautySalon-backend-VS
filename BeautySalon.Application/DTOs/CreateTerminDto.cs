using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs
{
    public class CreateTerminDto
    {
        [Required]
        public int KlijentId { get; set; }

        [Required]
        public int ZaposlenikId { get; set; }

        [Required]
        public DateTime DatumVrijeme { get; set; }

        public string Status { get; set; } = "Zakazan";

        public string? KomentarZaposlenika { get; set; }
    }

}
