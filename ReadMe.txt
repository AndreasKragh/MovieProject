
Group members: Andreas Kragh & Andreas Juul
Contribution: We chose to continued working on Andreas Krags project and shared it through gitlab. 
Almost everything were made when we sat together but the first attempt of a entity was made by Juul and the first controler Kragh.
We did not make the seeddata ourselves but Claude handeled that for us.

How to run the application:


Information about the database setup/migrations:


OpenAPI JSON URL: http://localhost:5258/openapi/v1.json
Scalar UI URL: http://localhost:5258/scalar/
API base URL : http://localhost:5258 

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- The EF Core command-line tool:

  dotnet tool install --global dotnet-ef
  

## Running the application

1. Start SQL Server (Docker)
Run "docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong!Passw0rd>" -p 1433:1433 --name sql1 -d mcr.microsoft.com/mssql/server:2022-latest"
Choose your own password - If the container already exists, start it with `docker start sql1`.

2. Start MongoDB (Docker)
Run "docker run -d --name movievault-mongo -p 27017:27017 -v movievault-mongo-data:/data/db mongo"
If the container already exists, start it with `docker start movievault-mongo`.

3. Configure the connection string
Open `appsettings.json` and replace the password placeholder in `ConnectionStrings:DefaultConnection`:
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=MovieVault;User Id=sa;Password=<YOUR_SA_PASSWORD>;TrustServerCertificate=True"
}
The password must match `MSSQL_SA_PASSWORD` from step 1.
The MongoDB connection is configured in the `Serilog` section of `appsettings.json` (`databaseUrl` and `collectionName`). 
The default (`mongodb://localhost:27017/MovieVaultLogs`) works with the container from step 2.

4. Create the database and seed data
Run "dotnet ef database update"
This creates the database `MovieVault` and applies all migrations including the seed data.

5. Run the API
Run "dotnet run"
Wait until the console shows `Now listening on: http://localhost:5258`, then open Scalar.

Database setup and migrations

The database is built **code-first** from the EF Core model, which implements the consolidated E/R diagram.

Entities: `Entities/` (one class per table, including the join entities `MovieCast` and `FilmedAt` for the N:M relationships).
DbContext: `Data/MovieVaultContext.cs` (relations, composite keys, delete behaviour, decimal precision and check constraints are configured in `OnModelCreating`).
Migrations: `Migrations/` (version history of the schema, applied with `dotnet ef database update`).
Seed data: `Data/SeedData.cs`, registered with `modelBuilder.Seed()` in the context. It is inserted by a migration, so a fresh database always contains the same data.
The data includes an actor in several movies, movies with several actors, a movie without production record, movies without screenings, and screenings of the same movie in different cinemas.



