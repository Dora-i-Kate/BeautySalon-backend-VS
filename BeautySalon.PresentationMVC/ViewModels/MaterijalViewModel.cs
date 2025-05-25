// BeautySalon.PresentationMVC\ViewModels\MaterijalViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.PresentationMVC.ViewModels
{
    public class MaterijalViewModel
    {
        public int MaterijalId { get; set; } // Vraćeno na MaterijalId

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [Display(Name = "Naziv materijala")]
        public string Naziv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.1, 10000, ErrorMessage = "Cijena mora biti između €0.1 i €10000")]
        [Display(Name = "Cijena")]
        public decimal Cijena { get; set; }

        [Required(ErrorMessage = "Minimalna količina je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimalna količina mora biti veća od 1")]
        [Display(Name = "Minimalna količina")]
        public int MinimalnaKolicina { get; set; }

        [Required(ErrorMessage = "Trenutna količina je obavezna.")]
        [Range(0, int.MaxValue, ErrorMessage = "Trenutna količina ne može biti negativna.")]
        [Display(Name = "Trenutna količina")]
        public int TrenutnaKolicina { get; set; }

        [Required(ErrorMessage = "Jedinica mjere je obavezna.")]
        [Display(Name = "Jedinica mjere")]
        public string JedinicaMjere { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vrsta materijala je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite vrstu materijala.")]
        [Display(Name = "Vrsta materijala")]
        public int VrstaId { get; set; }
        public string? VrstaNaziv { get; set; }
        public SelectList? VrsteMaterijala { get; set; }
    }
}