using BeautySalon.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.DataAccess.Repositories
{
    public class ZaposlenikRepository : IZaposlenikRepository
    {
        private readonly BeautySalonDbContext _context;

        public ZaposlenikRepository(BeautySalonDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Zaposlenik> GetAll()
        {
            return _context.Zaposlenici.ToList();
        }

        public Zaposlenik GetById(int id)
        {
            return _context.Zaposlenici.Find(id);
        }

        public void Add(Zaposlenik zaposlenik)
        {
            _context.Zaposlenici.Add(zaposlenik);
            _context.SaveChanges();
        }

        public void Update(Zaposlenik zaposlenik)
        {
            _context.Zaposlenici.Update(zaposlenik);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var zaposlenik = _context.Zaposlenici.Find(id);
            if (zaposlenik != null)
            {
                _context.Zaposlenici.Remove(zaposlenik);
                _context.SaveChanges();
            }
        }
    }
}
