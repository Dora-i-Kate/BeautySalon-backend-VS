// BeautySalon.PresentationMVC\ViewModels\UslugaSearchViewModel.cs
namespace BeautySalon.PresentationMVC.ViewModels
{
    public class UslugaSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public List<UslugaViewModel> Usluge { get; set; } = new();
    }
}