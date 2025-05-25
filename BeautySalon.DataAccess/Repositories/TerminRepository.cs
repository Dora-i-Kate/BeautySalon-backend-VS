using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.DataAccess.Repositories
{
    /// <summary>
    /// Implementacija repozitorija za entitet Termin.
    /// Koristi Entity Framework Core za pristup podacima.
    /// </summary>
    public class TerminRepository : ITerminRepository
    {
        private readonly BeautySalonDbContext _context; // Koristi BeautySalonDbContext

        public TerminRepository(BeautySalonDbContext context)
        {
            _context = context;
        }

        public async Task<Termin> GetByIdAsync(int id)
        {
            // Učitava termin s klijentom, zaposlenikom i svim stavkama termina s uslugama
            return await _context.Termini
                                 .Include(t => t.Klijent)
                                    .ThenInclude(k => k.Uloga) // Učitaj ulogu klijenta
                                 .Include(t => t.Zaposlenik)
                                    .ThenInclude(k => k.Uloga) // Učitaj ulogu zaposlenika
                                 .Include(t => t.StavkeTermina)
                                    .ThenInclude(st => st.Usluga) // Učitaj uslugu za svaku stavku
                                 .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Termin>> GetAllAsync()
        {
            // Učitava sve termine s klijentima i zaposlenicima
            return await _context.Termini
                                 .Include(t => t.Klijent)
                                 .Include(t => t.Zaposlenik)
                                 .OrderBy(t => t.Datum) // Sortiranje po datumu
                                 .ThenBy(t => t.Vrijeme) // Zatim po vremenu
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Termin>> SearchTerminiAsync(DateTime? datumOd = null, DateTime? datumDo = null, int? zaposlenikId = null, TerminStatus? status = null)
        {
            IQueryable<Termin> query = _context.Termini
                                               .Include(t => t.Klijent)
                                               .Include(t => t.Zaposlenik);

            if (datumOd.HasValue)
            {
                query = query.Where(t => t.Datum >= datumOd.Value.Date);
            }

            if (datumDo.HasValue)
            {
                query = query.Where(t => t.Datum <= datumDo.Value.Date);
            }

            if (zaposlenikId.HasValue && zaposlenikId.Value > 0)
            {
                query = query.Where(t => t.ZaposlenikId == zaposlenikId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            return await query.OrderBy(t => t.Datum).ThenBy(t => t.Vrijeme).ToListAsync();
        }

        public async Task<Termin> AddAsync(Termin termin)
        {
            _context.Termini.Add(termin);
            await _context.SaveChangesAsync();
            return termin;
        }

        public async Task UpdateAsync(Termin termin)
        {
            // Attach the existing entity to the context if it's not already tracked
            _context.Termini.Attach(termin);
            _context.Entry(termin).State = EntityState.Modified; // Mark as modified

            // Handle StavkeTermina updates (add, update, delete)
            // This requires careful handling as EF Core doesn't automatically track changes in child collections
            var existingStavke = await _context.StavkeTermina
                                               .Where(st => st.TerminId == termin.Id)
                                               .ToListAsync();

            // Remove deleted items
            foreach (var existingStavka in existingStavke)
            {
                if (!termin.StavkeTermina.Any(s => s.Id == existingStavka.Id))
                {
                    _context.StavkeTermina.Remove(existingStavka);
                }
            }

            // Add or Update items
            foreach (var newStavka in termin.StavkeTermina)
            {
                var existing = existingStavke.FirstOrDefault(s => s.Id == newStavka.Id);
                if (existing == null)
                {
                    // Add new StavkaTermina
                    // newStavka.GetType().GetProperty("TerminId").SetValue(newStavka, termin.Id); // This line is usually not needed if TerminId is set in domain logic or EF tracks it
                    _context.StavkeTermina.Add(newStavka);
                }
                else
                {
                    // Update existing StavkaTermina
                    _context.Entry(existing).CurrentValues.SetValues(newStavka);
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var termin = await _context.Termini.FindAsync(id);
            if (termin != null)
            {
                _context.Termini.Remove(termin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasOverlappingTerminAsync(int zaposlenikId, DateTime datum, TimeSpan vrijeme, int trajanjeMinuta, int? excludeTerminId = null)
        {
            // Izračunaj početak i kraj novog/ažuriranog termina
            DateTime newTerminStart = datum.Date + vrijeme;
            DateTime newTerminEnd = newTerminStart.AddMinutes(trajanjeMinuta);

            // Pronađi sve postojeće termine za tog zaposlenika na taj datum
            var overlappingTermini = await _context.Termini
                .Where(t => t.ZaposlenikId == zaposlenikId && t.Datum == datum)
                .ToListAsync();

            // Filtriraj termine koji se preklapaju
            foreach (var existingTermin in overlappingTermini)
            {
                // Isključi trenutni termin ako se radi o ažuriranju
                if (excludeTerminId.HasValue && existingTermin.Id == excludeTerminId.Value)
                {
                    continue;
                }

                DateTime existingTerminStart = existingTermin.Datum.Date + existingTermin.Vrijeme;
                DateTime existingTerminEnd = existingTerminStart.AddMinutes(existingTermin.TrajanjeMinuta);

                // Provjera preklapanja:
                // (StartA < EndB) && (EndA > StartB)
                if (newTerminStart < existingTerminEnd && newTerminEnd > existingTerminStart)
                {
                    return true; // Postoji preklapanje
                }
            }
            return false; // Nema preklapanja
        }

        public async Task<bool> TerminExistsAsync(int id)
        {
            return await _context.Termini.AnyAsync(t => t.Id == id);
        }
    }
}
