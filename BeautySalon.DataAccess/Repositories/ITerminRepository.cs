using BeautySalon.Domain.Models;
using System.Collections.Generic;

namespace BeautySalon.DataAccess.Repositories
{
    public interface ITerminRepository
    {
        IEnumerable<Termin> GetAll();
        Termin GetById(int id);
        void Add(Termin termin);
        void Update(Termin termin);
        void Delete(int id);
        IEnumerable<Termin> GetByKlijentId(int klijentId);

        void Save(); // <- DODAJ OVO
    }


}
