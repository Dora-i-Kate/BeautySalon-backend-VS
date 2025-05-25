using BeautySalon.Domain.Exceptions;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Services
{
    /// <summary>
    /// Domenski servis za validaciju usluga.
    /// Sadrži složena poslovna pravila koja zahtijevaju pristup repozitorijima.
    /// </summary>
    public class UslugaValidator
    {
        private readonly IUslugaRepository _uslugaRepository;

        public UslugaValidator(IUslugaRepository uslugaRepository)
        {
            _uslugaRepository = uslugaRepository ?? throw new ArgumentNullException(nameof(uslugaRepository));
        }

        /// <summary>
        /// Validira uslugu prije spremanja ili ažuriranja.
        /// </summary>
        /// <param name="usluga">Entitet usluge koji se validira.</param>
        /// <param name="isNew">True ako se radi o novoj usluzi, false ako se ažurira postojeća.</param>
        /// <returns>True ako je usluga validna.</returns>
        /// <exception cref="DomainValidationException">Baca iznimku ako validacija ne prođe.</exception>
        public async Task ValidateUslugaAsync(Usluga usluga, bool isNew)
        {
            var errors = new Dictionary<string, string[]>();

            // Pravilo: Naziv usluge mora biti jedinstven.
            bool isUnique = await _uslugaRepository.IsNazivUniqueAsync(usluga.Naziv, isNew ? (int?)null : usluga.Id);
            if (!isUnique)
            {
                errors.Add(nameof(usluga.Naziv), new[] { "Usluga s ovim nazivom već postoji." });
            }

            // Pravilo: Cijena usluge mora biti pozitivna i veća od 0.
            if (usluga.Cijena <= 0)
            {
                errors.Add(nameof(usluga.Cijena), new[] { "Cijena usluge mora biti veća od nule." });
            }

            // Pravilo: Trajanje usluge mora biti u rasponu od 15 do 240 minuta.
            if (usluga.TrajanjeMinuta < 15 || usluga.TrajanjeMinuta > 240)
            {
                errors.Add(nameof(usluga.TrajanjeMinuta), new[] { "Trajanje usluge mora biti između 15 i 240 minuta." });
            }

            if (errors.Count > 0)
            {
                throw new DomainValidationException("Validacija usluge nije uspjela.", errors);
            }
        }
    }
}
