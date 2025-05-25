using BeautySalon.Application.DTOs.Termin;
using BeautySalon.Application.Interfaces;
using BeautySalon.Domain.Exceptions;
using BeautySalon.Domain.Models;
using BeautySalon.PresentationMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

            ViewBag.TerminiList = termini;
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

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // GET: Termini/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new TerminViewModel
            {
                Datum = DateTime.Now.Date,
                Vrijeme = TimeSpan.FromHours(9),
                TrajanjeMinuta = 60,
                Status = TerminStatus.Zakazan,
                StavkeTermina = new List<StavkaTerminaViewModel>
                {
                    // Inicijalizirajte s jednom praznom stavkom kako bi se odmah prikazalo polje za uslugu
                    new StavkaTerminaViewModel { Id = 0, Kolicina = 1, Cijena = 0.00m }
                }
            };

            await PopulateDropdowns(viewModel); // Popuni SelectListe u ViewModelu

            // PROMJENA OVDJE:
            // Umjesto da proslijeđuješ "ParentUsluge" u ViewData, sada partial view
            // očekuje "Usluge" u svom ViewData, što će biti proslijeđeno iz glavnog ViewModel.Usluge.
            // Nema potrebe za ovim retkom ako se koristi pristup proslijeđivanja iz glavnog ViewModela.
            // ViewData["ParentUsluge"] = viewModel.Usluge; // OVO VIŠE NE TREBA AKO KORISTIŠ DONJI PRISTUP

            return View("Details", viewModel);
        }

        // POST: Termini/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TerminViewModel viewModel)
        {
            await PopulateDropdowns(viewModel); // Popuni SelectListe u ViewModelu

            if (viewModel.StavkeTermina == null)
            {
                viewModel.StavkeTermina = new List<StavkaTerminaViewModel>();
            }

            // Ukloni validaciju za UslugaId ako je 0 (jer je to placeholder za "Odaberite uslugu")
            // Imajte na umu da ovo možda nije idealno rješenje za produkcijsku aplikaciju,
            // bolja je validacija u ViewModelu ili DTO-u.
            // Ostavljam ovdje da rješim problem s prikazom, ali razmislite o refaktoriranju validacije.
            foreach (var stavka in viewModel.StavkeTermina)
            {
                if (stavka.UslugaId == 0)
                {
                    ModelState.Remove($"StavkeTermina[{viewModel.StavkeTermina.IndexOf(stavka)}].UslugaId");
                }
            }


            if (!ModelState.IsValid)
            {
                // Nema potrebe za ponovnim postavljanjem "ParentUsluge" ovdje ako se koristi pristup
                // gdje se SelectList direktno prosljeđuje iz glavnog ViewModela u partial.
                // ViewData["ParentUsluge"] = viewModel.Usluge;
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
                // Nema potrebe za ponovnim postavljanjem "ParentUsluge" ovdje.
                // ViewData["ParentUsluge"] = viewModel.Usluge;
                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Došlo je do pogreške prilikom kreiranja termina: {ex.Message}");
                // Nema potrebe za ponovnim postavljanjem "ParentUsluge" ovdje.
                // ViewData["ParentUsluge"] = viewModel.Usluge;
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

            await PopulateDropdowns(viewModel); // Popuni SelectListe u ViewModelu

            if (viewModel.StavkeTermina == null)
            {
                viewModel.StavkeTermina = new List<StavkaTerminaViewModel>();
            }

            // Ukloni validaciju za UslugaId ako je 0
            foreach (var stavka in viewModel.StavkeTermina)
            {
                if (stavka.UslugaId == 0)
                {
                    ModelState.Remove($"StavkeTermina[{viewModel.StavkeTermina.IndexOf(stavka)}].UslugaId");
                }
            }


            if (!ModelState.IsValid)
            {
                // Nema potrebe za ponovnim postavljanjem "ParentUsluge" ovdje.
                // ViewData["ParentUsluge"] = viewModel.Usluge;
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
                // Nema potrebe za ponovnim postavljanjem "ParentUsluge" ovdje.
                // ViewData["ParentUsluge"] = viewModel.Usluge;
                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Došlo je do pogreške prilikom ažuriranja termina: {ex.Message}");
                // Nema potrebe za ponovnim postavljanjem "ParentUsluge" ovdje.
                // ViewData["ParentUsluge"] = viewModel.Usluge;
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

        // Nova metoda za vraćanje partial viewa za novu stavku termina.
        [HttpGet]
        public async Task<IActionResult> GetNewStavkaTerminaPartial(int index)
        {
            var model = new StavkaTerminaViewModel
            {
                Id = 0,
                Kolicina = 1,
                Cijena = 0.00m
            };

            var usluge = await _uslugaAppService.GetAllUslugeAsync();
            var uslugeSelectList = new SelectList(usluge, "Id", "Naziv");

            ViewData["Index"] = index;
            // PROMJENA OVDJE: Koristi ključ "Usluge" umjesto "ParentUsluge"
            ViewData["Usluge"] = uslugeSelectList; // Proslijedi SelectList za usluge

            return PartialView("_StavkaTerminaPartial", model);
        }

        // Pomoćna metoda za popunjavanje SelectListova
        private async Task PopulateDropdowns(TerminViewModel viewModel)
        {
            viewModel.Klijenti = new SelectList(await _korisnikAppService.GetKlijentiForLookupAsync(), "Id", "ImePrezime", viewModel.KlijentId);
            viewModel.Zaposlenici = new SelectList(await _korisnikAppService.GetZaposleniciForLookupAsync(), "Id", "ImePrezime", viewModel.ZaposlenikId);
            // Dodana linija za popunjavanje usluga u glavni ViewModel
            viewModel.Usluge = new SelectList(await _uslugaAppService.GetAllUslugeAsync(), "Id", "Naziv");
            viewModel.StatusiTermina = new SelectList(Enum.GetValues(typeof(TerminStatus)).Cast<TerminStatus>().Select(e => new { Id = (int)e, Name = e.ToString() }), "Id", "Name", viewModel.Status);
        }
    }
}