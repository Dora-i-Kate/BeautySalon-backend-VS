// BeautySalon.Domain.Services.VrstaMaterijalaValidator.cs
using BeautySalon.Domain.Exceptions;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Services
{
    /// <summary>
    /// Domenski servis za validaciju vrste materijala.
    /// Sadrži složena poslovna pravila koja zahtijevaju pristup repozitorijima.
    /// </summary>
    public class VrstaMaterijalaValidator
    {
        private readonly IVrstaMaterijalaRepository _vrstaMaterijalaRepository;
        // NIJE VIŠE POTREBAN IMaterijalRepository za ovu provjeru jer je delegirana
        // private readonly IMaterijalRepository _materijalRepository; 

        public VrstaMaterijalaValidator(IVrstaMaterijalaRepository vrstaMaterijalaRepository) // Uklonjen IMaterijalRepository iz konstruktora
        {
            _vrstaMaterijalaRepository = vrstaMaterijalaRepository ?? throw new ArgumentNullException(nameof(vrstaMaterijalaRepository));
            // _materijalRepository = materijalRepository ?? throw new ArgumentNullException(nameof(materijalRepository)); // Uklonjeno
        }

        /// <summary>
        /// Validira vrstu materijala prije spremanja ili ažuriranja.
        /// </summary>
        /// <param name="vrstaMaterijala">Entitet vrste materijala koji se validira.</param>
        /// <param name="isNew">True ako se radi o novoj vrsti, false ako se ažurira postojeća.</param>
        /// <returns>True ako je vrsta materijala validna.</returns>
        /// <exception cref="DomainValidationException">Baca iznimku ako validacija ne prođe.</exception>
        public async Task ValidateVrstaMaterijalaAsync(VrstaMaterijala vrstaMaterijala, bool isNew)
        {
            var errors = new Dictionary<string, string[]>();

            // Pravilo: Naziv vrste materijala mora biti jedinstven.
            bool isUnique = await _vrstaMaterijalaRepository.IsNazivUniqueAsync(vrstaMaterijala.Naziv, isNew ? (int?)null : vrstaMaterijala.VrstaId);
            if (!isUnique)
            {
                errors.Add(nameof(vrstaMaterijala.Naziv), new[] { "Vrsta materijala s ovim nazivom već postoji." });
            }

            if (errors.Count > 0)
            {
                throw new DomainValidationException("Validacija vrste materijala nije uspjela.", errors);
            }
        }

        /// <summary>
        /// Provjerava može li se vrsta materijala obrisati.
        /// </summary>
        /// <param name="vrstaId">ID vrste materijala za provjeru.</param>
        /// <exception cref="DomainValidationException">Baca iznimku ako se vrsta ne može obrisati zbog povezanih materijala.</exception>
        public async Task ValidateDeleteVrstaMaterijalaAsync(int vrstaId)
        {
            // PROMIJENJENO: Pozivamo HasRelatedMaterijaliAsync na _vrstaMaterijalaRepository
            bool hasRelated = await _vrstaMaterijalaRepository.HasRelatedMaterijaliAsync(vrstaId);
            if (hasRelated)
            {
                throw new DomainValidationException($"Vrsta materijala s ID-jem {vrstaId} ne može se obrisati jer postoje povezani materijali.");
            }
        }
    }
}