// BeautySalon.Application.Services.MaterijalAppService.cs (Preimenovano i prilagođeno)
using BeautySalon.Application.DTOs.Materijal;
using BeautySalon.Application.DTOs.VrstaMaterijala; // Dodano za lookup
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
    /// Aplikacijski servis za upravljanje materijalima.
    /// Orkestrira pozive prema domenskom sloju i sloju za pristup podacima.
    /// </summary>
    public class MaterijalAppService : IMaterijalAppService
    {
        private readonly IMaterijalRepository _materijalRepository;
        private readonly IVrstaMaterijalaRepository _vrstaMaterijalaRepository; // Za dohvat naziva vrste
        private readonly MaterijalValidator _materijalValidator;

        public MaterijalAppService(IMaterijalRepository materijalRepository, IVrstaMaterijalaRepository vrstaMaterijalaRepository, MaterijalValidator materijalValidator)
        {
            _materijalRepository = materijalRepository ?? throw new ArgumentNullException(nameof(materijalRepository));
            _vrstaMaterijalaRepository = vrstaMaterijalaRepository ?? throw new ArgumentNullException(nameof(vrstaMaterijalaRepository));
            _materijalValidator = materijalValidator ?? throw new ArgumentNullException(nameof(materijalValidator));
        }

        public async Task<MaterijalDto> GetMaterijalByIdAsync(int id)
        {
            var materijal = await _materijalRepository.GetByIdAsync(id);
            if (materijal == null) return null;

            var vrstaMaterijala = await _vrstaMaterijalaRepository.GetByIdAsync(materijal.VrstaId); // Dohvat naziva vrste

            return new MaterijalDto
            {
                MaterijalId = materijal.MaterijalId,
                Naziv = materijal.Naziv,
                Cijena = materijal.Cijena,
                MinimalnaKolicina = materijal.MinimalnaKolicina,
                TrenutnaKolicina = materijal.TrenutnaKolicina,
                JedinicaMjere = materijal.JedinicaMjere,
                VrstaId = materijal.VrstaId,
                VrstaNaziv = vrstaMaterijala?.Naziv // Popunjavamo naziv za prikaz
            };
        }

        public async Task<IEnumerable<MaterijalDto>> GetAllMaterijaliAsync()
        {
            var materijali = await _materijalRepository.GetAllAsync();
            var vrsteMaterijala = (await _vrstaMaterijalaRepository.GetAllAsync()).ToDictionary(v => v.VrstaId, v => v.Naziv); // Cache naziva vrsta

            return materijali.Select(m => new MaterijalDto
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv,
                Cijena = m.Cijena,
                MinimalnaKolicina = m.MinimalnaKolicina,
                TrenutnaKolicina = m.TrenutnaKolicina,
                JedinicaMjere = m.JedinicaMjere,
                VrstaId = m.VrstaId,
                VrstaNaziv = vrsteMaterijala.GetValueOrDefault(m.VrstaId) // Popunjavamo naziv
            }).ToList();
        }

        public async Task<IEnumerable<MaterijalDto>> SearchMaterijaliAsync(string searchTerm, int? vrstaId)
        {
            var materijali = await _materijalRepository.SearchMaterijaliAsync(searchTerm, vrstaId);
            var vrsteMaterijala = (await _vrstaMaterijalaRepository.GetAllAsync()).ToDictionary(v => v.VrstaId, v => v.Naziv);

            return materijali.Select(m => new MaterijalDto
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv,
                Cijena = m.Cijena,
                MinimalnaKolicina = m.MinimalnaKolicina,
                TrenutnaKolicina = m.TrenutnaKolicina,
                JedinicaMjere = m.JedinicaMjere,
                VrstaId = m.VrstaId,
                VrstaNaziv = vrsteMaterijala.GetValueOrDefault(m.VrstaId)
            }).ToList();
        }

        public async Task<MaterijalDto> CreateMaterijalAsync(CreateMaterijalDto createDto)
        {
            var materijal = new Materijal(
                createDto.Naziv,
                createDto.Cijena,
                createDto.MinimalnaKolicina,
                createDto.TrenutnaKolicina,
                createDto.JedinicaMjere,
                createDto.VrstaId
            );

            // Validacija domenskog entiteta
            await _materijalValidator.ValidateMaterijalAsync(materijal, isNew: true);

            var createdMaterijal = await _materijalRepository.AddAsync(materijal);
            var vrstaMaterijala = await _vrstaMaterijalaRepository.GetByIdAsync(createdMaterijal.VrstaId);


            return new MaterijalDto
            {
                MaterijalId = createdMaterijal.MaterijalId,
                Naziv = createdMaterijal.Naziv,
                Cijena = createdMaterijal.Cijena,
                MinimalnaKolicina = createdMaterijal.MinimalnaKolicina,
                TrenutnaKolicina = createdMaterijal.TrenutnaKolicina,
                JedinicaMjere = createdMaterijal.JedinicaMjere,
                VrstaId = createdMaterijal.VrstaId,
                VrstaNaziv = vrstaMaterijala?.Naziv
            };
        }

        public async Task UpdateMaterijalAsync(UpdateMaterijalDto updateDto)
        {
            var materijal = await _materijalRepository.GetByIdAsync(updateDto.MaterijalId);
            if (materijal == null)
            {
                throw new KeyNotFoundException($"Materijal s ID-jem {updateDto.MaterijalId} nije pronađen.");
            }

            materijal.Update(
                updateDto.Naziv,
                updateDto.Cijena,
                updateDto.MinimalnaKolicina,
                updateDto.TrenutnaKolicina,
                updateDto.JedinicaMjere,
                updateDto.VrstaId
            );

            // Validacija domenskog entiteta
            await _materijalValidator.ValidateMaterijalAsync(materijal, isNew: false);

            await _materijalRepository.UpdateAsync(materijal);
        }

        public async Task DeleteMaterijalAsync(int id)
        {
            var materijalExists = await _materijalRepository.MaterijalExistsAsync(id);
            if (!materijalExists)
            {
                throw new KeyNotFoundException($"Materijal s ID-jem {id} nije pronađen.");
            }
            await _materijalRepository.DeleteAsync(id);
        }
    }
}