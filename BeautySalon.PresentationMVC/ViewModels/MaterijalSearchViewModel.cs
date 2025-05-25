// BeautySalon.PresentationMVC\ViewModels\MaterijalSearchViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering; // Dodano za SelectList

namespace BeautySalon.PresentationMVC.ViewModels
{
    public class MaterijalSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public List<MaterijalViewModel> Materijali { get; set; } = new();

        // Dodano za pretragu po vrsti materijala
        public int? SearchVrstaId { get; set; }
        public SelectList? VrsteMaterijala { get; set; } // SelectList za dropdown filter
    }
}