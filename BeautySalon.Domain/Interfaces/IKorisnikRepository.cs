using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Interfaces
{
    /// <summary>
    /// Sučelje repozitorija za entitet Korisnik.
    /// Definira operacije pristupa podacima koje domena zahtijeva za korisnike.
    /// </summary>
    public interface IKorisnikRepository
    {
        Task<Korisnik> GetByIdAsync(int id);
        Task<IEnumerable<Korisnik>> GetAllAsync();
        Task<IEnumerable<Korisnik>> GetByUlogaAsync(UlogaNaziv uloga);
        Task<Korisnik> AddAsync(Korisnik korisnik);
        Task UpdateAsync(Korisnik korisnik);
        Task DeleteAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    }
}
