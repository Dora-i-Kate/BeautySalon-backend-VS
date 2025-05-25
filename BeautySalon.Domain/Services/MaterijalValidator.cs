// BeautySalon.Domain.Services.MaterijalValidator.cs
using BeautySalon.Domain.Exceptions;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Services
{
    /// <summary>
    /// Domenski servis za validaciju materijala.
    /// Sadrži složena poslovna pravila koja zahtijevaju pristup repozitorijima.
    /// </summary>
    public class MaterijalValidator
    {
        private readonly IMaterijalRepository _materijalRepository;
        private readonly IVrstaMaterijalaRepository _vrstaMaterijalaRepository;

        public MaterijalValidator(IMaterijalRepository materijalRepository, IVrstaMaterijalaRepository vrstaMaterijalaRepository)
        {
            _materijalRepository = materijalRepository ?? throw new ArgumentNullException(nameof(materijalRepository));
            _vrstaMaterijalaRepository = vrstaMaterijalaRepository ?? throw new ArgumentNullException(nameof(vrstaMaterijalaRepository));
        }

        /// <summary>
        /// Validira materijal prije spremanja ili ažuriranja.
        /// </summary>
        /// <param name="materijal">Entitet materijala koji se validira.</param>
        /// <param name="isNew">True ako se radi o novom materijalu, false ako se ažurira postojeći.</param>
        /// <returns>True ako je materijal validan.</returns>
        /// <exception cref="DomainValidationException">Baca iznimku ako validacija ne prođe.</exception>
        public async Task ValidateMaterijalAsync(Materijal materijal, bool isNew)
        {
            var errors = new Dictionary<string, string[]>();

            // Pravilo: Naziv materijala mora biti jedinstven.
            bool isUnique = await _materijalRepository.IsNazivUniqueAsync(materijal.Naziv, isNew ? (int?)null : materijal.MaterijalId);
            if (!isUnique)
            {
                errors.Add(nameof(materijal.Naziv), new[] { "Materijal s ovim nazivom već postoji." });
            }

            // Pravilo: Cijena materijala mora biti pozitivna.
            if (materijal.Cijena <= 0)
            {
                errors.Add(nameof(materijal.Cijena), new[] { "Cijena materijala mora biti veća od nule." });
            }

            // Pravilo: Minimalna i trenutna količina ne smiju biti negativne.
            if (materijal.MinimalnaKolicina < 0)
            {
                errors.Add(nameof(materijal.MinimalnaKolicina), new[] { "Minimalna količina ne smije biti negativna." });
            }
            if (materijal.TrenutnaKolicina < 0)
            {
                errors.Add(nameof(materijal.TrenutnaKolicina), new[] { "Trenutna količina ne smije biti negativna." });
            }

            // Pravilo: Jedinica mjere ne smije biti prazna.
            if (string.IsNullOrWhiteSpace(materijal.JedinicaMjere))
            {
                errors.Add(nameof(materijal.JedinicaMjere), new[] { "Jedinica mjere je obavezna." });
            }

            // Pravilo: Vrsta materijala mora postojati.
            var vrstaMaterijala = await _vrstaMaterijalaRepository.GetByIdAsync(materijal.VrstaId);
            if (vrstaMaterijala == null)
            {
                errors.Add(nameof(materijal.VrstaId), new[] { "Odabrana vrsta materijala ne postoji." });
            }

            if (errors.Count > 0)
            {
                throw new DomainValidationException("Validacija materijala nije uspjela.", errors);
            }
        }
    }
}