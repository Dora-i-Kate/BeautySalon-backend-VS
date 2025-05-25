using BeautySalon.Application.DTOs.Termin; // Provjeri da su TerminViewModel i StavkaTerminaViewModel stvarno ovdje
using BeautySalon.Domain.Models;
using BeautySalon.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Browser; // <-- ISPRAVLJENO: Bilo je AngleSharp.Browse, sada je AngleSharp.Browse
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using BeautySalon.PresentationMVC.Controllers; // Za referenciranje kontrolera
using BeautySalon.PresentationMVC.ViewModels; // Dodaj ako su ti View modeli ovdje

namespace BeautySalon.Tests
{
    // Koristimo HomeController kao TProgram umjesto BeautySalon.PresentationMVC.Program
    // Pretpostavka: Imate HomeController u BeautySalon.PresentationMVC.Controllers
    // Ako nemate, pronađite bilo koji drugi Controller ili ViewModel npr. BeautySalon.PresentationMVC.ViewModels.UslugaViewModel
    public class TerminiIntegrationTests : IClassFixture<CustomWebApplicationFactory<BeautySalon.PresentationMVC.Controllers.HomeController>>
    {
        private readonly HttpClient _client;
        // Tip factory-a mora odgovarati tipu u IClassFixture
        private readonly CustomWebApplicationFactory<BeautySalon.PresentationMVC.Controllers.HomeController> _factory;

        // Konstruktor mora odgovarati tipu factory-a
        public TerminiIntegrationTests(CustomWebApplicationFactory<BeautySalon.PresentationMVC.Controllers.HomeController> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        // --- Testovi za GET akcije ---

        [Fact]
        public async Task Index_ReturnsSuccessAndCorrectContentType()
        {
            var response = await _client.GetAsync("/Termini");

            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var html = await response.Content.ReadAsStringAsync();
            // ISPRAVLJENO: Koristimo new BrowseContext()
            var document = await new BrowsingContext().OpenAsync(req => req.Content(html));
            Assert.NotNull(document.QuerySelector("#terminiTable"));
            Assert.Contains("Šišanje", html);
            Assert.Contains("Boja", html);
        }

        [Fact]
        public async Task Details_ExistingTermin_ReturnsSuccessAndCorrectContentType()
        {
            int terminId;
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BeautySalonDbContext>();
                terminId = dbContext.Termini.Include(t => t.StavkeTermina).First(t => t.StavkeTermina.Any()).Id;
            }

            var response = await _client.GetAsync($"/Termini/Details/{terminId}");

            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var html = await response.Content.ReadAsStringAsync();
            // ISPRAVLJENO: Koristimo new BrowseContext()
            var document = await new BrowsingContext().OpenAsync(req => req.Content(html));

            Assert.NotNull(document.QuerySelector($"input[name='Id'][value='{terminId}']"));
            Assert.NotNull(document.QuerySelector("input[name='Datum']"));
            Assert.NotNull(document.QuerySelector("select[name='KlijentId']"));
            Assert.NotNull(document.QuerySelector("select[name='ZaposlenikId']"));
            Assert.Contains("Šišanje", html);
        }

        [Fact]
        public async Task Details_NonExistingTermin_ReturnsNotFound()
        {
            int nonExistingTerminId = 999999;

            var response = await _client.GetAsync($"/Termini/Details/{nonExistingTerminId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_Get_ReturnsSuccessAndCorrectContentType()
        {
            var response = await _client.GetAsync("/Termini/Create");

            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());

            var html = await response.Content.ReadAsStringAsync();
            // ISPRAVLJENO: Koristimo new BrowseContext()
            var document = await new BrowsingContext().OpenAsync(req => req.Content(html));

            Assert.NotNull(document.QuerySelector("form[asp-action='Create']"));
            Assert.NotNull(document.QuerySelector("input[name='Datum']"));
            Assert.NotNull(document.QuerySelector("select[name='KlijentId']"));
        }

        // --- Testovi za POST akcije (Create, Edit, Delete) ---

        [Fact]
        public async Task Create_Post_ValidTermin_RedirectsToDetails()
        {
            int klijentId;
            int zaposlenikId;
            int uslugaId;
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BeautySalonDbContext>();
                klijentId = dbContext.Korisnici.First(k => k.Uloga.NazivUloge == UlogaNaziv.Klijent.ToString()).Id;
                zaposlenikId = dbContext.Korisnici.First(k => k.Uloga.NazivUloge == UlogaNaziv.Zaposlenik.ToString()).Id;
                uslugaId = dbContext.Usluge.First().Id;
            }

            // TerminViewModel mora biti dostupan kroz using direktivu
            var newTermin = new TerminViewModel
            {
                Datum = DateTime.Today.AddDays(7),
                Vrijeme = new TimeSpan(11, 0, 0),
                TrajanjeMinuta = 60,
                Status = TerminStatus.Zakazan,
                KlijentId = klijentId,
                ZaposlenikId = zaposlenikId,
                StavkeTermina = new List<StavkaTerminaViewModel>
                {
                    new StavkaTerminaViewModel { UslugaId = uslugaId, Kolicina = 1, Cijena = 50.00m }
                }
            };

            var antiForgeryToken = await GetAntiForgeryToken(_client, "/Termini/Create");

            var formContent = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", antiForgeryToken },
                { "Datum", newTermin.Datum.ToString("yyyy-MM-dd") },
                { "Vrijeme", newTermin.Vrijeme.ToString(@"hh\:mm") },
                { "TrajanjeMinuta", newTermin.TrajanjeMinuta.ToString() },
                { "Status", ((int)newTermin.Status).ToString() },
                { "KlijentId", newTermin.KlijentId.ToString() },
                { "ZaposlenikId", newTermin.ZaposlenikId.ToString() }
            };

