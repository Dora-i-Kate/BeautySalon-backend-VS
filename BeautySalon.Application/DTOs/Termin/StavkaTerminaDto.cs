using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs.Termin
{
    /// <summary>
    /// DTO za prikaz detalja stavke termina.
    /// </summary>
    public class StavkaTerminaDto
    {
        public int Id { get; set; }
        public int UslugaId { get; set; }
        public string UslugaNaziv { get; set; } // Za prikaz u UI-ju
        public int Kolicina { get; set; }
        public decimal Cijena { get; set; }
    }
}
