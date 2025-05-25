using BeautySalon.Application.DTOs.Korisnik;
using BeautySalon.Application.Interfaces;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Services
{
    /// <summary>
    /// Aplikacijski servis za dohvaćanje korisnika za lookup liste.
    /// </summary>
    public class KorisnikAppService : IKorisnikAppService
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikAppService(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository ?? throw new ArgumentNullException(nameof(korisnikRepository));
        }

        public async Task<IEnumerable<KorisnikLookupDto>> GetKlijentiForLookupAsync()
        {
            var klijenti = await _korisnikRepository.GetByUlogaAsync(UlogaNaziv.Klijent);
            return klijenti.Select(k => new KorisnikLookupDto
            {
                Id = k.Id,
                ImePrezime = $"{k.Ime} {k.Prezime}"
            }).ToList();
        }

        public async Task<IEnumerable<KorisnikLookupDto>> GetZaposleniciForLookupAsync()
        {
            var zaposlenici = await _korisnikRepository.GetByUlogaAsync(UlogaNaziv.Zaposlenik);
            return zaposlenici.Select(k => new KorisnikLookupDto
            {
                Id = k.Id,
                ImePrezime = $"{k.Ime} {k.Prezime}"
            }).ToList();
        }
    }
}