            for (int i = 0; i < newTermin.StavkeTermina.Count; i++)
            {
                formContent.Add($"StavkeTermina[{i}].UslugaId", newTermin.StavkeTermina[i].UslugaId.ToString());
                formContent.Add($"StavkeTermina[{i}].Kolicina", newTermin.StavkeTermina[i].Kolicina.ToString());
                formContent.Add($"StavkeTermina[{i}].Cijena", newTermin.StavkeTermina[i].Cijena.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            var requestContent = new FormUrlEncodedContent(formContent);

            var response = await _client.PostAsync("/Termini/Create", requestContent);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("/Termini/Details/", response.Headers.Location?.OriginalString);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BeautySalonDbContext>();
                var createdTermin = await dbContext.Termini
                    .Include(t => t.StavkeTermina)
                    .OrderByDescending(t => t.Id)
                    .FirstOrDefaultAsync();

                Assert.NotNull(createdTermin);
                Assert.Equal(newTermin.Datum.Date, createdTermin.Datum.Date);
                Assert.Equal(newTermin.Vrijeme, createdTermin.Vrijeme);
                Assert.Equal(newTermin.KlijentId, createdTermin.KlijentId);
                Assert.Single(createdTermin.StavkeTermina);

                var firstStavka = createdTermin.StavkeTermina.First();
                Assert.Equal(newTermin.StavkeTermina[0].UslugaId, firstStavka.UslugaId);
                Assert.Equal(newTermin.StavkeTermina[0].Kolicina, firstStavka.Kolicina);
                Assert.Equal(newTermin.StavkeTermina[0].Cijena, firstStavka.Cijena);
            }
        }

        [Fact]
        public async Task Create_Post_InvalidTermin_ReturnsViewWithErrors()
        {
            var invalidTermin = new TerminViewModel
            {
                Datum = DateTime.Today.AddDays(-1),
                Vrijeme = new TimeSpan(8, 0, 0),
                TrajanjeMinuta = 0,
            };

            var antiForgeryToken = await GetAntiForgeryToken(_client, "/Termini/Create");

            var formContent = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", antiForgeryToken },
                { "Datum", invalidTermin.Datum.ToString("yyyy-MM-dd") },
                { "Vrijeme", invalidTermin.Vrijeme.ToString(@"hh\:mm") },
                { "TrajanjeMinuta", invalidTermin.TrajanjeMinuta.ToString() },
                { "Status", ((int)TerminStatus.Zakazan).ToString() }
            };

            var requestContent = new FormUrlEncodedContent(formContent);

            var response = await _client.PostAsync("/Termini/Create", requestContent);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var html = await response.Content.ReadAsStringAsync();
            // ISPRAVLJENO: Koristimo new BrowseContext()
            var document = await new BrowsingContext().OpenAsync(req => req.Content(html));

            Assert.Contains("Klijent je obavezan.", html);
            Assert.Contains("Zaposlenik je obavezan.", html);
            Assert.Contains("Trajanje mora biti barem 1 minuta", html);
            Assert.Contains("Nijedna usluga nije odabrana.", html);
        }

        [Fact]
        public async Task Edit_Post_ValidTermin_RedirectsToDetails()
        {
            int terminId;
            int existingKlijentId;
            int existingZaposlenikId;
            int existingUslugaId;
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BeautySalonDbContext>();
                var terminToEdit = await dbContext.Termini
                                                   .Include(t => t.StavkeTermina)
                                                   .FirstAsync();
                terminId = terminToEdit.Id;
                existingKlijentId = terminToEdit.KlijentId;
                existingZaposlenikId = terminToEdit.ZaposlenikId;
                existingUslugaId = dbContext.Usluge.First(u => u.Naziv == "Manikura").Id;
            }

