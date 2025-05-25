using System.ComponentModel.DataAnnotations;

namespace BeautySalon.PresentationMVC.ViewModels
{
    /// <summary>
    /// ViewModel za prikaz i unos usluga (Šifrarnik).
    /// Koristi se u UslugeControlleru i Viewsima.
    /// </summary>
    public class UslugaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv usluge je obavezan.")]
        [StringLength(255, ErrorMessage = "Naziv usluge ne smije biti duži od 255 znakova.")]
        [Display(Name = "Naziv usluge")]
        public string Naziv { get; set; }

        [StringLength(1000, ErrorMessage = "Opis usluge ne smije biti duži od 1000 znakova.")]
        [Display(Name = "Opis usluge")]
        public string Opis { get; set; }

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
