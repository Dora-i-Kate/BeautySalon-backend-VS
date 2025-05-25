using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySalon.Application.DTOs;
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

        public async Task<List<UslugaDto>> GetAllAsync()
        {
            var usluge = await _context.Usluga.ToListAsync();

            return usluge.Select(u => new UslugaDto
            {
                UslugaId = u.UslugaId,
                Naziv = u.Naziv,
                Opis = u.Opis,
                Trajanje = u.Trajanje,
                Cijena = u.Cijena
            }).ToList();
        }

        public async Task<UslugaDto?> GetByIdAsync(int id)
        {
            var u = await _context.Usluga.FindAsync(id);
            if (u == null) return null;

            return new UslugaDto
            {
                UslugaId = u.UslugaId,
                Naziv = u.Naziv,
                Opis = u.Opis,
                Trajanje = u.Trajanje,
                Cijena = u.Cijena
            };
        }

        public async Task AddAsync(UslugaDto dto)
        {
            var usluga = new Usluga
            {
                Naziv = dto.Naziv,
                Opis = dto.Opis,
                Trajanje = dto.Trajanje,
                Cijena = dto.Cijena
            };

            _context.Usluga.Add(usluga);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UslugaDto dto)
        {
            var usluga = await _context.Usluga.FindAsync(dto.UslugaId);
            if (usluga == null)
                throw new ArgumentException("Usluga ne postoji.");

            usluga.Naziv = dto.Naziv;
            usluga.Opis = dto.Opis;
            usluga.Trajanje = dto.Trajanje;
            usluga.Cijena = dto.Cijena;

            _context.Usluga.Update(usluga);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usluga = await _context.Usluga.FindAsync(id);
            if (usluga != null)
            {
                _context.Usluga.Remove(usluga);
                await _context.SaveChangesAsync();
            }
        }
    }
}
