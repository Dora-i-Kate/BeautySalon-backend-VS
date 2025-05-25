using BeautySalon.Application.DTOs.Termin;
using BeautySalon.Application.Interfaces;
using BeautySalon.Domain.Exceptions;
using BeautySalon.Domain.Models;
using BeautySalon.PresentationMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic; // Dodaj ako nije već tu
using System.Linq; // Dodaj ako nije već tu
using System; // Dodaj ako nije već tu
using System.Threading.Tasks; // Dodaj ako nije već tu

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
            // Dohvaćamo listu termina na temelju parametara pretrage
            // Ovdje koristimo TerminDto jer je to tip koji vraća SearchTerminiAsync
            var terminiDto = await _terminAppService.SearchTerminiAsync(searchDatumOd, searchDatumDo, searchZaposlenikId, searchStatus);

            // Kreiramo TerminSearchViewModel za prikaz pretrage i rezultata
            var viewModel = new TerminSearchViewModel // Opet, pretpostavljam da ćeš dodati ovu klasu
            {
                SearchDatumOd = searchDatumOd,
                SearchDatumDo = searchDatumDo,
                SearchZaposlenikId = searchZaposlenikId,
                SearchStatus = searchStatus,

                Zaposlenici = new SelectList(await _korisnikAppService.GetZaposleniciForLookupAsync(), "Id", "ImePrezime", searchZaposlenikId),
                StatusiTermina = new SelectList(Enum.GetValues(typeof(TerminStatus)).Cast<TerminStatus>().Select(e => new { Id = (int)e, Name = e.ToString() }), "Id", "Name", searchStatus),

                // Mapiramo DTO-ove u listu TerminViewModel (ako je TerminViewModel prikazni model za listu)
                // Ako imaš TerminListDto, to bi bilo bolje koristiti ovdje
                Termini = terminiDto.Select(t => new TerminViewModel // ILI TerminListViewModel
                {
                    Id = t.Id,
                    Datum = t.Datum,
                    Vrijeme = t.Vrijeme,
                    TrajanjeMinuta = t.TrajanjeMinuta,
                    Status = t.Status,
                    KlijentId = t.KlijentId,
                    KlijentImePrezime = t.KlijentImePrezime, // Dodaj KlijentImePrezime u TerminViewModel ako ga želiš prikazati
                    ZaposlenikId = t.ZaposlenikId,
                    ZaposlenikImePrezime = t.ZaposlenikImePrezime, // Dodaj ZaposlenikImePrezime u TerminViewModel
                    UkupnaCijena = t.UkupnaCijena
                    // StavkeTermina nisu potrebne za Index prikaz liste
                }).ToList()
            };

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
                    Cijena = st.Cijena,
                    IsDeleted = false
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
                    new StavkaTerminaViewModel { Id = 0, Kolicina = 1, Cijena = 0.00m, IsDeleted = false }
                }
            };

            await PopulateDropdowns(viewModel);
            return View("Details", viewModel);
        }

        // POST: Termini/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TerminViewModel viewModel)
        {
            await PopulateDropdowns(viewModel);

            if (viewModel.StavkeTermina == null)
            {
                viewModel.StavkeTermina = new List<StavkaTerminaViewModel>();
            }

            // Ukloni validaciju za stavke koje su obrisane na frontendu ili su prazne
            foreach (var stavka in viewModel.StavkeTermina)
            {
                if (stavka.IsDeleted || (stavka.UslugaId == 0 && stavka.Kolicina == 0 && stavka.Cijena == 0m))
                {
                    ModelState.Remove($"StavkeTermina[{viewModel.StavkeTermina.IndexOf(stavka)}].UslugaId");
                    ModelState.Remove($"StavkeTermina[{viewModel.StavkeTermina.IndexOf(stavka)}].Kolicina");
                    ModelState.Remove($"StavkeTermina[{viewModel.StavkeTermina.IndexOf(stavka)}].Cijena");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Details", viewModel);
            }

            // Filtriraj samo aktivne stavke za CreateTerminDto
            var activeStavke = viewModel.StavkeTermina
                                        .Where(st => !st.IsDeleted && st.UslugaId != 0) // Dodana provjera UslugaId za sigurnost
                                        .Select(st => new CreateStavkaTerminaDto
                                        {
                                            UslugaId = st.UslugaId,
                                            Kolicina = st.Kolicina,
                                            Cijena = st.Cijena
                                        }).ToList();

            var createDto = new CreateTerminDto
            {
                Datum = viewModel.Datum,
                Vrijeme = viewModel.Vrijeme,
                TrajanjeMinuta = viewModel.TrajanjeMinuta,
                KlijentId = viewModel.KlijentId,
                ZaposlenikId = viewModel.ZaposlenikId,
                StavkeTermina = activeStavke // Šaljemo samo aktivne stavke
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

            await PopulateDropdowns(viewModel);

            if (viewModel.StavkeTermina == null)
            {
                viewModel.StavkeTermina = new List<StavkaTerminaViewModel>();
            }

            // Ukloni validaciju za stavke koje su obrisane na frontendu ili su prazne
            foreach (var stavka in viewModel.StavkeTermina)
            {
                if (stavka.IsDeleted || (stavka.UslugaId == 0 && stavka.Kolicina == 0 && stavka.Cijena == 0m))
                {
                    ModelState.Remove($"StavkeTermina[{viewModel.StavkeTermina.IndexOf(stavka)}].UslugaId");
                    ModelState.Remove($"StavkeTermina[{viewModel.StavkeTermina.IndexOf(stavka)}].Kolicina");
                    ModelState.Remove($"StavkeTermina[{viewModel.StavkeTermina.IndexOf(stavka)}].Cijena");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Details", viewModel);
            }

            // --- KLJUČNA PROMJENA: Prilagodba za tvoju postojeću UpdateTerminAsync metodu ---
            // Tvoja UpdateTerminAsync metoda očekuje da u UpdateTerminDto budu SVE STAVKE
            // koje bi trebale biti prisutne nakon ažuriranja.
            // Ona sama detektira koje su obrisane usporedbom s postojećim stavkama.
            // Stoga, ne šaljemo zasebnu listu deletedStavkaIds ovdje.
            var allStavkeForUpdate = viewModel.StavkeTermina
                                               .Where(st => !st.IsDeleted && st.UslugaId != 0) // Opet, filtriramo prazne/obrisane
                                               .Select(st => new UpdateStavkaTerminaDto // Koristimo UpdateStavkaTerminaDto
                                               {
                                                   Id = st.Id,
                                                   UslugaId = st.UslugaId,
                                                   Kolicina = st.Kolicina,
                                                   Cijena = st.Cijena
                                               }).ToList();

            var updateDto = new UpdateTerminDto
            {
                Id = viewModel.Id,
                Datum = viewModel.Datum,
                Vrijeme = viewModel.Vrijeme,
                TrajanjeMinuta = viewModel.TrajanjeMinuta,
                Status = viewModel.Status,
                KlijentId = viewModel.KlijentId,
                ZaposlenikId = viewModel.ZaposlenikId,
                StavkeTermina = allStavkeForUpdate // Šaljemo sve *aktivne* stavke
            };

            try
            {
                // Pozivamo tvoju postojeću metodu UpdateTerminAsync
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
                // Ako termin nije pronađen, vrati 404
                return NotFound();
            }
            catch (Exception ex)
            {
                // Ako dođe do druge greške, prikaži poruku i preusmjeri na Index
                TempData["ErrorMessage"] = $"Došlo je do pogreške prilikom brisanja termina: {ex.Message}";
                return RedirectToAction(nameof(Index));
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
                Cijena = 0.00m,
                IsDeleted = false
            };

            var usluge = await _uslugaAppService.GetAllUslugeAsync();
            var uslugeSelectList = new SelectList(usluge, "Id", "Naziv");

            ViewData["Index"] = index;
            ViewData["Usluge"] = uslugeSelectList;

            return PartialView("_StavkaTerminaPartial", model);
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