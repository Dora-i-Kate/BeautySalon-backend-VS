// BeautySalon.Application.Interfaces.IVrstaMaterijalaAppService.cs (Novo sučelje za VrstaMaterijala)
using BeautySalon.Application.DTOs.VrstaMaterijala;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    /// <summary>
    /// Sučelje za aplikacijski servis vrsta materijala.
    /// Definira operacije koje prezentacijski sloj može pozvati za upravljanje vrstama materijala.
    /// </summary>
    public interface IVrstaMaterijalaAppService
    {
        Task<VrstaMaterijalaDto> GetVrstaMaterijalaByIdAsync(int id);
        Task<IEnumerable<VrstaMaterijalaDto>> GetAllVrsteMaterijalaAsync();
        Task<VrstaMaterijalaDto> CreateVrstaMaterijalaAsync(CreateVrstaMaterijalaDto createDto);
        Task UpdateVrstaMaterijalaAsync(UpdateVrstaMaterijalaDto updateDto);
        Task DeleteVrstaMaterijalaAsync(int id);
        // Ako će VrsteMaterijala služiti i kao lookup, dodajte i to ovdje
        Task<IEnumerable<VrstaMaterijalaDto>> GetVrsteMaterijalaForLookupAsync();
    }
}