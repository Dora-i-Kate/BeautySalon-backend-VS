using BeautySalon.Application.DTOs.Termin;
using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    /// <summary>
    /// Sučelje za aplikacijski servis termina.
    /// Definira operacije koje prezentacijski sloj može pozvati za upravljanje terminima.
    /// </summary>
    public interface ITerminAppService
    {
        Task<TerminDto> GetTerminByIdAsync(int id);
        Task<IEnumerable<TerminDto>> GetAllTerminiAsync();
        Task<IEnumerable<TerminDto>> SearchTerminiAsync(DateTime? datumOd = null, DateTime? datumDo = null, int? zaposlenikId = null, TerminStatus? status = null);
        Task<TerminDto> CreateTerminAsync(CreateTerminDto createDto);
        Task UpdateTerminAsync(UpdateTerminDto updateDto);
        Task DeleteTerminAsync(int id);
    }
}
