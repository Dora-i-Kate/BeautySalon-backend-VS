using System.ComponentModel.DataAnnotations;


namespace BeautySalon.PresentationMVC.ViewModels
{
    public class StavkaTerminaViewModel
    {
        public int Id { get; set; } // 0 za nove stavke

        [Required(ErrorMessage = "Usluga je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite uslugu.")]
        [Display(Name = "Usluga")]
        public int UslugaId { get; set; }

        [Display(Name = "Naziv usluge")]
        public string? UslugaNaziv { get; set; }

        [Required(ErrorMessage = "Količina je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Količina mora biti barem 1.")]
        [Display(Name = "Količina")]
        public int Kolicina { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti pozitivna.")]
        [Display(Name = "Cijena")]
        public decimal Cijena { get; set; }

        public bool IsDeleted { get; set; } // DODANO: Za označavanje brisanja
    }
}