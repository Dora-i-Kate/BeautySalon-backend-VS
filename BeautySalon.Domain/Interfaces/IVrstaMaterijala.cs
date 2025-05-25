// BeautySalon.Domain.Interfaces.IVrstaMaterijalaRepository.cs
using BeautySalon.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Interfaces
{
    public interface IVrstaMaterijalaRepository
    {
        Task<VrstaMaterijala> GetByIdAsync(int id);
        Task<IEnumerable<VrstaMaterijala>> GetAllAsync();
        Task<VrstaMaterijala> AddAsync(VrstaMaterijala vrstaMaterijala);
        Task UpdateAsync(VrstaMaterijala vrstaMaterijala);
        Task DeleteAsync(int id);
        Task<bool> IsNazivUniqueAsync(string naziv, int? excludeId = null);
        Task<bool> VrstaMaterijalaExistsAsync(int id);
        Task<bool> HasRelatedMaterijaliAsync(int vrstaId); // OVDJE TREBA BITI!
    }
}