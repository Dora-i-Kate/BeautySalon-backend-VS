namespace BeautySalon.PresentationMVC.Models
{
    public class MaterijalSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public List<MaterijalViewModel> Materijali { get; set; } = new();
    }
}
