using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services;
using BeautySalon.DataAccess;
using BeautySalon.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodavanje servisa u DI kontejner
builder.Services.AddControllersWithViews();

// Konfiguracija konekcije na PostgreSQL bazu podataka
builder.Services.AddDbContext<BeautySalonDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repozitoriji
builder.Services.AddScoped<IKlijentRepository, KlijentRepository>();
builder.Services.AddScoped<IZaposlenikRepository, ZaposlenikRepository>();
builder.Services.AddScoped<ITerminRepository, TerminRepository>();

// Servisi
builder.Services.AddScoped<IKlijentService, KlijentService>();
builder.Services.AddScoped<ITerminService, TerminService>();
builder.Services.AddScoped<IZaposlenikService, ZaposlenikService>();

var app = builder.Build();

// Middleware konfiguracija
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Došlo je do pogreške.");
        });
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Klijent}/{action=Detalji}/{klijentId?}"); // Default akcija: Detalji

app.Run();
