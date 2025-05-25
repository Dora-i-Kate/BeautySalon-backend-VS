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
    /// Implementacija repozitorija za entitet Usluga.
    /// Koristi Entity Framework Core za pristup podacima.
    /// </summary>
    public class UslugaRepository : IUslugaRepository
    {
        private readonly BeautySalonDbContext _context; // Koristi BeautySalonDbContext

        public UslugaRepository(BeautySalonDbContext context)
        {
            _context = context;
        }

        public async Task<Usluga> GetByIdAsync(int id)
        {
            return await _context.Usluge.FindAsync(id);
        }

        public async Task<IEnumerable<Usluga>> GetAllAsync()
        {
            return await _context.Usluge.OrderBy(u => u.Naziv).ToListAsync();
        }

        public async Task<IEnumerable<Usluga>> SearchUslugeAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            return await _context.Usluge
                                 .Where(u => u.Naziv.Contains(searchTerm) || u.Opis.Contains(searchTerm))
                                 .OrderBy(u => u.Naziv)
                                 .ToListAsync();
        }

        public async Task<Usluga> AddAsync(Usluga usluga)
        {
            _context.Usluge.Add(usluga);
            await _context.SaveChangesAsync();
            return usluga;
        }

        public async Task UpdateAsync(Usluga usluga)
        {
            _context.Usluge.Update(usluga);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usluga = await _context.Usluge.FindAsync(id);
            if (usluga != null)
            {
                _context.Usluge.Remove(usluga);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsNazivUniqueAsync(string naziv, int? excludeId = null)
        {
            // Provjerava jedinstvenost naziva usluge, isključujući uslugu s određenim ID-jem (kod ažuriranja)
            return !await _context.Usluge
                                  .AnyAsync(u => u.Naziv == naziv && u.Id != excludeId);
        }

        public async Task<bool> UslugaExistsAsync(int id)
        {
            return await _context.Usluge.AnyAsync(u => u.Id == id);
        }
    }
}
