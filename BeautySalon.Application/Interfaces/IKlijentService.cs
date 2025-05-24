using BeautySalon.Application.DTOs;
using BeautySalon.Domain.Models;

namespace BeautySalon.Application.Interfaces
{
    public interface IKlijentService
    {
        IEnumerable<KlijentDto> GetAll();
        KlijentDto GetById(int id);
        void Add(KlijentDto klijent);
        void Update(KlijentDto klijent);
        void Delete(int id);
    }

}
