// BeautySalon.PresentationMVC\ViewModels\TerminViewModel.cs
using BeautySalon.Domain.Models; // Za TerminStatus
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic; // Za List

namespace BeautySalon.PresentationMVC.ViewModels
{
    public class TerminViewModel
    {
        public int Id { get; set; }

        // Postojeća svojstva
        public DateTime Datum { get; set; }
        public TimeSpan Vrijeme { get; set; }
        public int TrajanjeMinuta { get; set; }
        public TerminStatus Status { get; set; }

        // Foreign keys
        public int KlijentId { get; set; }
        public int ZaposlenikId { get; set; }

        // Nova svojstva koja nedostaju:
        public string? KlijentImePrezime { get; set; } // Dodaj ovo
        public string? ZaposlenikImePrezime { get; set; } // Dodaj ovo

        public decimal UkupnaCijena { get; set; }

        // Lista stavki termina
        public List<StavkaTerminaViewModel> StavkeTermina { get; set; } = new List<StavkaTerminaViewModel>();

        // SelectListovi za dropdownove (za Create/Edit view)
        public SelectList? Klijenti { get; set; }
        public SelectList? Zaposlenici { get; set; }
        public SelectList? Usluge { get; set; }
        public SelectList? StatusiTermina { get; set; }
    }
}