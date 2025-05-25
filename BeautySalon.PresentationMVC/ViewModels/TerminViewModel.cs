using BeautySalon.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace BeautySalon.PresentationMVC.ViewModels
{
    /// <summary>
    /// ViewModel za prikaz i unos termina (Master-Detail).
    /// Koristi se u TerminiControlleru i Viewsima.
    /// </summary>
    public class TerminViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Datum je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum termina")]
        public DateTime Datum { get; set; }

        [Required(ErrorMessage = "Vrijeme je obavezno.")]
        [DataType(DataType.Time)]
        [Display(Name = "Vrijeme termina")]
        public TimeSpan Vrijeme { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno.")]
        [Range(15, 240, ErrorMessage = "Trajanje mora biti između 15 i 240 minuta.")]
        [Display(Name = "Trajanje (min)")]
        public int TrajanjeMinuta { get; set; }

        [Required(ErrorMessage = "Status je obavezan.")]
        [Display(Name = "Status termina")]
        public TerminStatus Status { get; set; }

        [Required(ErrorMessage = "Klijent je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite klijenta.")]
        [Display(Name = "Klijent")]
        public int KlijentId { get; set; }

        [Required(ErrorMessage = "Zaposlenik je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite zaposlenika.")]
        [Display(Name = "Zaposlenik")]
        public int ZaposlenikId { get; set; }

        public List<StavkaTerminaViewModel> StavkeTermina { get; set; } = new List<StavkaTerminaViewModel>();

        public decimal UkupnaCijena { get; set; }

        // Za padajuće liste
        public SelectList? Klijenti { get; set; }
        public SelectList? Zaposlenici { get; set; }
        public SelectList? Usluge { get; set; } // Za stavke termina
        public SelectList? StatusiTermina { get; set; } // Za status termina

        // Za pretraživanje (opcionalno, može biti i zaseban ViewModel)
        [Display(Name = "Pretraži po datumu od")]
        [DataType(DataType.Date)]
        public DateTime? SearchDatumOd { get; set; }
        [Display(Name = "Pretraži po datumu do")]
        [DataType(DataType.Date)]
        public DateTime? SearchDatumDo { get; set; }
        [Display(Name = "Pretraži po zaposleniku")]
        public int? SearchZaposlenikId { get; set; }
        [Display(Name = "Pretraži po statusu")]
        public TerminStatus? SearchStatus { get; set; }
    }
}