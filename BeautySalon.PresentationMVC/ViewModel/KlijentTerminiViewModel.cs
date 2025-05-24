using BeautySalon.Application.DTOs;
using BeautySalon.Domain.Models;

using BeautySalon.Application.DTOs;

namespace BeautySalon.PresentationMVC.ViewModel
{
    public class KlijentTerminiViewModel
    {
        public KlijentDto Klijent { get; set; }
        public List<TerminDto> Termini { get; set; }
        public TerminDto NoviIliIzmijenjeniTermin { get; set; }
        public List<ZaposlenikDto> Zaposlenici { get; set; }
    }


}

