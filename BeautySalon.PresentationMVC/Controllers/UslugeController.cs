using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services;
using BeautySalon.DataAccess.DbContexts;
using BeautySalon.Domain.Models;
using BeautySalon.PresentationMVC.Models;
using Microsoft.AspNetCore.Mvc;
using BeautySalon.Application.DTOs;

namespace BeautySalon.PresentationMVC.Controllers
{
    public class UslugeController : Controller
    {
        private readonly IUslugaService _service;

        public UslugeController(IUslugaService service)
        {
            _service = service;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var usluge = await _service.GetAllAsync();
        //    var viewModels = usluge.Select(MapToViewModel).ToList();
        //    return View("UslugaIndex", viewModels);
        //}

        public async Task<IActionResult> Index(string? searchTerm)
        {
            var usluge = await _service.SearchAsync(searchTerm);
            var viewModels = usluge.Select(MapToViewModel).ToList();

            var model = new UslugaSearchViewModel
            {
                SearchTerm = searchTerm,
                Usluge = viewModels
            };

            return View("UslugaIndex", model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var usluga = await _service.GetByIdAsync(id);
            if (usluga == null) return NotFound();
            return View("UslugaDetails", MapToViewModel(usluga));
        }

        public IActionResult Create() => View("UslugaCreate");

        [HttpPost]
        public async Task<IActionResult> Create(UslugaViewModel model)
        {
            if (!ModelState.IsValid) return View("UslugaCreate", model);
            await _service.AddAsync(MapToDomain(model));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var usluga = await _service.GetByIdAsync(id);
            if (usluga == null) return NotFound();
            return View("UslugaEdit", MapToViewModel(usluga));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UslugaViewModel model)
        {
            if (!ModelState.IsValid) return View("UslugaEdit", model);
            await _service.UpdateAsync(MapToDomain(model));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var usluga = await _service.GetByIdAsync(id);
            if (usluga == null) return NotFound();
            return View("UslugaDelete", MapToViewModel(usluga));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Helper metode
        private UslugaViewModel MapToViewModel(UslugaDto usluga) => new()
        {
            UslugaId = usluga.UslugaId,
            Naziv = usluga.Naziv,
            Opis = usluga.Opis,
            Trajanje = usluga.Trajanje,
            Cijena = usluga.Cijena
        };

        private UslugaDto MapToDomain(UslugaViewModel model) => new()
        {
            UslugaId = model.UslugaId,
            Naziv = model.Naziv,
            Opis = model.Opis,
            Trajanje = model.Trajanje ?? 0,
            Cijena = model.Cijena ?? 0
        };
    }
}

