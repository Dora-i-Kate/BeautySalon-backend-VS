using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services;
using BeautySalon.DataAccess.DbContexts;
using BeautySalon.Domain.Models;
using BeautySalon.PresentationMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeautySalon.PresentationMVC.Controllers
{
    public class UslugeController : Controller
    {
        private readonly IUslugaService _service;

        public UslugeController(IUslugaService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var usluge = await _service.GetAllAsync();
            var viewModels = usluge.Select(MapToViewModel).ToList();
            return View("UslugaIndex", viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var usluga = await _service.GetByIdAsync(id);
            if (usluga == null) return NotFound();
            return View(MapToViewModel(usluga));
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UslugaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _service.AddAsync(MapToDomain(model));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var usluga = await _service.GetByIdAsync(id);
            if (usluga == null) return NotFound();
            return View(MapToViewModel(usluga));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UslugaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _service.UpdateAsync(MapToDomain(model));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var usluga = await _service.GetByIdAsync(id);
            if (usluga == null) return NotFound();
            return View(MapToViewModel(usluga));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Helper metode
        private UslugaViewModel MapToViewModel(Usluga usluga) => new()
        {
            UslugaId = usluga.UslugaId,
            Naziv = usluga.Naziv,
            Opis = usluga.Opis,
            Trajanje = usluga.Trajanje,
            Cijena = usluga.Cijena,
            Prikaz = usluga.Prikaz == null ? "N/A" : usluga.Prikaz
        };

        private Usluga MapToDomain(UslugaViewModel model) => new()
        {
            UslugaId = model.UslugaId,
            Naziv = model.Naziv,
            Opis = model.Opis,
            Trajanje = model.Trajanje,
            Cijena = model.Cijena,
            Prikaz = model.Prikaz
        };
    }
}

