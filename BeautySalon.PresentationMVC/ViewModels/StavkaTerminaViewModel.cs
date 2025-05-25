// In BeautySalon.PresentationMVC.ViewModels/StavkaTerminaViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace BeautySalon.PresentationMVC.ViewModels
{
    /// <summary>
    /// ViewModel za prikaz i unos stavki termina.
    /// Koristi se unutar TerminViewModela.
    /// </summary>
    public class StavkaTerminaViewModel
    {
        public int Id { get; set; } // 0 za nove stavke

        [Required(ErrorMessage = "Usluga je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite uslugu.")]
        [Display(Name = "Usluga")]
        public int UslugaId { get; set; }

        [Display(Name = "Naziv usluge")]
        public string? UslugaNaziv { get; set; } // <-- MAKE IT NULLABLE
                                                 // Or remove it if it's strictly not needed on the ViewModel,
                                                 // but for display purposes, nullable is fine.

        [Required(ErrorMessage = "Količina je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti barem 1.")]
        [Display(Name = "Količina")]
        public int Kolicina { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti pozitivna.")]
        [Display(Name = "Cijena")]
        public decimal Cijena { get; set; }
    }
}