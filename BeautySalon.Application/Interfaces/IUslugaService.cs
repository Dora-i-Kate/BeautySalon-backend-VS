using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySalon.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    public interface IUslugaService
    {
        Task<List<Usluga>> GetAllAsync();
        Task<Usluga?> GetByIdAsync(int id);
        Task AddAsync(Usluga usluga);
        Task UpdateAsync(Usluga usluga);
        Task DeleteAsync(int id);
    }
}
