namespace BeautySalon.PresentationMVC.Models
{
    public class UslugaViewModel
    {
        public int UslugaId { get; set; }
        public required string Naziv { get; set; }
        public string? Opis { get; set; }
        public int Trajanje { get; set; }
        public decimal Cijena { get; set; }
        public string? Prikaz { get; set; }
    }
}
