using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    public interface IMaterijalService
    {
        Task<List<Materijal>> GetAllAsync();
        Task<Materijal?> GetByIdAsync(int id);
        Task CreateAsync(Materijal materijal);
        Task UpdateAsync(Materijal materijal);
        Task DeleteAsync(int id);
    }
}
