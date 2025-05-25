using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    /// <summary>
    /// Enumeracija statusa termina.
    /// </summary>
    public enum TerminStatus
    {
        Zakazan = 1,
        Završen = 2,
        Otkazan = 3,
        U_Tijeku = 4 // Dodano za potpunost
    }
}
