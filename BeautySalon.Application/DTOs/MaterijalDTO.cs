using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs
{
    public class MaterijalDto
    {
        public int MaterijalId { get; set; }
        public required string Naziv { get; set; }
        public decimal Cijena { get; set; }
        public int MinimalnaKolicina { get; set; }
        public int TrenutnaKolicina { get; set; }
        public required string JedinicaMjere { get; set; }
        public int VrstaId { get; set; }
        public string? VrstaNaziv { get; set; }
    }
}
