// BeautySalon.Application.DTOs.Materijal.MaterijalDto.cs
namespace BeautySalon.Application.DTOs.Materijal
{
    /// <summary>
    /// DTO za prikaz detalja materijala.
    /// </summary>
    public class MaterijalDto
    {
        public int MaterijalId { get; set; }
        public string Naziv { get; set; }
        public decimal Cijena { get; set; }
        public int MinimalnaKolicina { get; set; }
        public int TrenutnaKolicina { get; set; }
        public string JedinicaMjere { get; set; }
        public int VrstaId { get; set; }
        public string? VrstaNaziv { get; set; } // Za prikaz u UI-ju (navigacijsko svojstvo)
    }
}