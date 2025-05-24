using BeautySalon.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Application.Interfaces
{
    public interface IZaposlenikService
    {
        IEnumerable<ZaposlenikDto> GetAll();
        ZaposlenikDto GetById(int id);
        void Add(ZaposlenikDto zaposlenik);
        void Update(ZaposlenikDto zaposlenik);
        void Delete(int id);
    }
}
