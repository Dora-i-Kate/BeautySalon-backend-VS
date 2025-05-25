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
    /// Domenski servis za validaciju termina.
    /// Sadrži složena poslovna pravila koja zahtijevaju pristup drugim entitetima ili repozitorijima.
    /// </summary>
    public class TerminValidator
    {
        private readonly ITerminRepository _terminRepository;
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly IUslugaRepository _uslugaRepository;

        public TerminValidator(ITerminRepository terminRepository, IKorisnikRepository korisnikRepository, IUslugaRepository uslugaRepository)
        {
            _terminRepository = terminRepository ?? throw new ArgumentNullException(nameof(terminRepository));
            _korisnikRepository = korisnikRepository ?? throw new ArgumentNullException(nameof(korisnikRepository));
            _uslugaRepository = uslugaRepository ?? throw new ArgumentNullException(nameof(uslugaRepository));
        }

        /// <summary>
        /// Validira termin prije spremanja ili ažuriranja.
        /// </summary>
        /// <param name="termin">Entitet termina koji se validira.</param>
        /// <param name="isNew">True ako se radi o novom terminu, false ako se ažurira postojeći.</param>
        /// <returns>True ako je termin validan.</returns>
        /// <exception cref="DomainValidationException">Baca iznimku ako validacija ne prođe.</exception>
        public async Task ValidateTerminAsync(Termin termin, bool isNew)
        {
            var errors = new Dictionary<string, string[]>();

            // Pravilo: Termin se ne može zakazati u prošlosti.
            // Provjera datuma i vremena zajedno.
            DateTime terminDateTime = termin.Datum.Date + termin.Vrijeme;
            if (terminDateTime < DateTime.Now)
            {
                errors.Add(nameof(termin.Datum), new[] { "Termin se ne može zakazati u prošlosti." });
            }

            // Pravilo: Trajanje termina mora biti višekratnik od 15 minuta.
            if (termin.TrajanjeMinuta % 15 != 0)
            {
                errors.Add(nameof(termin.TrajanjeMinuta), new[] { "Trajanje termina mora biti višekratnik od 15 minuta." });
            }

            // Pravilo: Zaposlenik ne može imati preklapajuće termine.
            bool hasOverlap = await _terminRepository.HasOverlappingTerminAsync(
                termin.ZaposlenikId,
                termin.Datum,
                termin.Vrijeme,
                termin.TrajanjeMinuta,
                isNew ? (int?)null : termin.Id // Isključi trenutni termin ako se ažurira
            );

            if (hasOverlap)
            {
                errors.Add(nameof(termin.ZaposlenikId), new[] { "Odabrani zaposlenik već ima preklapajući termin u tom vremenskom razdoblju." });
            }

            // Provjera postojanja Klijenta i Zaposlenika
            var klijent = await _korisnikRepository.GetByIdAsync(termin.KlijentId);
            if (klijent == null)
            {
                errors.Add(nameof(termin.KlijentId), new[] { "Odabrani klijent ne postoji." });
            }
            // Dodana provjera da li je klijent.Uloga null prije pristupa NazivUloge (ako navigacijsko svojstvo nije učitano)
            else if (klijent.Uloga == null || klijent.Uloga.NazivUloge != UlogaNaziv.Klijent.ToString())
            {
                errors.Add(nameof(termin.KlijentId), new[] { "Odabrani korisnik nije klijent ili mu uloga nije definirana." });
            }

            var zaposlenik = await _korisnikRepository.GetByIdAsync(termin.ZaposlenikId);
            if (zaposlenik == null)
            {
                errors.Add(nameof(termin.ZaposlenikId), new[] { "Odabrani zaposlenik ne postoji." });
            }
            // Dodana provjera da li je zaposlenik.Uloga null prije pristupa NazivUloge (ako navigacijsko svojstvo nije učitano)
            else if (zaposlenik.Uloga == null || zaposlenik.Uloga.NazivUloge != UlogaNaziv.Zaposlenik.ToString())
            {
                errors.Add(nameof(termin.ZaposlenikId), new[] { "Odabrani korisnik nije zaposlenik ili mu uloga nije definirana." });
            }

            // Validacija stavki termina
            foreach (var stavka in termin.StavkeTermina)
            {
                var usluga = await _uslugaRepository.GetByIdAsync(stavka.UslugaId);
                if (usluga == null)
                {
                    // ISPRAVKA: Uklonjen '?? 0' jer stavka.Id ne može biti null (int tip)
                    errors.Add($"StavkaTermina_{stavka.Id}_UslugaId", new[] { $"Usluga s ID-jem {stavka.UslugaId} ne postoji." });
                }
                else
                {
                    // Pravilo: Cijena usluge u stavci termina ne može biti manja od minimalne definirane cijene za tu uslugu
                    // OVO JE PRIMJER SLOŽENOG PRAVILA KOJE BI ZAHTIJEVALO DODATNU LOGIKU (npr. cjenik, popusti)
                    // Za sada, samo provjeravamo da nije manja od osnovne cijene usluge
                    if (stavka.Cijena < usluga.Cijena * 0.9m) // Dopustimo 10% odstupanja za popuste, ali zahtijeva opravdanje u stvarnom sustavu
                    {
                        // ISPRAVKA: Uklonjen '?? 0' jer stavka.Id ne može biti null (int tip)
                        errors.Add($"StavkaTermina_{stavka.Id}_Cijena", new[] { $"Cijena za uslugu '{usluga.Naziv}' je značajno niža od osnovne cijene ({usluga.Cijena:C}). Potrebno opravdanje." });
                    }
                }

                // Pravilo: Količina usluge mora biti barem 1. (Već je u entitetu, ali može biti i ovdje za konzistentnost)
                if (stavka.Kolicina <= 0)
                {
                    // ISPRAVKA: Uklonjen '?? 0' jer stavka.Id ne može biti null (int tip)
                    errors.Add($"StavkaTermina_{stavka.Id}_Kolicina", new[] { "Količina usluge mora biti barem 1." });
                }
            }

            if (errors.Count > 0)
            {
                throw new DomainValidationException("Validacija termina nije uspjela.", errors);
            }
        }
    }
}