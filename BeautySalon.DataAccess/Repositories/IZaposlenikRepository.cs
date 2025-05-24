using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.DataAccess.Repositories
{
    public interface IZaposlenikRepository
    {
        IEnumerable<Zaposlenik> GetAll();
        Zaposlenik GetById(int id);
        void Add(Zaposlenik zaposlenik);
        void Update(Zaposlenik zaposlenik);
        void Delete(int id);
    }
}
