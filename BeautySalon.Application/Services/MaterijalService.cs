using BeautySalon.Application.DTOs;
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

        public async Task<List<MaterijalDto>> GetAllAsync()
        {
            var materijali = await _context.Materijal.Include(m => m.VrstaMaterijala).ToListAsync();

            return materijali.Select(m => new MaterijalDto
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv,
                Cijena = m.Cijena,
                MinimalnaKolicina = m.MinimalnaKolicina,
                TrenutnaKolicina = m.TrenutnaKolicina,
                JedinicaMjere = m.JedinicaMjere,
                VrstaId = m.VrstaId,
                VrstaNaziv = m.VrstaMaterijala?.Naziv
            }).ToList();
        }

        public async Task<MaterijalDto?> GetByIdAsync(int id)
        {
            var m = await _context.Materijal.Include(m => m.VrstaMaterijala)
                                            .FirstOrDefaultAsync(m => m.MaterijalId == id);
            if (m == null) return null;

            return new MaterijalDto
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv,
                Cijena = m.Cijena,
                MinimalnaKolicina = m.MinimalnaKolicina,
                TrenutnaKolicina = m.TrenutnaKolicina,
                JedinicaMjere = m.JedinicaMjere,
                VrstaId = m.VrstaId,
                VrstaNaziv = m.VrstaMaterijala?.Naziv
            };
        }

        public async Task CreateAsync(MaterijalDto dto)
        {
            if (dto.TrenutnaKolicina < dto.MinimalnaKolicina)
                throw new ArgumentException("Trenutna količina ne smije biti manja od minimalne.");

            var materijal = new Materijal
            {
                Naziv = dto.Naziv,
                Cijena = dto.Cijena,
                MinimalnaKolicina = dto.MinimalnaKolicina,
                TrenutnaKolicina = dto.TrenutnaKolicina,
                JedinicaMjere = dto.JedinicaMjere,
                VrstaId = dto.VrstaId
            };

            _context.Materijal.Add(materijal);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MaterijalDto dto)
        {
            if (dto.TrenutnaKolicina < dto.MinimalnaKolicina)
                throw new ArgumentException("Trenutna količina ne smije biti manja od minimalne.");

            var materijal = await _context.Materijal.FindAsync(dto.MaterijalId);
            if (materijal == null)
                throw new ArgumentException("Materijal ne postoji.");

            materijal.Naziv = dto.Naziv;
            materijal.Cijena = dto.Cijena;
            materijal.MinimalnaKolicina = dto.MinimalnaKolicina;
            materijal.TrenutnaKolicina = dto.TrenutnaKolicina;
            materijal.JedinicaMjere = dto.JedinicaMjere;
            materijal.VrstaId = dto.VrstaId;

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

        public async Task<List<VrstaMaterijalaDto>> GetAllVrsteMaterijalaAsync()
        {
            return await _context.VrstaMaterijala
                .Select(v => new VrstaMaterijalaDto
                {
                    VrstaId = v.VrstaId,
                    Naziv = v.Naziv
                })
                .ToListAsync();
        }
    }
}