            var updatedTermin = new TerminViewModel
            {
                Id = terminId,
                Datum = DateTime.Today.AddDays(2),
                Vrijeme = new TimeSpan(14, 0, 0),
                TrajanjeMinuta = 120,
                Status = TerminStatus.Završen,
                KlijentId = existingKlijentId,
                ZaposlenikId = existingZaposlenikId,
                StavkeTermina = new List<StavkaTerminaViewModel>
                {
                    new StavkaTerminaViewModel { Id = 0, UslugaId = existingUslugaId, Kolicina = 2, Cijena = 80.00m }
                }
            };

            var antiForgeryToken = await GetAntiForgeryToken(_client, $"/Termini/Edit/{terminId}");

            var formContent = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", antiForgeryToken },
                { "Id", updatedTermin.Id.ToString() },
                { "Datum", updatedTermin.Datum.ToString("yyyy-MM-dd") },
                { "Vrijeme", updatedTermin.Vrijeme.ToString(@"hh\:mm") },
                { "TrajanjeMinuta", updatedTermin.TrajanjeMinuta.ToString() },
                { "Status", ((int)updatedTermin.Status).ToString() },
                { "KlijentId", updatedTermin.KlijentId.ToString() },
                { "ZaposlenikId", updatedTermin.ZaposlenikId.ToString() }
            };

            for (int i = 0; i < updatedTermin.StavkeTermina.Count; i++)
            {
                formContent.Add($"StavkeTermina[{i}].Id", updatedTermin.StavkeTermina[i].Id.ToString());
                formContent.Add($"StavkeTermina[{i}].UslugaId", updatedTermin.StavkeTermina[i].UslugaId.ToString());
                formContent.Add($"StavkeTermina[{i}].Kolicina", updatedTermin.StavkeTermina[i].Kolicina.ToString());
                formContent.Add($"StavkeTermina[{i}].Cijena", updatedTermin.StavkeTermina[i].Cijena.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            var requestContent = new FormUrlEncodedContent(formContent);

            var response = await _client.PostAsync($"/Termini/Edit/{terminId}", requestContent);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith($"/Termini/Details/{terminId}", response.Headers.Location?.OriginalString);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BeautySalonDbContext>();
                var editedTermin = await dbContext.Termini
                                                   .Include(t => t.StavkeTermina)
                                                   .FirstOrDefaultAsync(t => t.Id == terminId);

                Assert.NotNull(editedTermin);
                Assert.Equal(updatedTermin.Datum.Date, editedTermin.Datum.Date);
                Assert.Equal(updatedTermin.TrajanjeMinuta, editedTermin.TrajanjeMinuta);
                Assert.Equal(updatedTermin.Status, editedTermin.Status);
                Assert.Contains(editedTermin.StavkeTermina, st => st.UslugaId == existingUslugaId && st.Kolicina == 2);
            }
        }

        [Fact]
        public async Task Delete_Post_ExistingTermin_RedirectsToIndex()
        {
            int terminIdToDelete;
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BeautySalonDbContext>();
                var klijent = dbContext.Korisnici.First(k => k.Uloga.NazivUloge == UlogaNaziv.Klijent.ToString());
                var zaposlenik = dbContext.Korisnici.First(k => k.Uloga.NazivUloge == UlogaNaziv.Zaposlenik.ToString());
                var usluga = dbContext.Usluge.First();

                var newTermin = new Termin(DateTime.Today.AddDays(5), new TimeSpan(9, 0, 0), usluga.TrajanjeMinuta, klijent.Id, zaposlenik.Id);
                newTermin.AddStavka(usluga.Id, 1, usluga.Cijena);
                dbContext.Termini.Add(newTermin);
                await dbContext.SaveChangesAsync();
                terminIdToDelete = newTermin.Id;
            }

            var antiForgeryToken = await GetAntiForgeryToken(_client, $"/Termini/Delete/{terminIdToDelete}");

            var formContent = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", antiForgeryToken }
            };
            var requestContent = new FormUrlEncodedContent(formContent);

            var response = await _client.PostAsync($"/Termini/Delete/{terminIdToDelete}", requestContent);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Termini", response.Headers.Location?.OriginalString);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BeautySalonDbContext>();
                var deletedTermin = await dbContext.Termini.FindAsync(terminIdToDelete);
                Assert.Null(deletedTermin);
            }
        }

        // --- Pomoćne metode ---

        private async Task<string> GetAntiForgeryToken(HttpClient client, string requestUrl)
        {
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            // ISPRAVLJENO: Koristimo new BrowseContext()
            var document = await new BrowsingContext().OpenAsync(req => req.Content(html));

            var tokenElement = document.QuerySelector("input[name='__RequestVerificationToken']");
            return tokenElement?.GetAttribute("value") ?? throw new InvalidOperationException($"Anti-forgery token not found on {requestUrl}. Check if the view contains <input type=\"hidden\" name=\"__RequestVerificationToken\" />.");
        }
    }
}