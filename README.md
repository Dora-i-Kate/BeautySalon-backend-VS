Naziv Projekta

Ovo programsko rješenje koristi trodijelnu arhitekturu koja se sastoji od sloja za prikaz (presentation layer), poslovnog sloja (business layer) i sloja za pristup podacima (data layer). Implementirano je koristeæi .NET MVC framework uz Entity Framework Core kao ORM za rad s bazom podataka.

Arhitektura

Rješenje je organizirano prema trodijelnoj arhitekturi:

Presentation layer predstavlja sloj za prikaz koji sadrži kontrolere i prikaze (View-ove) u MVC-u.Business layer (Application) obuhvaæa poslovnu logiku sustava i sadrži servise i DTO-ove.Data layer (DataAccess) odgovoran je za pristup podacima i koristi Entity Framework Core za rad s bazom podataka.

Pokretanje aplikacije

Za pokretanje aplikacije potrebno je prvo klonirati repozitorij s GitHub-a:

git clone https://github.com/Dora-i-Kate/BeautySalon-backend-VS

Nakon kloniranja, u korijenskom direktoriju projekta potrebno je izvršiti build naredbom:

dotnet build

Prije pokretanja aplikacije potrebno je postaviti bazu podataka. U datoteci appsettings.json potrebno je unijeti connection string za PostgreSQL, a zatim izvršiti migracije kako bi se baza podataka kreirala i pripremila:

dotnet ef database update

Za pokretanje same aplikacije koristi se naredba:

dotnet run --project PresentationMVC

