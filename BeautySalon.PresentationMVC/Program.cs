using BeautySalon.Application.Interfaces;
using BeautySalon.Application.Services;
using BeautySalon.DataAccess;
using BeautySalon.DataAccess.Repositories;
using BeautySalon.Domain.Interfaces;
using BeautySalon.Domain.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodavanje servisa u DI kontejner
builder.Services.AddControllersWithViews();

// Konfiguracija konekcije na PostgreSQL bazu podataka
builder.Services.AddDbContext<BeautySalonDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// REGISTRACIJA REPOZITORIJA (DataAccess Layer)
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<ITerminRepository, TerminRepository>();
builder.Services.AddScoped<IUlogaRepository, UlogaRepository>();
builder.Services.AddScoped<IUslugaRepository, UslugaRepository>();
builder.Services.AddScoped<IMaterijalRepository, MaterijalRepository>();
builder.Services.AddScoped<IVrstaMaterijalaRepository, VrstaMaterijalaRepository>();

// REGISTRACIJA DOMENSKIH VALIDATORA (Domain Layer)
builder.Services.AddScoped<TerminValidator>();
builder.Services.AddScoped<UslugaValidator>();
builder.Services.AddScoped<MaterijalValidator>();
builder.Services.AddScoped<VrstaMaterijalaValidator>();

// REGISTRACIJA APLIKACIJSKIH SERVISA (Application Layer)
builder.Services.AddScoped<ITerminAppService, TerminAppService>();
builder.Services.AddScoped<IUslugaAppService, UslugaAppService>();
builder.Services.AddScoped<IKorisnikAppService, KorisnikAppService>();
builder.Services.AddScoped<IMaterijalAppService, MaterijalAppService>();
builder.Services.AddScoped<IVrstaMaterijalaAppService, VrstaMaterijalaAppService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "sifrarnik-usluga",
    pattern: "sifrarnik-usluga",
    defaults: new { controller = "Usluge", action = "Index" });

app.Run();
