using BeautySalon.Application.DTOs.Usluga;
using BeautySalon.Application.Interfaces;
using BeautySalon.Domain.Exceptions;
using BeautySalon.PresentationMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
                Id = u.Id,
                Naziv = u.Naziv,
                Opis = u.Opis,
                Cijena = u.Cijena,
                TrajanjeMinuta = u.TrajanjeMinuta
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
                Id = usluga.Id,
                Naziv = usluga.Naziv,
                Opis = usluga.Opis,
                Cijena = usluga.Cijena,
                TrajanjeMinuta = usluga.TrajanjeMinuta
            };
            return View(viewModel);
        }

        // GET: Usluge/Create
        public IActionResult Create()
        {
            return View("Edit", new UslugaViewModel());
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

            var createDto = new CreateUslugaDto
            {
                Naziv = viewModel.Naziv,
                Opis = viewModel.Opis,
                Cijena = viewModel.Cijena,
                TrajanjeMinuta = viewModel.TrajanjeMinuta
            };

            try
            {
                await _uslugaAppService.CreateUslugaAsync(createDto);
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
                Id = uslugaDto.Id,
                Naziv = uslugaDto.Naziv,
                Opis = uslugaDto.Opis,
                Cijena = uslugaDto.Cijena,
                TrajanjeMinuta = uslugaDto.TrajanjeMinuta
            };
            return View(viewModel);
        }

        // POST: Usluge/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UslugaViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var updateDto = new UpdateUslugaDto
            {
                Id = viewModel.Id,
                Naziv = viewModel.Naziv,
                Opis = viewModel.Opis,
                Cijena = viewModel.Cijena,
                TrajanjeMinuta = viewModel.TrajanjeMinuta
            };

            try
            {
                await _uslugaAppService.UpdateUslugaAsync(updateDto);
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
