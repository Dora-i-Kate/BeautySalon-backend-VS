using BeautySalon.Application.DTOs.Usluga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    /// <summary>
    /// Sučelje za aplikacijski servis usluga.
    /// Definira operacije koje prezentacijski sloj može pozvati za upravljanje uslugama.
    /// </summary>
    public interface IUslugaAppService
    {
        Task<UslugaDto> GetUslugaByIdAsync(int id);
        Task<IEnumerable<UslugaDto>> GetAllUslugeAsync();
        Task<IEnumerable<UslugaDto>> SearchUslugeAsync(string searchTerm);
        Task<UslugaDto> CreateUslugaAsync(CreateUslugaDto createDto);
        Task UpdateUslugaAsync(UpdateUslugaDto updateDto);
        Task DeleteUslugaAsync(int id);
    }
}
