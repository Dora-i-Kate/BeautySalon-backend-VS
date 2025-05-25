// BeautySalon.Application.Interfaces.IMaterijalAppService.cs (Preimenovano i prilagođeni parametri)
using BeautySalon.Application.DTOs.Materijal;
using BeautySalon.Application.DTOs.VrstaMaterijala; // Dodano za GetAllVrsteMaterijalaForLookupAsync
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    /// <summary>
    /// Sučelje za aplikacijski servis materijala.
    /// Definira operacije koje prezentacijski sloj može pozvati za upravljanje materijalima.
    /// </summary>
    public interface IMaterijalAppService
    {
        Task<MaterijalDto> GetMaterijalByIdAsync(int id);
        Task<IEnumerable<MaterijalDto>> GetAllMaterijaliAsync();
        Task<IEnumerable<MaterijalDto>> SearchMaterijaliAsync(string searchTerm, int? vrstaId); // Dodana search metoda
        Task<MaterijalDto> CreateMaterijalAsync(CreateMaterijalDto createDto); // Koristi CreateMaterijalDto
        Task UpdateMaterijalAsync(UpdateMaterijalDto updateDto); // Koristi UpdateMaterijalDto
        Task DeleteMaterijalAsync(int id);
    }
}