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

// Registracija repozitorija (DataAccess Layer)
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<ITerminRepository, TerminRepository>();
builder.Services.AddScoped<IUlogaRepository, UlogaRepository>();
builder.Services.AddScoped<IUslugaRepository, UslugaRepository>();

// Registracija domenskih validatora (Domain Layer)
builder.Services.AddScoped<TerminValidator>();
builder.Services.AddScoped<UslugaValidator>();

// Registracija aplikacijskih servisa (Application Layer)
builder.Services.AddScoped<ITerminAppService, TerminAppService>();
builder.Services.AddScoped<IUslugaAppService, UslugaAppService>();
builder.Services.AddScoped<IKorisnikAppService, KorisnikAppService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
