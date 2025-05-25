using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySalon.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeautySalon.Application.DTOs;

namespace BeautySalon.Application.Interfaces
{
    public interface IUslugaService
    {
        Task<List<UslugaDto>> GetAllAsync();
        Task<UslugaDto?> GetByIdAsync(int id);
        Task AddAsync(UslugaDto usluga);
        Task UpdateAsync(UslugaDto usluga);
        Task DeleteAsync(int id);
        Task<List<UslugaDto>> SearchAsync(string? searchTerm);
    }
}
