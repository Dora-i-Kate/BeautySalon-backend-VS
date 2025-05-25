using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs.Usluga
{
    /// <summary>
    /// DTO za ažuriranje postojeće usluge.
    /// </summary>
    public class UpdateUslugaDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv usluge je obavezan.")]
        [StringLength(255, ErrorMessage = "Naziv usluge ne smije biti duži od 255 znakova.")]
        public string Naziv { get; set; }

        [StringLength(1000, ErrorMessage = "Opis usluge ne smije biti duži od 1000 znakova.")]
        public string Opis { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti veća od nule.")]
        public decimal Cijena { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno.")]
        [Range(15, 240, ErrorMessage = "Trajanje mora biti između 15 i 240 minuta.")]
        public int TrajanjeMinuta { get; set; }
    }
}
