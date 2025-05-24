using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BeautySalon.DataAccess.Repositories
{
    public class TerminRepository : ITerminRepository
    {
        private readonly BeautySalonDbContext _context;

        public TerminRepository(BeautySalonDbContext context)
        {
            _context = context;
        }

        public void Add(Termin termin)
        {
            _context.Termini.Add(termin);
            _context.SaveChanges();
        }

        public void Update(Termin termin)
        {
            _context.Termini.Update(termin);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var termin = _context.Termini.Find(id);
            if (termin != null)
                _context.Termini.Remove(termin);
        }

        public IEnumerable<Termin> GetAll()
        {
            return _context.Termini.ToList();
        }

        public Termin GetById(int id)
        {
            return _context.Termini.FirstOrDefault(t => t.TerminId == id);
        }

        public IEnumerable<Termin> GetByKlijentId(int klijentId)
        {
            return _context.Termini.Where(t => t.KlijentId == klijentId).ToList();
        }

        public void Save()
        {
            Console.WriteLine(">>> SaveChanges called");
            _context.SaveChanges();

        }
    }

}
