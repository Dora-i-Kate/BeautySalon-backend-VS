using BeautySalon.Application.DTOs.Korisnik;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    /// <summary>
    /// Sučelje za aplikacijski servis korisnika (za lookup liste).
    /// </summary>
    public interface IKorisnikAppService
    {
        Task<IEnumerable<KorisnikLookupDto>> GetKlijentiForLookupAsync();
        Task<IEnumerable<KorisnikLookupDto>> GetZaposleniciForLookupAsync();
    }
}
