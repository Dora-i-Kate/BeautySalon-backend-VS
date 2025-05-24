using BeautySalon.Application.DTOs;
using BeautySalon.Application.Interfaces;
using BeautySalon.PresentationMVC.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BeautySalon.PresentationMVC.Controllers
{
    public class KlijentController : Controller
    {
        private readonly IKlijentService _klijentService;
        private readonly ITerminService _terminService;
        private readonly IZaposlenikService _zaposlenikService;

        public KlijentController(
            IKlijentService klijentService,
            ITerminService terminService,
            IZaposlenikService zaposlenikService)
        {
            _klijentService = klijentService;
            _terminService = terminService;
            _zaposlenikService = zaposlenikService;
        }

        // Pomoćna metoda za postavljanje ViewBag-a za zaposlenike
        private void SetupZaposleniciViewBag(int? selectedZaposlenikId = null)
        {
            var zaposlenici = _zaposlenikService.GetAll().ToList();
            ViewBag.Zaposlenici = zaposlenici.Select(z => new SelectListItem
            {
                Value = z.KorisnikId.ToString(),
                Text = $"{z.Ime} {z.Prezime}",
                Selected = z.KorisnikId == selectedZaposlenikId
            }).ToList();
        }

        // Prikaz master-detail forme za odabranog klijenta
        public IActionResult Detalji(int klijentId)
        {
            var klijent = _klijentService.GetById(klijentId);
            if (klijent == null)
            {
                return NotFound($"Klijent s ID-em {klijentId} nije pronađen.");
            }

            var viewModel = new KlijentTerminiViewModel
            {
                Klijent = klijent,
                Termini = _terminService.GetByKlijentId(klijentId)?.ToList() ?? new List<TerminDto>(),
                Zaposlenici = _zaposlenikService.GetAll().ToList()
            };

            return View(viewModel);
        }

        // GET: Dodaj novi termin
        public IActionResult NoviTermin(int klijentId)
        {
            var klijent = _klijentService.GetById(klijentId);
            if (klijent == null)
            {
                return NotFound($"Klijent s ID-em {klijentId} nije pronađen.");
            }

            SetupZaposleniciViewBag(); // Postavi ViewBag

            var viewModel = new KlijentTerminiViewModel
            {
                Klijent = klijent,
                NoviIliIzmijenjeniTermin = new TerminDto { KlijentId = klijentId },
                Zaposlenici = _zaposlenikService.GetAll().ToList()
            };

            return View(viewModel);
        }

        // POST: Dodaj novi termin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NoviTermin(CreateTerminDto termin)
        {
            termin.Status = "Zakazan";

            if (ModelState.IsValid)
            {
                try
                {
                    _terminService.Add(termin);
                    return RedirectToAction("Detalji", new { klijentId = termin.KlijentId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Greška: {ex.Message}");
                }
            }

            var klijent = _klijentService.GetById(termin.KlijentId);
            SetupZaposleniciViewBag(termin.ZaposlenikId);

            var viewModel = new KlijentTerminiViewModel
            {
                Klijent = klijent,
                NoviIliIzmijenjeniTermin = new TerminDto
                {
                    KlijentId = termin.KlijentId,
                    DatumVrijeme = termin.DatumVrijeme,
                    Status = termin.Status,
                    ZaposlenikId = termin.ZaposlenikId,
                    KomentarZaposlenika = termin.KomentarZaposlenika
                },
                Zaposlenici = _zaposlenikService.GetAll().ToList()
            };

            return View(viewModel);
        }


        // GET: Uredi termin
        public IActionResult UrediTermin(int id)
        {
            var termin = _terminService.GetById(id);
            if (termin == null)
                return NotFound($"Termin s ID-em {id} nije pronađen.");

            SetupZaposleniciViewBag(termin.ZaposlenikId);

            var viewModel = new KlijentTerminiViewModel
            {
                NoviIliIzmijenjeniTermin = termin,
                Zaposlenici = _zaposlenikService.GetAll().ToList()
            };

            return View(viewModel);
        }


        // POST: Uredi termin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UrediTermin(TerminDto termin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _terminService.Update(termin); // Sada prima TerminDto
                    var klijentId = termin.KlijentId;
                    return RedirectToAction("Detalji", new { klijentId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Greška: {ex.Message}");
                }
            }

            SetupZaposleniciViewBag(termin.ZaposlenikId);

            var viewModel = new KlijentTerminiViewModel
            {
                NoviIliIzmijenjeniTermin = termin,
                Zaposlenici = _zaposlenikService.GetAll().ToList()
            };

            return View(viewModel);
        }




        // Obriši termin
        public IActionResult ObrisiTermin(int id, int klijentId)
        {
            try
            {
                _terminService.Delete(id); // Poziv servisa koji briše iz baze
            }
            catch (Exception ex)
            {
                // Ovdje možete dodati logiranje greške ili poruku korisniku
                TempData["ErrorMessage"] = $"Došlo je do greške prilikom brisanja: {ex.Message}";
            }
            return RedirectToAction("Detalji", new { klijentId });
        }
    }
}