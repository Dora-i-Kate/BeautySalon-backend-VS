// BeautySalon.PresentationMVC\ViewModels\TerminSearchViewModel.cs
using BeautySalon.Domain.Models; // Za TerminStatus
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic; // Za List

namespace BeautySalon.PresentationMVC.ViewModels
{
    public class TerminSearchViewModel
    {
        public DateTime? SearchDatumOd { get; set; }
        public DateTime? SearchDatumDo { get; set; }
        public int? SearchZaposlenikId { get; set; }
        public TerminStatus? SearchStatus { get; set; }

        public SelectList? Zaposlenici { get; set; }
        public SelectList? StatusiTermina { get; set; }

        // PROMJENA OVDJE: Treba biti lista ViewModel-a
        public List<TerminViewModel> Termini { get; set; } = new List<TerminViewModel>();
    }
}