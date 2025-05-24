using BeautySalon.Application.Interfaces;
using BeautySalon.DataAccess.DbContexts;
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Services
{
    public class MaterijalService : IMaterijalService
    {
        private readonly SalonDbContext _context;

        public MaterijalService(SalonDbContext context)
        {
            _context = context;
        }

        public async Task<List<Materijal>> GetAllAsync()
            => await _context.Materijal.Include(m => m.VrstaMaterijala).ToListAsync();

        public async Task<Materijal?> GetByIdAsync(int id)
            => await _context.Materijal.Include(m => m.VrstaMaterijala).FirstOrDefaultAsync(m => m.MaterijalId == id);

        public async Task CreateAsync(Materijal materijal)
        {
            if (materijal.TrenutnaKolicina < materijal.MinimalnaKolicina)
                throw new ArgumentException("Trenutna količina ne smije biti manja od minimalne.");

            _context.Materijal.Add(materijal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Materijal materijal)
        {
            if (materijal.TrenutnaKolicina < materijal.MinimalnaKolicina)
                throw new ArgumentException("Trenutna količina ne smije biti manja od minimalne.");

            _context.Materijal.Update(materijal);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Materijal.FindAsync(id);
            if (entity != null)
            {
                _context.Materijal.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
