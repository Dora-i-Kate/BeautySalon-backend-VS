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
    /// Implementacija repozitorija za entitet Uloga.
    /// Koristi Entity Framework Core za pristup podacima.
    /// </summary>
    public class UlogaRepository : IUlogaRepository
    {
        private readonly BeautySalonDbContext _context; // Koristi BeautySalonDbContext

        public UlogaRepository(BeautySalonDbContext context)
        {
            _context = context;
        }

        public async Task<Uloga> GetByIdAsync(int id)
        {
            return await _context.Uloge.FindAsync(id);
        }

        public async Task<IEnumerable<Uloga>> GetAllAsync()
        {
            return await _context.Uloge.ToListAsync();
        }

        public async Task<Uloga> AddAsync(Uloga uloga)
        {
            _context.Uloge.Add(uloga);
            await _context.SaveChangesAsync();
            return uloga;
        }

        public async Task UpdateAsync(Uloga uloga)
        {
            _context.Uloge.Update(uloga);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var uloga = await _context.Uloge.FindAsync(id);
            if (uloga != null)
            {
                _context.Uloge.Remove(uloga);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Uloga> GetByNazivAsync(string nazivUloge)
        {
            return await _context.Uloge.FirstOrDefaultAsync(u => u.NazivUloge == nazivUloge);
        }
    }
}
