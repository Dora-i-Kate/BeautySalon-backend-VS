using System.ComponentModel.DataAnnotations;

namespace BeautySalon.PresentationMVC.Models
{
    public class UslugaViewModel
    {
        public int UslugaId { get; set; }

        [Required(ErrorMessage = "Naziv usluge je obavezan.")]
        public required string Naziv { get; set; }

        [Required(ErrorMessage = "Opis je obavezan.")]
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno.")]
        [Range(10, 1440, ErrorMessage = "Trajanje usluge mora biti između 10 i 1440 minuta.")]
        public int? Trajanje { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.1, 10000, ErrorMessage = "Cijena mora biti između €0.1 i €10000")]
        public decimal? Cijena { get; set; }
    }
}
