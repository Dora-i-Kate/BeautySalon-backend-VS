// BeautySalon.Application.Services.VrstaMaterijalaAppService.cs (Novo implementirano sučelje)
using BeautySalon.Application.DTOs.VrstaMaterijala;
using BeautySalon.Application.Interfaces;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using BeautySalon.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeautySalon.Application.Services
{
    /// <summary>
    /// Aplikacijski servis za upravljanje vrstama materijala.
    /// Orkestrira pozive prema domenskom sloju i sloju za pristup podacima.
    /// </summary>
    public class VrstaMaterijalaAppService : IVrstaMaterijalaAppService
    {
        private readonly IVrstaMaterijalaRepository _vrstaMaterijalaRepository;
        private readonly VrstaMaterijalaValidator _vrstaMaterijalaValidator;

        public VrstaMaterijalaAppService(IVrstaMaterijalaRepository vrstaMaterijalaRepository, VrstaMaterijalaValidator vrstaMaterijalaValidator)
        {
            _vrstaMaterijalaRepository = vrstaMaterijalaRepository ?? throw new ArgumentNullException(nameof(vrstaMaterijalaRepository));
            _vrstaMaterijalaValidator = vrstaMaterijalaValidator ?? throw new ArgumentNullException(nameof(vrstaMaterijalaValidator));
        }

        public async Task<VrstaMaterijalaDto> GetVrstaMaterijalaByIdAsync(int id)
        {
            var vrsta = await _vrstaMaterijalaRepository.GetByIdAsync(id);
            if (vrsta == null) return null;

            return new VrstaMaterijalaDto
            {
                VrstaId = vrsta.VrstaId,
                Naziv = vrsta.Naziv
            };
        }

        public async Task<IEnumerable<VrstaMaterijalaDto>> GetAllVrsteMaterijalaAsync()
        {
            var vrste = await _vrstaMaterijalaRepository.GetAllAsync();
            return vrste.Select(v => new VrstaMaterijalaDto
            {
                VrstaId = v.VrstaId,
                Naziv = v.Naziv
            }).ToList();
        }

        public async Task<VrstaMaterijalaDto> CreateVrstaMaterijalaAsync(CreateVrstaMaterijalaDto createDto)
        {
            var vrsta = new VrstaMaterijala(createDto.Naziv);

            // Validacija domenskog entiteta
            await _vrstaMaterijalaValidator.ValidateVrstaMaterijalaAsync(vrsta, isNew: true);

            var createdVrsta = await _vrstaMaterijalaRepository.AddAsync(vrsta);

            return new VrstaMaterijalaDto
            {
                VrstaId = createdVrsta.VrstaId,
                Naziv = createdVrsta.Naziv
            };
        }

        public async Task UpdateVrstaMaterijalaAsync(UpdateVrstaMaterijalaDto updateDto)
        {
            var vrsta = await _vrstaMaterijalaRepository.GetByIdAsync(updateDto.VrstaId);
            if (vrsta == null)
            {
                throw new KeyNotFoundException($"Vrsta materijala s ID-jem {updateDto.VrstaId} nije pronađena.");
            }

            vrsta.Update(updateDto.Naziv);

            // Validacija domenskog entiteta
            await _vrstaMaterijalaValidator.ValidateVrstaMaterijalaAsync(vrsta, isNew: false);

            await _vrstaMaterijalaRepository.UpdateAsync(vrsta);
        }

        public async Task DeleteVrstaMaterijalaAsync(int id)
        {
            var vrstaExists = await _vrstaMaterijalaRepository.VrstaMaterijalaExistsAsync(id);
            if (!vrstaExists)
            {
                throw new KeyNotFoundException($"Vrsta materijala s ID-jem {id} nije pronađena.");
            }

            // Dodatna provjera poslovnog pravila prije brisanja (preko domenskog servisa)
            await _vrstaMaterijalaValidator.ValidateDeleteVrstaMaterijalaAsync(id);

            await _vrstaMaterijalaRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<VrstaMaterijalaDto>> GetVrsteMaterijalaForLookupAsync()
        {
            var vrste = await _vrstaMaterijalaRepository.GetAllAsync();
            return vrste.Select(v => new VrstaMaterijalaDto
            {
                VrstaId = v.VrstaId,
                Naziv = v.Naziv
            }).ToList();
        }
    }
}