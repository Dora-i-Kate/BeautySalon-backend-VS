using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs.Usluga
{
    /// <summary>
    /// DTO za prikaz detalja usluge.
    /// </summary>
    public class UslugaDto
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public decimal Cijena { get; set; }
        public int TrajanjeMinuta { get; set; }
    }
}
