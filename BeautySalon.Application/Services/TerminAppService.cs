using BeautySalon.Application.DTOs.Termin;
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
    /// Aplikacijski servis za upravljanje terminima.
    /// Orkestrira pozive prema domenskom sloju i sloju za pristup podacima.
    /// </summary>
    public class TerminAppService : ITerminAppService
    {
        private readonly ITerminRepository _terminRepository;
        private readonly IUslugaRepository _uslugaRepository; // Potrebno za mapiranje StavkaTerminaDto
        private readonly IKorisnikRepository _korisnikRepository; // Potrebno za mapiranje Klijent/Zaposlenik
        private readonly TerminValidator _terminValidator;

        public TerminAppService(ITerminRepository terminRepository, IUslugaRepository uslugaRepository, IKorisnikRepository korisnikRepository, TerminValidator terminValidator)
        {
            _terminRepository = terminRepository ?? throw new ArgumentNullException(nameof(terminRepository));
            _uslugaRepository = uslugaRepository ?? throw new ArgumentNullException(nameof(uslugaRepository));
            _korisnikRepository = korisnikRepository ?? throw new ArgumentNullException(nameof(korisnikRepository));
            _terminValidator = terminValidator ?? throw new ArgumentNullException(nameof(terminValidator));
        }

        public async Task<TerminDto> GetTerminByIdAsync(int id)
        {
            var termin = await _terminRepository.GetByIdAsync(id);
            if (termin == null) return null;

            // Mapiranje entiteta u DTO
            var terminDto = new TerminDto
            {
                Id = termin.Id,
                Datum = termin.Datum,
                Vrijeme = termin.Vrijeme,
                TrajanjeMinuta = termin.TrajanjeMinuta,
                Status = termin.Status,
                KlijentId = termin.KlijentId,
                KlijentImePrezime = $"{termin.Klijent?.Ime} {termin.Klijent?.Prezime}",
                ZaposlenikId = termin.ZaposlenikId,
                ZaposlenikImePrezime = $"{termin.Zaposlenik?.Ime} {termin.Zaposlenik?.Prezime}",
                UkupnaCijena = termin.CalculateUkupnaCijena(),
                StavkeTermina = termin.StavkeTermina.Select(st => new StavkaTerminaDto
                {
                    Id = st.Id,
                    UslugaId = st.UslugaId,
                    UslugaNaziv = st.Usluga?.Naziv,
                    Kolicina = st.Kolicina,
                    Cijena = st.Cijena
                }).ToList()
            };
            return terminDto;
        }

        public async Task<IEnumerable<TerminDto>> GetAllTerminiAsync()
        {
            var termini = await _terminRepository.GetAllAsync();
            var terminDtos = new List<TerminDto>();
            foreach (var termin in termini)
            {
                terminDtos.Add(new TerminDto
                {
                    Id = termin.Id,
                    Datum = termin.Datum,
                    Vrijeme = termin.Vrijeme,
                    TrajanjeMinuta = termin.TrajanjeMinuta,
                    Status = termin.Status,
                    KlijentId = termin.KlijentId,
                    KlijentImePrezime = $"{termin.Klijent?.Ime} {termin.Klijent?.Prezime}",
                    ZaposlenikId = termin.ZaposlenikId,
                    ZaposlenikImePrezime = $"{termin.Zaposlenik?.Ime} {termin.Zaposlenik?.Prezime}",
                    UkupnaCijena = termin.CalculateUkupnaCijena()
                    // StavkeTermina se ovdje ne učitavaju u GetAll, samo u GetById
                });
            }
            return terminDtos;
        }

        public async Task<IEnumerable<TerminDto>> SearchTerminiAsync(DateTime? datumOd = null, DateTime? datumDo = null, int? zaposlenikId = null, TerminStatus? status = null)
        {
            var termini = await _terminRepository.SearchTerminiAsync(datumOd, datumDo, zaposlenikId, status);
            var terminDtos = new List<TerminDto>();
            foreach (var termin in termini)
            {
                terminDtos.Add(new TerminDto
                {
                    Id = termin.Id,
                    Datum = termin.Datum,
                    Vrijeme = termin.Vrijeme,
                    TrajanjeMinuta = termin.TrajanjeMinuta,
                    Status = termin.Status,
                    KlijentId = termin.KlijentId,
                    KlijentImePrezime = $"{termin.Klijent?.Ime} {termin.Klijent?.Prezime}",
                    ZaposlenikId = termin.ZaposlenikId,
                    ZaposlenikImePrezime = $"{termin.Zaposlenik?.Ime} {termin.Zaposlenik?.Prezime}",
                    UkupnaCijena = termin.CalculateUkupnaCijena()
                });
            }
            return terminDtos;
        }

        public async Task<TerminDto> CreateTerminAsync(CreateTerminDto createDto)
        {
            // Stvaranje domenskog entiteta
            var termin = new Termin(createDto.Datum, createDto.Vrijeme, createDto.TrajanjeMinuta, createDto.KlijentId, createDto.ZaposlenikId);

            // Dodavanje stavki termina
            foreach (var stavkaDto in createDto.StavkeTermina)
            {
                termin.AddStavka(stavkaDto.UslugaId, stavkaDto.Kolicina, stavkaDto.Cijena);
            }

            // Validacija domenskog entiteta
            await _terminValidator.ValidateTerminAsync(termin, isNew: true);

            // Spremanje u bazu
            var createdTermin = await _terminRepository.AddAsync(termin);

            // Dohvaćanje kompletnog termina za DTO (s navigacijskim svojstvima)
            var fullTermin = await _terminRepository.GetByIdAsync(createdTermin.Id);

            return new TerminDto
            {
                Id = fullTermin.Id,
                Datum = fullTermin.Datum,
                Vrijeme = fullTermin.Vrijeme,
                TrajanjeMinuta = fullTermin.TrajanjeMinuta,
                Status = fullTermin.Status,
                KlijentId = fullTermin.KlijentId,
                KlijentImePrezime = $"{fullTermin.Klijent?.Ime} {fullTermin.Klijent?.Prezime}",
                ZaposlenikId = fullTermin.ZaposlenikId,
                ZaposlenikImePrezime = $"{fullTermin.Zaposlenik?.Ime} {fullTermin.Zaposlenik?.Prezime}",
                UkupnaCijena = fullTermin.CalculateUkupnaCijena(),
                StavkeTermina = fullTermin.StavkeTermina.Select(st => new StavkaTerminaDto
                {
                    Id = st.Id,
                    UslugaId = st.UslugaId,
                    UslugaNaziv = st.Usluga?.Naziv,
                    Kolicina = st.Kolicina,
                    Cijena = st.Cijena
                }).ToList()
            };
        }

        public async Task UpdateTerminAsync(UpdateTerminDto updateDto)
        {
            var termin = await _terminRepository.GetByIdAsync(updateDto.Id);
            if (termin == null)
            {
                throw new KeyNotFoundException($"Termin s ID-jem {updateDto.Id} nije pronađen.");
            }

            // Ažuriranje domenskog entiteta
            termin.Update(updateDto.Datum, updateDto.Vrijeme, updateDto.TrajanjeMinuta, updateDto.Status, updateDto.KlijentId, updateDto.ZaposlenikId);

            // Ažuriranje stavki termina
            // Prvo ukloni sve koje nisu u novoj listi
            var stavkeToRemove = termin.StavkeTermina
                                       .Where(existingStavka => !updateDto.StavkeTermina.Any(s => s.Id == existingStavka.Id))
                                       .ToList();
            foreach (var stavka in stavkeToRemove)
            {
                termin.RemoveStavka(stavka.Id);
            }

            // Zatim dodaj nove ili ažuriraj postojeće
            foreach (var stavkaDto in updateDto.StavkeTermina)
            {
                if (stavkaDto.Id == 0) // Nova stavka
                {
                    termin.AddStavka(stavkaDto.UslugaId, stavkaDto.Kolicina, stavkaDto.Cijena);
                }
                else // Postojeća stavka
                {
                    termin.UpdateStavka(stavkaDto.Id, stavkaDto.UslugaId, stavkaDto.Kolicina, stavkaDto.Cijena);
                }
            }

            // Validacija domenskog entiteta
            await _terminValidator.ValidateTerminAsync(termin, isNew: false);

            // Spremanje ažuriranog entiteta
            await _terminRepository.UpdateAsync(termin);
        }

        public async Task DeleteTerminAsync(int id)
        {
            var terminExists = await _terminRepository.TerminExistsAsync(id);
            if (!terminExists)
            {
                throw new KeyNotFoundException($"Termin s ID-jem {id} nije pronađen.");
            }
            await _terminRepository.DeleteAsync(id);
        }
    }
}
