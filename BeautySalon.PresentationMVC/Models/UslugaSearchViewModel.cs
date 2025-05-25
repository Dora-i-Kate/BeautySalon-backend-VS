namespace BeautySalon.PresentationMVC.Models
{
    public class UslugaSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public List<UslugaViewModel> Usluge { get; set; } = new();
    }
}
