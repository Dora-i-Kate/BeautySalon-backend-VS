using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs
{
    public class UslugaDto
    {
        public int UslugaId { get; set; }
        public required string Naziv { get; set; }
        public string? Opis { get; set; }
        public int Trajanje { get; set; } // u minutama
        public decimal Cijena { get; set; }
    }
}
