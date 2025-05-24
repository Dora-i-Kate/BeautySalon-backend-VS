using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services;
using BeautySalon.DataAccess.DbContexts;
using BeautySalon.Domain.Models;
using BeautySalon.PresentationMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeautySalon.PresentationMVC.Controllers
{
    public class MaterijaliController : Controller
    {
        private readonly IMaterijalService _service;
        private readonly SalonDbContext _context;

        public MaterijaliController(IMaterijalService service, SalonDbContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var materijali = await _service.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
                materijali = materijali
                    .Where(m => m.Naziv.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            var viewModel = materijali.Select(m => new MaterijalViewModel
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv,
                Cijena = m.Cijena,
                MinimalnaKolicina = m.MinimalnaKolicina,
                TrenutnaKolicina = m.TrenutnaKolicina,
                JedinicaMjere = m.JedinicaMjere,
                VrstaNaziv = m.VrstaMaterijala?.Naziv
            }).ToList();

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
                VrstaNaziv = m.VrstaMaterijala?.Naziv
            });
        }

        public IActionResult Create()
        {
            var model = new MaterijalViewModel
            {
                VrsteMaterijala = new SelectList(_context.VrstaMaterijala, "VrstaId", "Naziv")
            };
            return View("MaterijalCreate", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MaterijalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var materijal = new Materijal
                {
                    Naziv = model.Naziv ?? "",
                    Cijena = model.Cijena,
                    MinimalnaKolicina = model.MinimalnaKolicina,
                    TrenutnaKolicina = model.TrenutnaKolicina,
                    JedinicaMjere = model.JedinicaMjere ?? "",
                    VrstaId = model.VrstaId
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

            model.VrsteMaterijala = new SelectList(_context.VrstaMaterijala, "VrstaId", "Naziv", model.VrstaId);
            return View("MaterijalCreate", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var m = await _service.GetByIdAsync(id);
            if (m == null) return NotFound();

            var model = new MaterijalViewModel
            {
                MaterijalId = m.MaterijalId,
                Naziv = m.Naziv,
                Cijena = m.Cijena,
                MinimalnaKolicina = m.MinimalnaKolicina,
                TrenutnaKolicina = m.TrenutnaKolicina,
                JedinicaMjere = m.JedinicaMjere,
                VrstaId = m.VrstaId,
                VrsteMaterijala = new SelectList(_context.VrstaMaterijala, "VrstaId", "Naziv", m.VrstaId)
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
                    await _service.UpdateAsync(new Materijal
                    {
                        MaterijalId = model.MaterijalId,
                        Naziv = model.Naziv ?? "",
                        Cijena = model.Cijena,
                        MinimalnaKolicina = model.MinimalnaKolicina,
                        TrenutnaKolicina = model.TrenutnaKolicina,
                        JedinicaMjere = model.JedinicaMjere ?? "",
                        VrstaId = model.VrstaId
                    });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            model.VrsteMaterijala = new SelectList(_context.VrstaMaterijala, "VrstaId", "Naziv", model.VrstaId);
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
    }

}

