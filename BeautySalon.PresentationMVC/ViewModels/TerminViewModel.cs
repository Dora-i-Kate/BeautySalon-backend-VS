using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BeautySalon.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeautySalon.PresentationMVC.ViewModels
{
    public class TerminViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Datum je obavezan.")]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }

        [Required(ErrorMessage = "Vrijeme je obavezno.")]
        [Display(Name = "Vrijeme")]
        [DataType(DataType.Time)]
        public TimeSpan Vrijeme { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno.")]
        [Range(1, int.MaxValue, ErrorMessage = "Trajanje mora biti barem 1 minuta.")]
        [Display(Name = "Trajanje (min.)")]
        public int TrajanjeMinuta { get; set; }

        [Required(ErrorMessage = "Status je obavezan.")]
        [Display(Name = "Status")]
        public TerminStatus Status { get; set; }

        [Required(ErrorMessage = "Klijent je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite klijenta.")]
        [Display(Name = "Klijent")]
        public int KlijentId { get; set; }

        [Required(ErrorMessage = "Zaposlenik je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite zaposlenika.")]
        [Display(Name = "Zaposlenik")]
        public int ZaposlenikId { get; set; }

        [Display(Name = "Ukupna cijena")]
        public decimal UkupnaCijena { get; set; }

        public List<StavkaTerminaViewModel> StavkeTermina { get; set; } = new List<StavkaTerminaViewModel>();

        // SelectListe za dropdownove
        public SelectList? Klijenti { get; set; }
        public SelectList? Zaposlenici { get; set; }
        public SelectList? StatusiTermina { get; set; }
        public SelectList? Usluge { get; set; } // <-- DODAJ OVO SVOJSTVO


        // Svojstva za pretragu (ako se koristi isti ViewModel za Index)
        [Display(Name = "Datum od")]
        [DataType(DataType.Date)]
        public DateTime? SearchDatumOd { get; set; }

        [Display(Name = "Datum do")]
        [DataType(DataType.Date)]
        public DateTime? SearchDatumDo { get; set; }

        [Display(Name = "Zaposlenik")]
        public int? SearchZaposlenikId { get; set; }

        [Display(Name = "Status")]
        public TerminStatus? SearchStatus { get; set; }
    }
}