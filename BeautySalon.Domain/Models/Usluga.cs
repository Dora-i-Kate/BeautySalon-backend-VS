namespace BeautySalon.Domain.Models
{
    public class Usluga
    {
        public int UslugaId { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Opis { get; set; }
        public int Trajanje { get; set; } // u minutama
        public decimal Cijena { get; set; }
        public bool Prikaz { get; set; }
    }
}
