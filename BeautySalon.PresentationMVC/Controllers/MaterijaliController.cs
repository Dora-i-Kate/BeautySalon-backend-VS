using BeautySalon.Application.Interfaces;
using BeautySalon.Application.DTOs.Materijal;
using BeautySalon.PresentationMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using BeautySalon.Domain.Exceptions; // Pretpostavka: Dodali ste ovu klasu za složenije validacije
using System.Collections.Generic;
using System;

namespace BeautySalon.PresentationMVC.Controllers
{
    public class MaterijaliController : Controller
    {
        private readonly IMaterijalAppService _materijalAppService;
        private readonly IVrstaMaterijalaAppService _vrstaMaterijalaAppService;

        public MaterijaliController(IMaterijalAppService materijalAppService, IVrstaMaterijalaAppService vrstaMaterijalaAppService)
        {
            _materijalAppService = materijalAppService;
            _vrstaMaterijalaAppService = vrstaMaterijalaAppService;
        }

        // Glavni zaslon šifrarnika: pregled i pretraživanje
        public async Task<IActionResult> Index(string searchTerm, int? searchVrstaId)
        {
            var materijali = await _materijalAppService.GetAllMaterijaliAsync();

            var filtered = materijali.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(m =>
                    (!string.IsNullOrEmpty(m.Naziv) && m.Naziv.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(m.JedinicaMjere) && m.JedinicaMjere.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(m.VrstaNaziv) && m.VrstaNaziv.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (searchVrstaId.HasValue && searchVrstaId.Value > 0)
            {
                filtered = filtered.Where(m => m.VrstaId == searchVrstaId.Value);
            }

            var viewModel = new MaterijalSearchViewModel
            {
                SearchTerm = searchTerm,
                SearchVrstaId = searchVrstaId,
                Materijali = filtered.Select(MapToViewModel).ToList()
            };

            // Popunjavamo SelectList za pretragu po vrsti
            await PopulateVrsteMaterijalaDropdownForSearch(viewModel);

            return View(viewModel);
        }

        // GET: Materijali/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var materijalDto = await _materijalAppService.GetMaterijalByIdAsync(id);
            if (materijalDto == null) return NotFound();

            return View(MapToViewModel(materijalDto));
        }

        // GET: Materijali/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new MaterijalViewModel();
            await PopulateVrsteMaterijalaDropdown(viewModel);
            return View(viewModel);
        }

        // POST: Materijali/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaterijalViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateVrsteMaterijalaDropdown(viewModel);
                return View(viewModel);
            }

            var createMaterijalDto = new CreateMaterijalDto
            {
                Naziv = viewModel.Naziv,
                Cijena = viewModel.Cijena,
                MinimalnaKolicina = viewModel.MinimalnaKolicina,
                TrenutnaKolicina = viewModel.TrenutnaKolicina,
                JedinicaMjere = viewModel.JedinicaMjere,
                VrstaId = viewModel.VrstaId
            };

