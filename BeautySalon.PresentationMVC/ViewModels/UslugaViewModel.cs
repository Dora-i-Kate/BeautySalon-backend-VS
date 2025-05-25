// BeautySalon.PresentationMVC\ViewModels\UslugaViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.PresentationMVC.ViewModels
{
    public class UslugaViewModel
    {
        public int Id { get; set; } // Koristite Id za konzistentnost

        [Required(ErrorMessage = "Naziv usluge je obavezan.")]
        [StringLength(255, ErrorMessage = "Naziv usluge ne smije biti duži od 255 znakova.")]
        [Display(Name = "Naziv usluge")]
        public string Naziv { get; set; } = string.Empty; // Inicijalizirajte

        [StringLength(1000, ErrorMessage = "Opis usluge ne smije biti duži od 1000 znakova.")]
        [Display(Name = "Opis usluge")]
        public string Opis { get; set; } = string.Empty; // Inicijalizirajte

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti veća od nule.")]
        [Display(Name = "Cijena")]
        public decimal Cijena { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno.")]
        [Range(15, 240, ErrorMessage = "Trajanje mora biti između 15 i 240 minuta.")]
        [Display(Name = "Trajanje (min)")]
        public int TrajanjeMinuta { get; set; }
    }
}