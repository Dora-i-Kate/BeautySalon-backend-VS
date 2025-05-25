using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.PresentationMVC.Models
{
    public class MaterijalViewModel
    {
        public int MaterijalId { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        public string? Naziv { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.1, 10000, ErrorMessage = "Cijena mora biti između €0.1 i €10000")]
        public decimal? Cijena { get; set; }

        [Required(ErrorMessage = "Minimalna količina je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimalna količina mora biti veća od 1")]
        public int? MinimalnaKolicina { get; set; }

        [Required(ErrorMessage = "Trenutna količina je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Trenutna količina mora biti veća od 1")]
        public int? TrenutnaKolicina { get; set; }

        [Required(ErrorMessage = "Jedinica mjere je obavezna.")]
        public string? JedinicaMjere { get; set; }

        [Required(ErrorMessage = "Vrsta materijala je obavezna.")]
        public int? VrstaId { get; set; }
        public string? VrstaNaziv { get; set; }
        public SelectList? VrsteMaterijala { get; set; }
    }
}