            try
            {
                await _materijalAppService.CreateMaterijalAsync(createMaterijalDto);
                TempData["SuccessMessage"] = "Materijal uspješno kreiran.";
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
                await PopulateVrsteMaterijalaDropdown(viewModel);
                TempData["ErrorMessage"] = "Greška pri kreiranju materijala: Provjerite unesene podatke.";
                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Neočekivana greška pri kreiranju materijala: {ex.Message}");
                await PopulateVrsteMaterijalaDropdown(viewModel);
                TempData["ErrorMessage"] = "Neočekivana greška pri kreiranju materijala.";
                return View(viewModel);
            }
        }

        // GET: Materijali/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var materijalDto = await _materijalAppService.GetMaterijalByIdAsync(id);
            if (materijalDto == null) return NotFound();

            var viewModel = MapToViewModel(materijalDto);
            await PopulateVrsteMaterijalaDropdown(viewModel);
            return View(viewModel);
        }

        // POST: Materijali/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MaterijalViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateVrsteMaterijalaDropdown(viewModel);
                return View(viewModel);
            }

            var updateMaterijalDto = new UpdateMaterijalDto
            {
                MaterijalId = viewModel.MaterijalId,
                Naziv = viewModel.Naziv,
                Cijena = viewModel.Cijena,
                MinimalnaKolicina = viewModel.MinimalnaKolicina,
                TrenutnaKolicina = viewModel.TrenutnaKolicina,
                JedinicaMjere = viewModel.JedinicaMjere,
                VrstaId = viewModel.VrstaId
            };

            try
            {
                await _materijalAppService.UpdateMaterijalAsync(updateMaterijalDto);
                TempData["SuccessMessage"] = "Materijal uspješno ažuriran.";
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
                await PopulateVrsteMaterijalaDropdown(viewModel);
                TempData["ErrorMessage"] = "Greška pri ažuriranju materijala: Provjerite unesene podatke.";
                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Neočekivana greška pri ažuriranju materijala: {ex.Message}");
                await PopulateVrsteMaterijalaDropdown(viewModel);
                TempData["ErrorMessage"] = "Neočekivana greška pri ažuriranju materijala.";
                return View(viewModel);
            }
        }

        // GET: Materijali/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var materijalDto = await _materijalAppService.GetMaterijalByIdAsync(id);
            if (materijalDto == null)
            {
                return NotFound();
            }
            return View(MapToViewModel(materijalDto));
        }

        // POST: Materijali/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _materijalAppService.DeleteMaterijalAsync(id);
                TempData["SuccessMessage"] = "Materijal uspješno obrisan.";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Greška pri brisanju materijala: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper metoda za mapiranje MaterijalDto u MaterijalViewModel
        private MaterijalViewModel MapToViewModel(MaterijalDto m) => new()
        {
            MaterijalId = m.MaterijalId,
            Naziv = m.Naziv,
            Cijena = m.Cijena,
            MinimalnaKolicina = m.MinimalnaKolicina,
            TrenutnaKolicina = m.TrenutnaKolicina,
            JedinicaMjere = m.JedinicaMjere,
            VrstaId = m.VrstaId,
            VrstaNaziv = m.VrstaNaziv
        };

        // Helper metoda za popunjavanje SelectListe za MaterijalViewModel (Create/Edit)
        private async Task PopulateVrsteMaterijalaDropdown(MaterijalViewModel viewModel)
        {
            var vrste = await _vrstaMaterijalaAppService.GetAllVrsteMaterijalaAsync();
            // ISPRAVLJENO: Koristimo "VrstaId" jer je to naziv ID svojstva u VrstaMaterijalaDto
            var vrsteListItems = vrste.Select(v => new SelectListItem { Value = v.VrstaId.ToString(), Text = v.Naziv }).ToList();
            vrsteListItems.Insert(0, new SelectListItem { Value = "", Text = "-- Odaberite vrstu materijala --" });
            viewModel.VrsteMaterijala = new SelectList(vrsteListItems, "Value", "Text", viewModel.VrstaId);
        }

        // Helper metoda za popunjavanje SelectListe za MaterijalSearchViewModel (Index filter)
        private async Task PopulateVrsteMaterijalaDropdownForSearch(MaterijalSearchViewModel viewModel)
        {
            var vrste = await _vrstaMaterijalaAppService.GetAllVrsteMaterijalaAsync();
            // ISPRAVLJENO: Koristimo "VrstaId" jer je to naziv ID svojstva u VrstaMaterijalaDto
            var vrsteListItems = vrste.Select(v => new SelectListItem { Value = v.VrstaId.ToString(), Text = v.Naziv }).ToList();
            vrsteListItems.Insert(0, new SelectListItem { Value = "", Text = "-- Sve vrste materijala --" });
            viewModel.VrsteMaterijala = new SelectList(vrsteListItems, "Value", "Text", viewModel.SearchVrstaId);
        }
    }
}