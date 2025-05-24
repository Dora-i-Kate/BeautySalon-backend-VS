using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BeautySalon.DataAccess.Repositories
{
    public class KlijentRepository : IKlijentRepository
    {
        private readonly BeautySalonDbContext _context;

        public KlijentRepository(BeautySalonDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Klijent> GetAll()
        {
            return _context.Klijenti.ToList();
        }

        public Klijent GetById(int id)
        {
            return _context.Klijenti.Find(id);
        }

        public void Add(Klijent klijent)
        {
            _context.Klijenti.Add(klijent);
            _context.SaveChanges();
        }

        public void Update(Klijent klijent)
        {
            _context.Klijenti.Update(klijent);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var klijent = _context.Klijenti.Find(id);
            if (klijent != null)
            {
                _context.Klijenti.Remove(klijent);
                _context.SaveChanges();
            }
        }
    }
}
