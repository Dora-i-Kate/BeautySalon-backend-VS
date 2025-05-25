using BeautySalon.Application.DTOs.Usluga;
using BeautySalon.Application.Interfaces;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using BeautySalon.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Services
{
    /// <summary>
    /// Aplikacijski servis za upravljanje uslugama.
    /// Orkestrira pozive prema domenskom sloju i sloju za pristup podacima.
    /// </summary>
    public class UslugaAppService : IUslugaAppService
    {
        private readonly IUslugaRepository _uslugaRepository;
        private readonly UslugaValidator _uslugaValidator;

        public UslugaAppService(IUslugaRepository uslugaRepository, UslugaValidator uslugaValidator)
        {
            _uslugaRepository = uslugaRepository ?? throw new ArgumentNullException(nameof(uslugaRepository));
            _uslugaValidator = uslugaValidator ?? throw new ArgumentNullException(nameof(uslugaValidator));
        }

        public async Task<UslugaDto> GetUslugaByIdAsync(int id)
        {
            var usluga = await _uslugaRepository.GetByIdAsync(id);
            if (usluga == null) return null;

            return new UslugaDto
            {
                Id = usluga.Id,
                Naziv = usluga.Naziv,
                Opis = usluga.Opis,
                Cijena = usluga.Cijena,
                TrajanjeMinuta = usluga.TrajanjeMinuta
            };
        }

        public async Task<IEnumerable<UslugaDto>> GetAllUslugeAsync()
        {
            var usluge = await _uslugaRepository.GetAllAsync();
            return usluge.Select(u => new UslugaDto
            {
                Id = u.Id,
                Naziv = u.Naziv,
                Opis = u.Opis,
                Cijena = u.Cijena,
                TrajanjeMinuta = u.TrajanjeMinuta
            }).ToList();
        }

        public async Task<IEnumerable<UslugaDto>> SearchUslugeAsync(string searchTerm)
        {
            var usluge = await _uslugaRepository.SearchUslugeAsync(searchTerm);
            return usluge.Select(u => new UslugaDto
            {
                Id = u.Id,
                Naziv = u.Naziv,
                Opis = u.Opis,
                Cijena = u.Cijena,
                TrajanjeMinuta = u.TrajanjeMinuta
            }).ToList();
        }

        public async Task<UslugaDto> CreateUslugaAsync(CreateUslugaDto createDto)
        {
            var usluga = new Usluga(createDto.Naziv, createDto.Opis, createDto.Cijena, createDto.TrajanjeMinuta);

            // Validacija domenskog entiteta
            await _uslugaValidator.ValidateUslugaAsync(usluga, isNew: true);

            var createdUsluga = await _uslugaRepository.AddAsync(usluga);

            return new UslugaDto
            {
                Id = createdUsluga.Id,
                Naziv = createdUsluga.Naziv,
                Opis = createdUsluga.Opis,
                Cijena = createdUsluga.Cijena,
                TrajanjeMinuta = createdUsluga.TrajanjeMinuta
            };
        }

        public async Task UpdateUslugaAsync(UpdateUslugaDto updateDto)
        {
            var usluga = await _uslugaRepository.GetByIdAsync(updateDto.Id);
            if (usluga == null)
            {
                throw new KeyNotFoundException($"Usluga s ID-jem {updateDto.Id} nije pronađena.");
            }

            usluga.Update(updateDto.Naziv, updateDto.Opis, updateDto.Cijena, updateDto.TrajanjeMinuta);

            // Validacija domenskog entiteta
            await _uslugaValidator.ValidateUslugaAsync(usluga, isNew: false);

            await _uslugaRepository.UpdateAsync(usluga);
        }

        public async Task DeleteUslugaAsync(int id)
        {
            var uslugaExists = await _uslugaRepository.UslugaExistsAsync(id);
            if (!uslugaExists)
            {
                throw new KeyNotFoundException($"Usluga s ID-jem {id} nije pronađena.");
            }
            await _uslugaRepository.DeleteAsync(id);
        }
    }
}
