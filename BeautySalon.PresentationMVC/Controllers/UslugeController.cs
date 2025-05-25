using BeautySalon.Application.DTOs.Usluga;
using BeautySalon.Application.Interfaces;
using BeautySalon.Domain.Exceptions;
using BeautySalon.PresentationMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq; // Potrebno za LINQ metode

namespace BeautySalon.PresentationMVC.Controllers
{
    public class UslugeController : Controller
    {
        private readonly IUslugaAppService _uslugaAppService;

        public UslugeController(IUslugaAppService uslugaAppService)
        {
            _uslugaAppService = uslugaAppService;
        }

        // GET: Usluge
        public async Task<IActionResult> Index(string searchTerm)
        {
            var usluge = await _uslugaAppService.SearchUslugeAsync(searchTerm);
            var viewModels = usluge.Select(u => new UslugaViewModel
            {
                // Mapiranje na temelju tvojih stvarnih DTO/ViewModel propertyja
                Id = u.Id, // Korišteno 'Id' iz DTO-a
                Naziv = u.Naziv,
                Opis = u.Opis,
                Cijena = u.Cijena,
                TrajanjeMinuta = u.TrajanjeMinuta // Korišteno 'TrajanjeMinuta' iz DTO-a
            }).ToList();

            ViewBag.SearchTerm = searchTerm;
            return View(viewModels);
        }

        // GET: Usluge/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var usluga = await _uslugaAppService.GetUslugaByIdAsync(id);
            if (usluga == null) return NotFound();

            var viewModel = new UslugaViewModel
            {
                // Mapiranje na temelju tvojih stvarnih DTO/ViewModel propertyja
                Id = usluga.Id, // Korišteno 'Id' iz DTO-a
                Naziv = usluga.Naziv,
                Opis = usluga.Opis,
                Cijena = usluga.Cijena,
                TrajanjeMinuta = usluga.TrajanjeMinuta // Korišteno 'TrajanjeMinuta' iz DTO-a
            };
            return View(viewModel);
        }

        // GET: Usluge/Create
        public IActionResult Create()
        {
            return View("Edit", new UslugaViewModel()); // Koristi isti Edit view za Create
        }

        // POST: Usluge/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UslugaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", viewModel);
            }

            // Kreiramo CreateUslugaDto iz ViewModela s ISPRAVNIM nazivima propertyja
            var createDto = new CreateUslugaDto
            {
                Naziv = viewModel.Naziv,
                Opis = viewModel.Opis,
                Cijena = viewModel.Cijena,
                TrajanjeMinuta = viewModel.TrajanjeMinuta // Korišteno 'TrajanjeMinuta' iz ViewModela
            };

            try
            {
                await _uslugaAppService.CreateUslugaAsync(createDto); // Prosljeđujemo CreateUslugaDto
                TempData["SuccessMessage"] = "Usluga je uspješno kreirana!";
                return RedirectToAction(nameof(Index));
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
                return View("Edit", viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Došlo je do pogreške prilikom kreiranja usluge: {ex.Message}");
                return View("Edit", viewModel);
            }
        }

        // GET: Usluge/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var uslugaDto = await _uslugaAppService.GetUslugaByIdAsync(id);
            if (uslugaDto == null)
            {
                return NotFound();
            }

            var viewModel = new UslugaViewModel
            {
                // Mapiranje na temelju tvojih stvarnih DTO/ViewModel propertyja
                Id = uslugaDto.Id, // Korišteno 'Id' iz DTO-a
                Naziv = uslugaDto.Naziv,
                Opis = uslugaDto.Opis,
                Cijena = uslugaDto.Cijena,
                TrajanjeMinuta = uslugaDto.TrajanjeMinuta // Korišteno 'TrajanjeMinuta' iz DTO-a
            };
            return View(viewModel);
        }

        // POST: Usluge/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UslugaViewModel viewModel)
        {
            if (id != viewModel.Id) // Usporedba s 'Id' iz ViewModela
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Kreiramo UpdateUslugaDto iz ViewModela s ISPRAVNIM nazivima propertyja
            var updateDto = new UpdateUslugaDto
            {
                Id = viewModel.Id, // Korišteno 'Id' iz ViewModela
                Naziv = viewModel.Naziv,
                Opis = viewModel.Opis,
                Cijena = viewModel.Cijena,
                TrajanjeMinuta = viewModel.TrajanjeMinuta // Korišteno 'TrajanjeMinuta' iz ViewModela
            };

            try
            {
                await _uslugaAppService.UpdateUslugaAsync(updateDto); // Prosljeđujemo UpdateUslugaDto
                TempData["SuccessMessage"] = "Usluga je uspješno ažurirana!";
                return RedirectToAction(nameof(Index));
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
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Došlo je do pogreške prilikom ažuriranja usluge: {ex.Message}");
                return View(viewModel);
            }
        }

        // POST: Usluge/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _uslugaAppService.DeleteUslugaAsync(id);
                TempData["SuccessMessage"] = "Usluga je uspješno obrisana!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Došlo je do pogreške prilikom brisanja usluge: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Dohvaća cijenu usluge po ID-u. Koristi se za AJAX pozive u JavaScriptu na stranici termina.
        /// </summary>
        /// <param name="id">ID usluge.</param>
        /// <returns>JSON objekt s cijenom usluge.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUslugaPrice(int id)
        {
            var usluga = await _uslugaAppService.GetUslugaByIdAsync(id);
            if (usluga == null)
            {
                return NotFound();
            }
            return Json(new { cijena = usluga.Cijena });
        }
    }
}