// BeautySalon.Domain.Interfaces.IMaterijalRepository.cs
// Uklonite liniju Task<bool> HasRelatedMaterijaliAsync(int vrstaId); ako postoji!

using BeautySalon.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Interfaces
{
    public interface IMaterijalRepository
    {
        Task<Materijal> GetByIdAsync(int id);
        Task<IEnumerable<Materijal>> GetAllAsync();
        Task<IEnumerable<Materijal>> SearchMaterijaliAsync(string searchTerm, int? vrstaId);
        Task<Materijal> AddAsync(Materijal materijal);
        Task UpdateAsync(Materijal materijal);
        Task DeleteAsync(int id);
        Task<bool> IsNazivUniqueAsync(string naziv, int? excludeId = null);
        Task<bool> MaterijalExistsAsync(int id);
        // Ovdje NE SMIJE BITI HasRelatedMaterijaliAsync(int vrstaId);
    }
}