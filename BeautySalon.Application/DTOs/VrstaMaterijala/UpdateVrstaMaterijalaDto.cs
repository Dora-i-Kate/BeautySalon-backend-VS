// BeautySalon.Application.DTOs.VrstaMaterijala.UpdateVrstaMaterijalaDto.cs
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Application.DTOs.VrstaMaterijala
{
    /// <summary>
    /// DTO za ažuriranje postojeće vrste materijala.
    /// </summary>
    public class UpdateVrstaMaterijalaDto
    {
        [Required]
        public int VrstaId { get; set; }

        [Required(ErrorMessage = "Naziv vrste materijala je obavezan.")]
        [StringLength(255, ErrorMessage = "Naziv vrste materijala ne smije biti duži od 255 znakova.")]
        public string Naziv { get; set; }
    }
}