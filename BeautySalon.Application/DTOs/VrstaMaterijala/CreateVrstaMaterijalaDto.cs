// BeautySalon.Application.DTOs.VrstaMaterijala.CreateVrstaMaterijalaDto.cs
using System.ComponentModel.DataAnnotations;

namespace BeautySalon.Application.DTOs.VrstaMaterijala
{
    /// <summary>
    /// DTO za stvaranje nove vrste materijala.
    /// </summary>
    public class CreateVrstaMaterijalaDto
    {
        [Required(ErrorMessage = "Naziv vrste materijala je obavezan.")]
        [StringLength(255, ErrorMessage = "Naziv vrste materijala ne smije biti duži od 255 znakova.")]
        public string Naziv { get; set; }
    }
}