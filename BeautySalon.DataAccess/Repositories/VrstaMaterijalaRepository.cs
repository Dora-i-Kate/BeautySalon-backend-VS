// BeautySalon.DataAccess.Repositories.VrstaMaterijalaRepository.cs
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalon.DataAccess.Repositories
{
    public class VrstaMaterijalaRepository : IVrstaMaterijalaRepository
    {
        private readonly BeautySalonDbContext _context;

        public VrstaMaterijalaRepository(BeautySalonDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<VrstaMaterijala> GetByIdAsync(int id)
        {
            return await _context.VrsteMaterijala.FindAsync(id);
        }

        public async Task<IEnumerable<VrstaMaterijala>> GetAllAsync()
        {
            return await _context.VrsteMaterijala.OrderBy(v => v.Naziv).ToListAsync();
        }

        public async Task<VrstaMaterijala> AddAsync(VrstaMaterijala vrstaMaterijala)
        {
            _context.VrsteMaterijala.Add(vrstaMaterijala);
            await _context.SaveChangesAsync();
            return vrstaMaterijala;
        }

        public async Task UpdateAsync(VrstaMaterijala vrstaMaterijala)
        {
            _context.VrsteMaterijala.Attach(vrstaMaterijala);
            _context.Entry(vrstaMaterijala).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var vrstaMaterijala = await _context.VrsteMaterijala.FindAsync(id);
            if (vrstaMaterijala != null)
            {
                _context.VrsteMaterijala.Remove(vrstaMaterijala);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsNazivUniqueAsync(string naziv, int? excludeId = null)
        {
            return !await _context.VrsteMaterijala
                                  .AnyAsync(v => v.Naziv == naziv && v.VrstaId != excludeId);
        }

        public async Task<bool> VrstaMaterijalaExistsAsync(int id)
        {
            return await _context.VrsteMaterijala.AnyAsync(v => v.VrstaId == id);
        }

        // NOVO: Implementacija HasRelatedMaterijaliAsync u VrstaMaterijalaRepository
        public async Task<bool> HasRelatedMaterijaliAsync(int vrstaId)
        {
            // Provjerava postoje li materijali povezani s ovom vrstom materijala
            return await _context.Materijali.AnyAsync(m => m.VrstaId == vrstaId);
        }
    }
}