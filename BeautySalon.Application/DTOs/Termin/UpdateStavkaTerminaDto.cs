using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs.Termin
{
    /// <summary>
    /// DTO za ažuriranje postojeće stavke termina.
    /// </summary>
    public class UpdateStavkaTerminaDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Usluga je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite uslugu.")]
        public int UslugaId { get; set; }

        [Required(ErrorMessage = "Količina je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti barem 1.")]
        public int Kolicina { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti pozitivna.")]
        public decimal Cijena { get; set; }
    }
}
