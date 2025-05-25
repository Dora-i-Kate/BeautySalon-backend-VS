// BeautySalon.Application.DTOs.Materijal.CreateMaterijalDto.cs
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Application.DTOs.Materijal
{
    /// <summary>
    /// DTO za stvaranje novog materijala.
    /// </summary>
    public class CreateMaterijalDto
    {
        [Required(ErrorMessage = "Naziv materijala je obavezan.")]
        [StringLength(255, ErrorMessage = "Naziv materijala ne smije biti duži od 255 znakova.")]
        public string Naziv { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena materijala mora biti veća od nule.")]
        public decimal Cijena { get; set; }

        [Required(ErrorMessage = "Minimalna količina je obavezna.")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimalna količina ne smije biti negativna.")]
        public int MinimalnaKolicina { get; set; }

        [Required(ErrorMessage = "Trenutna količina je obavezna.")]
        [Range(0, int.MaxValue, ErrorMessage = "Trenutna količina ne smije biti negativna.")]
        public int TrenutnaKolicina { get; set; }

        [Required(ErrorMessage = "Jedinica mjere je obavezna.")]
        [StringLength(50, ErrorMessage = "Jedinica mjere ne smije biti duža od 50 znakova.")]
        public string JedinicaMjere { get; set; }

        [Required(ErrorMessage = "Vrsta materijala je obavezna.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite vrstu materijala.")]
        public int VrstaId { get; set; }
    }
}