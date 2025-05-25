using BeautySalon.Application.DTOs.Termin;
using BeautySalon.Application.Interfaces;
using BeautySalon.Domain.Exceptions;
using BeautySalon.Domain.Models;
using BeautySalon.PresentationMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Ensure this is present

namespace BeautySalon.PresentationMVC.Controllers
{
    public class TerminiController : Controller
    {
        private readonly ITerminAppService _terminAppService;
        private readonly IKorisnikAppService _korisnikAppService;
        private readonly IUslugaAppService _uslugaAppService;

        public TerminiController(ITerminAppService terminAppService, IKorisnikAppService korisnikAppService, IUslugaAppService uslugaAppService)
        {
            _terminAppService = terminAppService;
            _korisnikAppService = korisnikAppService;
            _uslugaAppService = uslugaAppService;
        }

        // GET: Termini
        public async Task<IActionResult> Index(DateTime? searchDatumOd, DateTime? searchDatumDo, int? searchZaposlenikId, TerminStatus? searchStatus)
        {
            var termini = await _terminAppService.SearchTerminiAsync(searchDatumOd, searchDatumDo, searchZaposlenikId, searchStatus);

            var viewModel = new TerminViewModel
            {
                SearchDatumOd = searchDatumOd,
                SearchDatumDo = searchDatumDo,
                SearchZaposlenikId = searchZaposlenikId,
                SearchStatus = searchStatus,
                Zaposlenici = new SelectList(await _korisnikAppService.GetZaposleniciForLookupAsync(), "Id", "ImePrezime", searchZaposlenikId),
                StatusiTermina = new SelectList(Enum.GetValues(typeof(TerminStatus)).Cast<TerminStatus>().Select(e => new { Id = (int)e, Name = e.ToString() }), "Id", "Name", searchStatus)
            };

            ViewBag.TerminiList = termini; // Proslijedi listu termina u ViewBag za prikaz u Index viewu
            return View(viewModel);
        }

        // GET: Termini/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var terminDto = await _terminAppService.GetTerminByIdAsync(id);
            if (terminDto == null)
            {
                return NotFound();
            }

            var viewModel = new TerminViewModel
            {
                Id = terminDto.Id,
                Datum = terminDto.Datum,
                Vrijeme = terminDto.Vrijeme,
                TrajanjeMinuta = terminDto.TrajanjeMinuta,
                Status = terminDto.Status,
                KlijentId = terminDto.KlijentId,
                ZaposlenikId = terminDto.ZaposlenikId,
                UkupnaCijena = terminDto.UkupnaCijena,
                StavkeTermina = terminDto.StavkeTermina.Select(st => new StavkaTerminaViewModel
                {
                    Id = st.Id,
                    UslugaId = st.UslugaId,
                    UslugaNaziv = st.UslugaNaziv,
                    Kolicina = st.Kolicina,
                    Cijena = st.Cijena
                }).ToList()
            };

            // Popuni SelectListe za Details/Edit view
            await PopulateDropdowns(viewModel);

            return View(viewModel);
        }


        // GET: Termini/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new TerminViewModel
            {
                Datum = DateTime.Now.Date, // Predpopuni današnji datum
                Vrijeme = TimeSpan.FromHours(9), // Predpopuni 9:00
                TrajanjeMinuta = 60, // Predpopuni 60 minuta
                Status = TerminStatus.Zakazan, // Predpopuni status
                StavkeTermina = new List<StavkaTerminaViewModel>() // KLJUČNA INICIJALIZACIJA ZA NOVI TERMIN
            };

            await PopulateDropdowns(viewModel); // Popuni SelectListe

