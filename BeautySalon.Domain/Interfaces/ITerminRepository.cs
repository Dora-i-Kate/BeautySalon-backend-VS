using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Interfaces
{
    /// <summary>
    /// Sučelje repozitorija za entitet Termin.
    /// Definira operacije pristupa podacima koje domena zahtijeva za termine.
    /// </summary>
    public interface ITerminRepository
    {
        Task<Termin> GetByIdAsync(int id);
        Task<IEnumerable<Termin>> GetAllAsync();
        Task<IEnumerable<Termin>> SearchTerminiAsync(DateTime? datumOd = null, DateTime? datumDo = null, int? zaposlenikId = null, TerminStatus? status = null);
        Task<Termin> AddAsync(Termin termin);
        Task UpdateAsync(Termin termin);
        Task DeleteAsync(int id);
        Task<bool> HasOverlappingTerminAsync(int zaposlenikId, DateTime datum, TimeSpan vrijeme, int trajanjeMinuta, int? excludeTerminId = null);
        Task<bool> TerminExistsAsync(int id);
    }
}
