using BeautySalon.Domain.Models;

namespace BeautySalon.PresentationMVC.ViewModel
{
    public class TerminiViewModel
    {
        public Klijent Klijent { get; set; }
        public IEnumerable<Termin> Termini { get; set; }
    }

}