            // Ako koristite isti 'Details' view za Create i Edit, ovo je u redu.
            // Inače biste vratili View("Create", viewModel);
            return View("Details", viewModel);
        }

        // POST: Termini/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TerminViewModel viewModel)
        {
            // Ponovno popuni SelectListe u slučaju greške validacije
            await PopulateDropdowns(viewModel);

            // Dodatna provjera ako model binder nije uspio vezati stavke termina (npr. prazna lista)
            if (viewModel.StavkeTermina == null)
            {
                viewModel.StavkeTermina = new List<StavkaTerminaViewModel>();
            }

            if (!ModelState.IsValid)
            {
                return View("Details", viewModel);
            }

            var createDto = new CreateTerminDto
            {
                Datum = viewModel.Datum,
                Vrijeme = viewModel.Vrijeme,
                TrajanjeMinuta = viewModel.TrajanjeMinuta,
                KlijentId = viewModel.KlijentId,
                ZaposlenikId = viewModel.ZaposlenikId,
                StavkeTermina = viewModel.StavkeTermina.Select(st => new CreateStavkaTerminaDto
                {
                    UslugaId = st.UslugaId,
                    Kolicina = st.Kolicina,
                    Cijena = st.Cijena
                }).ToList()
            };

            try
            {
                var createdTermin = await _terminAppService.CreateTerminAsync(createDto);
                TempData["SuccessMessage"] = "Termin je uspješno kreiran!";
                return RedirectToAction(nameof(Details), new { id = createdTermin.Id });
            }
            catch (DomainValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    foreach (var msg in error.Value)
                    {
                        ModelState.AddModelError(error.Key, msg);
                    }
                }
                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Došlo je do pogreške prilikom kreiranja termina: {ex.Message}");
                return View("Details", viewModel);
            }
        }

        // POST: Termini/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TerminViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            // Ponovno popuni SelectListe u slučaju greške validacije
            await PopulateDropdowns(viewModel);

            // Dodatna provjera ako model binder nije uspio vezati stavke termina (npr. prazna lista)
            if (viewModel.StavkeTermina == null)
            {
                viewModel.StavkeTermina = new List<StavkaTerminaViewModel>();
            }


            if (!ModelState.IsValid)
            {
                return View("Details", viewModel);
            }

            var updateDto = new UpdateTerminDto
            {
                Id = viewModel.Id,
                Datum = viewModel.Datum,
                Vrijeme = viewModel.Vrijeme,
                TrajanjeMinuta = viewModel.TrajanjeMinuta,
                Status = viewModel.Status,
                KlijentId = viewModel.KlijentId,
                ZaposlenikId = viewModel.ZaposlenikId,
                StavkeTermina = viewModel.StavkeTermina.Select(st => new UpdateStavkaTerminaDto
                {
                    Id = st.Id,
                    UslugaId = st.UslugaId,
                    Kolicina = st.Kolicina,
                    Cijena = st.Cijena
                }).ToList()
            };

            try
            {
                await _terminAppService.UpdateTerminAsync(updateDto);
                TempData["SuccessMessage"] = "Termin je uspješno ažuriran!";
                return RedirectToAction(nameof(Details), new { id = viewModel.Id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DomainValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    foreach (var msg in error.Value)
                    {
                        ModelState.AddModelError(error.Key, msg);
                    }
                }
                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Došlo je do pogreške prilikom ažuriranja termina: {ex.Message}");
                return View("Details", viewModel);
            }
        }

        // POST: Termini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _terminAppService.DeleteTerminAsync(id);
                TempData["SuccessMessage"] = "Termin je uspješno obrisan!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Došlo je do pogreške prilikom brisanja termina: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

        // Pomoćna metoda za popunjavanje SelectListova
        private async Task PopulateDropdowns(TerminViewModel viewModel)
        {
            viewModel.Klijenti = new SelectList(await _korisnikAppService.GetKlijentiForLookupAsync(), "Id", "ImePrezime", viewModel.KlijentId);
            viewModel.Zaposlenici = new SelectList(await _korisnikAppService.GetZaposleniciForLookupAsync(), "Id", "ImePrezime", viewModel.ZaposlenikId);
            viewModel.Usluge = new SelectList(await _uslugaAppService.GetAllUslugeAsync(), "Id", "Naziv");
            viewModel.StatusiTermina = new SelectList(Enum.GetValues(typeof(TerminStatus)).Cast<TerminStatus>().Select(e => new { Id = (int)e, Name = e.ToString() }), "Id", "Name", viewModel.Status);
        }
    }
}