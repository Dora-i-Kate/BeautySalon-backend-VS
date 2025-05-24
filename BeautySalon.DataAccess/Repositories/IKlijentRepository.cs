using BeautySalon.Domain.Models;
using System.Collections.Generic;

namespace BeautySalon.DataAccess.Repositories
{
    public interface IKlijentRepository
    {
        IEnumerable<Klijent> GetAll();
        Klijent GetById(int id);
        void Add(Klijent klijent);
        void Update(Klijent klijent);
        void Delete(int id);
    }
}
