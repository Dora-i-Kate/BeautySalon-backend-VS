using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.DTOs.Termin
{
    /// <summary>
    /// DTO za prikaz detalja termina.
    /// </summary>
    public class TerminDto
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public TimeSpan Vrijeme { get; set; }
        public int TrajanjeMinuta { get; set; }
        public TerminStatus Status { get; set; }

        public int KlijentId { get; set; }
        public string KlijentImePrezime { get; set; } // Za prikaz u UI-ju

        public int ZaposlenikId { get; set; }
        public string ZaposlenikImePrezime { get; set; } // Za prikaz u UI-ju

        public List<StavkaTerminaDto> StavkeTermina { get; set; } = new List<StavkaTerminaDto>();
        public decimal UkupnaCijena { get; set; }
    }
}
