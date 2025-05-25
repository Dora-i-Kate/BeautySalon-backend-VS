using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Interfaces
{
    /// <summary>
    /// Sučelje repozitorija za entitet Usluga.
    /// Definira operacije pristupa podacima koje domena zahtijeva za usluge.
    /// </summary>
    public interface IUslugaRepository
    {
        Task<Usluga> GetByIdAsync(int id);
        Task<IEnumerable<Usluga>> GetAllAsync();
        Task<IEnumerable<Usluga>> SearchUslugeAsync(string searchTerm);
        Task<Usluga> AddAsync(Usluga usluga);
        Task UpdateAsync(Usluga usluga);
        Task DeleteAsync(int id);
        Task<bool> IsNazivUniqueAsync(string naziv, int? excludeId = null);
        Task<bool> UslugaExistsAsync(int id);
    }
}
