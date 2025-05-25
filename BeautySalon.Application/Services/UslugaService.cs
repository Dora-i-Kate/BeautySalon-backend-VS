using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySalon.Application.Interfaces;
using BeautySalon.DataAccess;
using BeautySalon.DataAccess.DbContexts;
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySalon.Application.Services
{
    public class UslugaService : IUslugaService
    {
        private readonly SalonDbContext _context;

        public UslugaService(SalonDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usluga>> GetAllAsync() => await _context.Usluge.ToListAsync();

        public async Task<Usluga?> GetByIdAsync(int id) => await _context.Usluge.FindAsync(id);

        public async Task AddAsync(Usluga usluga)
        {
            _context.Usluge.Add(usluga);
            await _context.SaveChangesAsync();
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
    }
}
