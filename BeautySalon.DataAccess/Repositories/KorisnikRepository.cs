using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.DataAccess.Repositories
{
    /// <summary>
    /// Implementacija repozitorija za entitet Korisnik.
    /// Koristi Entity Framework Core za pristup podacima.
    /// </summary>
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly BeautySalonDbContext _context; // Koristi BeautySalonDbContext

        public KorisnikRepository(BeautySalonDbContext context)
        {
            _context = context;
        }

        public async Task<Korisnik> GetByIdAsync(int id)
        {
            // Učitava korisnika s njegovom ulogom
            return await _context.Korisnici
                                 .Include(k => k.Uloga)
                                 .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<IEnumerable<Korisnik>> GetAllAsync()
        {
            // Učitava sve korisnike s njihovim ulogama
            return await _context.Korisnici
                                 .Include(k => k.Uloga)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Korisnik>> GetByUlogaAsync(UlogaNaziv uloga)
        {
            // Učitava korisnike po određenoj ulozi
            return await _context.Korisnici
                                 .Include(k => k.Uloga)
                                 .Where(k => k.Uloga.NazivUloge == uloga.ToString())
                                 .ToListAsync();
        }

        public async Task<Korisnik> AddAsync(Korisnik korisnik)
        {
            _context.Korisnici.Add(korisnik);
            await _context.SaveChangesAsync();
            return korisnik;
        }

        public async Task UpdateAsync(Korisnik korisnik)
        {
            _context.Korisnici.Update(korisnik);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var korisnik = await _context.Korisnici.FindAsync(id);
            if (korisnik != null)
            {
                _context.Korisnici.Remove(korisnik);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            // Provjerava jedinstvenost emaila, isključujući korisnika s određenim ID-jem (kod ažuriranja)
            return !await _context.Korisnici
                                  .AnyAsync(k => k.Email == email && k.Id != excludeId);
        }
    }
}
