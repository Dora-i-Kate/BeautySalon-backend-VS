using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Interfaces
{
    /// <summary>
    /// Sučelje repozitorija za entitet Uloga.
    /// Definira operacije pristupa podacima koje domena zahtijeva za uloge.
    /// </summary>
    public interface IUlogaRepository
    {
        Task<Uloga> GetByIdAsync(int id);
        Task<IEnumerable<Uloga>> GetAllAsync();
        Task<Uloga> AddAsync(Uloga uloga);
        Task UpdateAsync(Uloga uloga);
        Task DeleteAsync(int id);
        Task<Uloga> GetByNazivAsync(string nazivUloge);
    }
}
