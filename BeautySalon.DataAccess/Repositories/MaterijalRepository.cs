// BeautySalon.DataAccess.Repositories.MaterijalRepository.cs
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalon.DataAccess.Repositories
{
    /// <summary>
    /// Implementacija repozitorija za entitet Materijal.
    /// Koristi Entity Framework Core za pristup podacima.
    /// </summary>
    public class MaterijalRepository : IMaterijalRepository
    {
        private readonly BeautySalonDbContext _context;

        public MaterijalRepository(BeautySalonDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Materijal> GetByIdAsync(int id)
        {
            // Učitava materijal s njegovom vrstom
            return await _context.Materijali
                                 .Include(m => m.VrstaMaterijala)
                                 .FirstOrDefaultAsync(m => m.MaterijalId == id);
        }

        public async Task<IEnumerable<Materijal>> GetAllAsync()
        {
            // Učitava sve materijale s njihovim vrstama
            return await _context.Materijali
                                 .Include(m => m.VrstaMaterijala)
                                 .OrderBy(m => m.Naziv)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Materijal>> SearchMaterijaliAsync(string searchTerm, int? vrstaId)
        {
            IQueryable<Materijal> query = _context.Materijali.Include(m => m.VrstaMaterijala);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m => m.Naziv.Contains(searchTerm) || m.JedinicaMjere.Contains(searchTerm));
            }

            if (vrstaId.HasValue && vrstaId.Value > 0)
            {
                query = query.Where(m => m.VrstaId == vrstaId.Value);
            }

            return await query.OrderBy(m => m.Naziv).ToListAsync();
        }

        public async Task<Materijal> AddAsync(Materijal materijal)
        {
            _context.Materijali.Add(materijal);
            await _context.SaveChangesAsync();
            return materijal;
        }

        public async Task UpdateAsync(Materijal materijal)
        {
            // Attach the entity if it's not already tracked (common scenario when entity comes from DTO/API)
            _context.Materijali.Attach(materijal);
            _context.Entry(materijal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var materijal = await _context.Materijali.FindAsync(id);
            if (materijal != null)
            {
                _context.Materijali.Remove(materijal);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsNazivUniqueAsync(string naziv, int? excludeId = null)
        {
            return !await _context.Materijali
                                  .AnyAsync(m => m.Naziv == naziv && m.MaterijalId != excludeId);
        }

        public async Task<bool> MaterijalExistsAsync(int id)
        {
            return await _context.Materijali.AnyAsync(m => m.MaterijalId == id);
        }
    }
}