using BeautySalon.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using Npgsql.EntityFrameworkCore.PostgreSQL; // Dodaj ovu liniju!

namespace BeautySalon.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BeautySalonDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Ažurirano za PostgreSQL
                services.AddDbContext<BeautySalonDbContext>(options =>
                {
                    // **Važno**: Ovdje trebaš staviti connection string za TESTNU PostgreSQL bazu.
                    // Preporučuje se korištenje SEPARATNE baze za testove kako ne bi slučajno
                    // utjecao na produkcijske ili razvojne podatke.
                    options.UseNpgsql("Host=dpg-d0kdi07fte5s738jsfn0-a.frankfurt-postgres.render.com;Port=5432;Database=beautysalondb;Username=beautysalondb_user;Password=DssCuUlm5mVyh1dnP1wH5jTi1ExqD8vD");
                });

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<BeautySalonDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

                    // Za PostgreSQL je preporučljivije koristiti migracije za kreiranje/ažuriranje sheme.
                    // EnsureDeleted() može biti sporije i opasnije za PostgreSQL.
                    // Ako želiš da testovi počinju s praznom bazom, moraš implementirati logiku brisanja podataka
                    // ili resetiranja baze, ili koristiti transakcije za svaki test.
                    // Za početak, možete koristiti EnsureCreated, ali imajte na umu ograničenja.
                    // db.Database.EnsureDeleted(); // Oprezno s ovim na PostgreSQLu!
                    db.Database.EnsureCreated(); // Kreiraj bazu ako ne postoji
                    // db.Database.Migrate(); // Idealno bi bilo koristiti migracije ako su definirane

                    try
                    {
                        SeedData.Initialize(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }
}