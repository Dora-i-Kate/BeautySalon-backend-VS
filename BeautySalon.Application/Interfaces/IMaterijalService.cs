using BeautySalon.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautySalon.Application.DTOs;

namespace BeautySalon.Application.Interfaces
{
    public interface IMaterijalService
    {
        Task<List<MaterijalDto>> GetAllAsync();
        Task<MaterijalDto?> GetByIdAsync(int id);
        Task CreateAsync(MaterijalDto materijal);
        Task UpdateAsync(MaterijalDto materijal);
        Task DeleteAsync(int id);
        Task<List<VrstaMaterijalaDto>> GetAllVrsteMaterijalaAsync();
    }
}
