using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BeautySalon.Domain.Exceptions;

namespace BeautySalon.Domain.Models
{
    /// <summary>
    /// Reprezentira termin u kozmetičkom salonu.
    /// Master entitet za master-detail formu.
    /// </summary>
    public class Termin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; } // termin_id
        public DateTime Datum { get; private set; }
        public TimeSpan Vrijeme { get; private set; }
        public int TrajanjeMinuta { get; private set; } // trajanje u minutama
        public TerminStatus Status { get; private set; } // status termina

        public int KlijentId { get; private set; } // korisnik_id (klijent)
        public Korisnik Klijent { get; private set; } // Navigacijsko svojstvo

        public int ZaposlenikId { get; private set; } // zaposlenik_id
        public Korisnik Zaposlenik { get; private set; } // Navigacijsko svojstvo

        private readonly List<StavkaTermina> _stavkeTermina = new List<StavkaTermina>();
        public IReadOnlyCollection<StavkaTermina> StavkeTermina => _stavkeTermina.AsReadOnly(); // Navigacijsko svojstvo

        // Prazan konstruktor za EF Core
        private Termin() { }

        /// <summary>
        /// Konstruktor za stvaranje novog termina.
        /// </summary>
        public Termin(DateTime datum, TimeSpan vrijeme, int trajanjeMinuta, int klijentId, int zaposlenikId)
        {
            if (datum == default) throw new ArgumentException("Datum termina ne smije biti prazan.", nameof(datum));
            if (vrijeme == default) throw new ArgumentException("Vrijeme termina ne smije biti prazno.", nameof(vrijeme));
            if (trajanjeMinuta <= 0) throw new ArgumentException("Trajanje termina mora biti pozitivno.", nameof(trajanjeMinuta));
            if (klijentId <= 0) throw new ArgumentException("Klijent ID mora biti pozitivan broj.", nameof(klijentId));
            if (zaposlenikId <= 0) throw new ArgumentException("Zaposlenik ID mora biti pozitivan broj.", nameof(zaposlenikId));

            Datum = datum;
            Vrijeme = vrijeme;
            TrajanjeMinuta = trajanjeMinuta;
            KlijentId = klijentId;
            ZaposlenikId = zaposlenikId;
            Status = TerminStatus.Zakazan; // Početni status
        }

        /// <summary>
        /// Ažurira podatke termina.
        /// </summary>
        public void Update(DateTime datum, TimeSpan vrijeme, int trajanjeMinuta, TerminStatus status, int klijentId, int zaposlenikId)
        {
            if (datum == default) throw new ArgumentException("Datum termina ne smije biti prazan.", nameof(datum));
            if (vrijeme == default) throw new ArgumentException("Vrijeme termina ne smije biti prazno.", nameof(vrijeme));
            if (trajanjeMinuta <= 0) throw new ArgumentException("Trajanje termina mora biti pozitivno.", nameof(trajanjeMinuta));
            if (!Enum.IsDefined(typeof(TerminStatus), status)) throw new ArgumentException("Nevažeći status termina.", nameof(status));
            if (klijentId <= 0) throw new ArgumentException("Klijent ID mora biti pozitivan broj.", nameof(klijentId));
            if (zaposlenikId <= 0) throw new ArgumentException("Zaposlenik ID mora biti pozitivan broj.", nameof(zaposlenikId));

            Datum = datum;
            Vrijeme = vrijeme;
            TrajanjeMinuta = trajanjeMinuta;
            Status = status;
            KlijentId = klijentId;
            ZaposlenikId = zaposlenikId;
        }

        /// <summary>
        /// Dodaje stavku u termin.
        /// </summary>
        public void AddStavka(int uslugaId, int kolicina, decimal cijena)
        {
            var novaStavka = new StavkaTermina(uslugaId, kolicina, cijena);
            _stavkeTermina.Add(novaStavka);
        }

        /// <summary>
        /// Ažurira postojeću stavku u terminu.
        /// </summary>
        public void UpdateStavka(int stavkaId, int uslugaId, int kolicina, decimal cijena)
        {
            var stavka = _stavkeTermina.FirstOrDefault(s => s.Id == stavkaId);
            if (stavka == null)
            {
                throw new DomainValidationException($"Stavka termina s ID-jem {stavkaId} nije pronađena.");
            }
            stavka.Update(uslugaId, kolicina, cijena);
        }

        /// <summary>
        /// Uklanja stavku iz termina.
        /// </summary>
        public void RemoveStavka(int stavkaId)
        {
            var stavka = _stavkeTermina.FirstOrDefault(s => s.Id == stavkaId);
            if (stavka == null)
            {
                throw new DomainValidationException($"Stavka termina s ID-jem {stavkaId} nije pronađena.");
            }
            _stavkeTermina.Remove(stavka);
        }

        /// <summary>
        /// Izračunava ukupnu cijenu termina na temelju cijena stavki.
        /// </summary>
        public decimal CalculateUkupnaCijena()
        {
            return StavkeTermina.Sum(s => s.Kolicina * s.Cijena);
        }
    }
}
