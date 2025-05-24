using BeautySalon.Application.DTOs;
using BeautySalon.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    public interface ITerminService
    {
        IEnumerable<TerminDto> GetAll();
        TerminDto GetById(int id);
        public void Add(CreateTerminDto termin);
        public void Update(TerminDto termin);

        void Delete(int id);

        IEnumerable<TerminDto> GetByKlijentId(int klijentId); // Dodano!
    }
}
