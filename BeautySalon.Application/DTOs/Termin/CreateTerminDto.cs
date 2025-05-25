using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs.Termin
{
    /// <summary>
    /// DTO za stvaranje novog termina.
    /// </summary>
    public class CreateTerminDto
    {
        [Required(ErrorMessage = "Datum je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }

        [Required(ErrorMessage = "Vrijeme je obavezno.")]
        [DataType(DataType.Time)]
        public TimeSpan Vrijeme { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno.")]
        [Range(15, 240, ErrorMessage = "Trajanje mora biti između 15 i 240 minuta.")] // Osnovna validacija
        public int TrajanjeMinuta { get; set; }

        [Required(ErrorMessage = "Klijent je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite klijenta.")]
        public int KlijentId { get; set; }

        [Required(ErrorMessage = "Zaposlenik je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite zaposlenika.")]
        public int ZaposlenikId { get; set; }

        public List<CreateStavkaTerminaDto> StavkeTermina { get; set; } = new List<CreateStavkaTerminaDto>();
    }
}
