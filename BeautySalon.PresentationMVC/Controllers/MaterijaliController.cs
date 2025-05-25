using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services;
using BeautySalon.DataAccess.DbContexts;
using BeautySalon.Domain.Models;
using BeautySalon.PresentationMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BeautySalon.Application.DTOs;

namespace BeautySalon.PresentationMVC.Controllers
{
    public class MaterijaliController : Controller
    {
        private readonly IMaterijalService _service;

        public MaterijaliController(IMaterijalService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var materijali = await _service.GetAllAsync();
            var filtered = string.IsNullOrWhiteSpace(searchTerm) ? materijali : materijali.Where(m =>
                (!string.IsNullOrEmpty(m.Naziv) && m.Naziv.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(m.JedinicaMjere) && m.JedinicaMjere.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(m.VrstaNaziv) && m.VrstaNaziv.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            var viewModel = new MaterijalSearchViewModel
            {
                SearchTerm = searchTerm,
                Materijali = filtered.Select(MapToViewModel).ToList()
            };

            return View("MaterijalIndex", viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var m = await _service.GetByIdAsync(id);
            if (m == null) return NotFound();

            return View("MaterijalDetails", new MaterijalViewModel
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv,
                Cijena = m.Cijena,
                MinimalnaKolicina = m.MinimalnaKolicina,
                TrenutnaKolicina = m.TrenutnaKolicina,
                JedinicaMjere = m.JedinicaMjere,
                VrstaId = m.VrstaId,
                VrstaNaziv = m.VrstaNaziv
            });
        }

        public async Task<IActionResult> Create()
        {
            var vrste = await _service.GetAllVrsteMaterijalaAsync();
            var model = new MaterijalViewModel
            {
                VrsteMaterijala = new SelectList(vrste, "VrstaId", "Naziv")
            };
            return View("MaterijalCreate", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MaterijalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var materijal = new MaterijalDto
                {
                    Naziv = model.Naziv ?? "",
                    Cijena = model.Cijena ?? 0,
                    MinimalnaKolicina = model.MinimalnaKolicina ?? 0,
                    TrenutnaKolicina = model.TrenutnaKolicina ?? 0,
                    JedinicaMjere = model.JedinicaMjere ?? "",
                    VrstaId = model.VrstaId ?? 0
                };
                try
                {
                    await _service.CreateAsync(materijal);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var vrste = await _service.GetAllVrsteMaterijalaAsync();
            model.VrsteMaterijala = new SelectList(vrste, "VrstaId", "Naziv", model.VrstaId);
            return View("MaterijalCreate", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var m = await _service.GetByIdAsync(id);
            if (m == null) return NotFound();

            var vrste = await _service.GetAllVrsteMaterijalaAsync();
            var model = new MaterijalViewModel
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv,
                Cijena = m.Cijena,
                MinimalnaKolicina = m.MinimalnaKolicina,
                TrenutnaKolicina = m.TrenutnaKolicina,
                JedinicaMjere = m.JedinicaMjere,
                VrstaId = m.VrstaId,
                VrsteMaterijala = new SelectList(vrste, "VrstaId", "Naziv", m.VrstaId)
            };

            return View("MaterijalEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MaterijalViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(new MaterijalDto
                    {
                        MaterijalId = model.MaterijalId,
                        Naziv = model.Naziv ?? "",
                        Cijena = model.Cijena ?? 0,
                        MinimalnaKolicina = model.MinimalnaKolicina ?? 0,
                        TrenutnaKolicina = model.TrenutnaKolicina ?? 0,
                        JedinicaMjere = model.JedinicaMjere ?? "",
                        VrstaId = model.VrstaId ?? 0
                    });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var vrste = await _service.GetAllVrsteMaterijalaAsync();
            model.VrsteMaterijala = new SelectList(vrste, "VrstaId", "Naziv", model.VrstaId);
            return View("MaterijalEdit", model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var m = await _service.GetByIdAsync(id);
            if (m == null) return NotFound();

            return View("MaterijalDelete", new MaterijalViewModel
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv
            });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

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
        
    }

}

