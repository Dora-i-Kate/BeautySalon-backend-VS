using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.PresentationMVC.Models
{
    public class MaterijalViewModel
    {
        public int MaterijalId { get; set; }

        [Required]
        public string? Naziv { get; set; }

        [Range(0.01, 10000)]
        public decimal Cijena { get; set; }

        [Range(0, int.MaxValue)]
        public int MinimalnaKolicina { get; set; }

        [Range(0, int.MaxValue)]
        public int TrenutnaKolicina { get; set; }

        [Required]
        public string? JedinicaMjere { get; set; }

        [Required]
        public int VrstaId { get; set; }
        public string? VrstaNaziv { get; set; }
        public SelectList? VrsteMaterijala { get; set; }
    }
}
