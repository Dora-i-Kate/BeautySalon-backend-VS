using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Domain.Models
{
    public class VrstaMaterijala
    {
        public int VrstaId { get; set; }
        public required string Naziv { get; set; }
    }
}
