using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Data;

public static class SeedData
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // ---------- Directors (4) ----------
        modelBuilder.Entity<Director>().HasData(
            new Director { DirectorId = 1, Name = "Anna Kristensen", BirthDate = new DateOnly(1975, 8, 14), Nationality = "Danish" },
            new Director { DirectorId = 2, Name = "Marcus Webb", BirthDate = new DateOnly(1969, 3, 27), Nationality = "American" },
            new Director { DirectorId = 3, Name = "Elise Fontaine", BirthDate = new DateOnly(1982, 10, 5), Nationality = "French" },
            new Director { DirectorId = 4, Name = "Rajesh Nair", BirthDate = new DateOnly(1980, 1, 19), Nationality = "Indian" }
        );

        // ---------- Production companies (4) ----------
        modelBuilder.Entity<ProductionCompany>().HasData(
            new ProductionCompany { ProductionCompanyId = 1, Name = "Nordic Lights Pictures", FoundedYear = 1998, Country = "Denmark" },
            new ProductionCompany { ProductionCompanyId = 2, Name = "Silverline Studios", FoundedYear = 1985, Country = "United States" },
            new ProductionCompany { ProductionCompanyId = 3, Name = "Aurora Film Group", FoundedYear = 2005, Country = "Sweden" },
            new ProductionCompany { ProductionCompanyId = 4, Name = "Blue Meridian Productions", FoundedYear = 2012, Country = "United Kingdom" }
        );

        // ---------- Studios (5) ----------
        modelBuilder.Entity<Studio>().HasData(
            new Studio { StudioId = 1, Name = "Nordic Lights Studio Copenhagen", City = "Copenhagen", Country = "Denmark", NumberOfSoundStages = 6, Capacity = 400, CompanyId = 1 },
            new Studio { StudioId = 2, Name = "Silverline Burbank Lot", City = "Los Angeles", Country = "United States", NumberOfSoundStages = 12, Capacity = 1200, CompanyId = 2 },
            new Studio { StudioId = 3, Name = "Silverline Atlanta Stages", City = "Atlanta", Country = "United States", NumberOfSoundStages = 8, Capacity = 800, CompanyId = 2 },
            new Studio { StudioId = 4, Name = "Aurora Stockholm Studios", City = "Stockholm", Country = "Sweden", NumberOfSoundStages = 5, Capacity = 350, CompanyId = 3 },
            new Studio { StudioId = 5, Name = "Blue Meridian London Studios", City = "London", Country = "United Kingdom", NumberOfSoundStages = 7, Capacity = 600, CompanyId = 4 }
        );

        // ---------- Locations (6) ----------
        modelBuilder.Entity<Location>().HasData(
            new Location { LocationId = 1, Name = "Nyhavn Waterfront", City = "Copenhagen", Country = "Denmark", LocationType = "street" },
            new Location { LocationId = 2, Name = "Old Customs House", City = "Aarhus", Country = "Denmark", LocationType = "building" },
            new Location { LocationId = 3, Name = "Lofoten Coastline", City = "Svolvaer", Country = "Norway", LocationType = "landscape" },
            new Location { LocationId = 4, Name = "Soundstage 4", City = "Los Angeles", Country = "United States", LocationType = "set" },
            new Location { LocationId = 5, Name = "Rue des Lanternes", City = "Paris", Country = "France", LocationType = "street" },
            new Location { LocationId = 6, Name = "Icefjord Plateau", City = "Ilulissat", Country = "Greenland", LocationType = "landscape" }
        );

        // ---------- Movies (6) ----------
        // Film 5 er "in production" og film 6 er "planned": de har ingen screenings.
        // Film 6 har heller ingen ProductionRecord (valgfri 1:1).
        modelBuilder.Entity<Movie>().HasData(
            new Movie { MovieId = 1, Title = "Midnight Harbor", Runtime = 118, ReleaseDate = new DateOnly(2024, 3, 15), AgeRating = "PG-13", MovieStatus = "released", DirectorId = 1, ProductionCompanyId = 1 },
            new Movie { MovieId = 2, Title = "The Glass Orchard", Runtime = 132, ReleaseDate = new DateOnly(2025, 6, 20), AgeRating = "R", MovieStatus = "released", DirectorId = 2, ProductionCompanyId = 2 },
            new Movie { MovieId = 3, Title = "Echoes of Tundra", Runtime = 105, ReleaseDate = new DateOnly(2025, 11, 7), AgeRating = "PG", MovieStatus = "released", DirectorId = 3, ProductionCompanyId = 1 },
            new Movie { MovieId = 4, Title = "Neon Requiem", Runtime = 126, ReleaseDate = new DateOnly(2026, 10, 2), AgeRating = "R", MovieStatus = "released", DirectorId = 1, ProductionCompanyId = 3 },
            new Movie { MovieId = 5, Title = "Paper Kingdoms", Runtime = 111, ReleaseDate = new DateOnly(2027, 2, 14), AgeRating = "PG-13", MovieStatus = "in production", DirectorId = 4, ProductionCompanyId = 4 },
            new Movie { MovieId = 6, Title = "The Last Lighthouse", Runtime = 120, ReleaseDate = new DateOnly(2027, 8, 1), AgeRating = "PG-13", MovieStatus = "planned", DirectorId = 2, ProductionCompanyId = 2 }
        );

        // ---------- Production records (5, 1:1 med Movie) ----------
        modelBuilder.Entity<ProductionRecord>().HasData(
            new ProductionRecord { ProductionRecordId = 1, MovieId = 1, Budget = 45_000_000m, ProductionStartDate = new DateOnly(2023, 1, 10), ProductionEndDate = new DateOnly(2023, 9, 30) },
            new ProductionRecord { ProductionRecordId = 2, MovieId = 2, Budget = 80_000_000m, ProductionStartDate = new DateOnly(2024, 2, 1), ProductionEndDate = new DateOnly(2024, 12, 15) },
            new ProductionRecord { ProductionRecordId = 3, MovieId = 3, Budget = 28_000_000m, ProductionStartDate = new DateOnly(2024, 5, 5), ProductionEndDate = new DateOnly(2025, 2, 28) },
            new ProductionRecord { ProductionRecordId = 4, MovieId = 4, Budget = 62_500_000m, ProductionStartDate = new DateOnly(2025, 3, 1), ProductionEndDate = new DateOnly(2026, 1, 31) },
            new ProductionRecord { ProductionRecordId = 5, MovieId = 5, Budget = 35_000_000m, ProductionStartDate = new DateOnly(2026, 3, 15), ProductionEndDate = new DateOnly(2026, 11, 30) }
        );

        // ---------- Actors (10) ----------
        modelBuilder.Entity<Actor>().HasData(
            new Actor { ActorId = 1, Name = "Jonas Beck", BirthDate = new DateOnly(1978, 4, 12), Nationality = "Danish", Gender = "Male" },
            new Actor { ActorId = 2, Name = "Emma Larsson", BirthDate = new DateOnly(1990, 9, 3), Nationality = "Swedish", Gender = "Female" },
            new Actor { ActorId = 3, Name = "Henrik Dahl", BirthDate = new DateOnly(1965, 1, 22), Nationality = "Norwegian", Gender = "Male" },
            new Actor { ActorId = 4, Name = "Sofia Marino", BirthDate = new DateOnly(1985, 7, 30), Nationality = "Italian", Gender = "Female" },
            new Actor { ActorId = 5, Name = "Margaret Ellis", BirthDate = new DateOnly(1958, 11, 8), Nationality = "British", Gender = "Female" },
            new Actor { ActorId = 6, Name = "Aino Virtanen", BirthDate = new DateOnly(1995, 2, 17), Nationality = "Finnish", Gender = "Female" },
            new Actor { ActorId = 7, Name = "Lucas Moreau", BirthDate = new DateOnly(1992, 6, 25), Nationality = "French", Gender = "Male" },
            new Actor { ActorId = 8, Name = "Daniel Okafor", BirthDate = new DateOnly(1988, 12, 1), Nationality = "Nigerian", Gender = "Male" },
            new Actor { ActorId = 9, Name = "Peter Holm", BirthDate = new DateOnly(1950, 5, 19), Nationality = "Danish", Gender = "Male" },
            new Actor { ActorId = 10, Name = "Yuki Tanaka", BirthDate = new DateOnly(1999, 3, 9), Nationality = "Japanese", Gender = "Female" }
        );

        // ---------- MovieCast (N:M Movie <-> Actor) ----------
        // Emma Larsson (2) er med i film 1, 2 og 4. Hver film har flere skuespillere.
        modelBuilder.Entity<MovieCast>().HasData(
            // Midnight Harbor
            new MovieCast { MovieId = 1, ActorId = 1, CharacterName = "Captain Elias Rowe", RoleType = "lead" },
            new MovieCast { MovieId = 1, ActorId = 2, CharacterName = "Mara Quinn", RoleType = "lead" },
            new MovieCast { MovieId = 1, ActorId = 3, CharacterName = "Dockmaster Hale", RoleType = "supporting" },
            new MovieCast { MovieId = 1, ActorId = 9, CharacterName = "Old Fisherman", RoleType = "cameo" },
            // The Glass Orchard
            new MovieCast { MovieId = 2, ActorId = 2, CharacterName = "Clara Voss", RoleType = "lead" },
            new MovieCast { MovieId = 2, ActorId = 4, CharacterName = "Detective Anton Brandt", RoleType = "lead" },
            new MovieCast { MovieId = 2, ActorId = 5, CharacterName = "Mrs. Lindqvist", RoleType = "supporting" },
            new MovieCast { MovieId = 2, ActorId = 1, CharacterName = "Viktor", RoleType = "supporting" },
            // Echoes of Tundra
            new MovieCast { MovieId = 3, ActorId = 6, CharacterName = "Aino", RoleType = "lead" },
            new MovieCast { MovieId = 3, ActorId = 7, CharacterName = "Kasper", RoleType = "lead" },
            new MovieCast { MovieId = 3, ActorId = 3, CharacterName = "Professor Lund", RoleType = "supporting" },
            new MovieCast { MovieId = 3, ActorId = 10, CharacterName = "Research Student", RoleType = "cameo" },
            // Neon Requiem
            new MovieCast { MovieId = 4, ActorId = 4, CharacterName = "Vera Santoro", RoleType = "lead" },
            new MovieCast { MovieId = 4, ActorId = 8, CharacterName = "Jax Mercer", RoleType = "lead" },
            new MovieCast { MovieId = 4, ActorId = 5, CharacterName = "The Collector", RoleType = "supporting" },
            new MovieCast { MovieId = 4, ActorId = 2, CharacterName = "Nightclub Singer", RoleType = "cameo" },
            // Paper Kingdoms
            new MovieCast { MovieId = 5, ActorId = 6, CharacterName = "Princess Lena", RoleType = "lead" },
            new MovieCast { MovieId = 5, ActorId = 7, CharacterName = "The Cartographer", RoleType = "supporting" },
            new MovieCast { MovieId = 5, ActorId = 8, CharacterName = "General Oyo", RoleType = "supporting" },
            // The Last Lighthouse
            new MovieCast { MovieId = 6, ActorId = 1, CharacterName = "Keeper Anders", RoleType = "lead" },
            new MovieCast { MovieId = 6, ActorId = 9, CharacterName = "Harbor Master", RoleType = "supporting" }
        );

        // ---------- FilmedAt (N:M Movie <-> Location) ----------
        // Location 4 (Soundstage 4) bruges af film 1, 2 og 4.
        modelBuilder.Entity<FilmedAt>().HasData(
            new FilmedAt { MovieId = 1, LocationId = 1, NumberOfShootingDays = 10 },
            new FilmedAt { MovieId = 1, LocationId = 2, NumberOfShootingDays = 6 },
            new FilmedAt { MovieId = 1, LocationId = 4, NumberOfShootingDays = 14 },
            new FilmedAt { MovieId = 2, LocationId = 5, NumberOfShootingDays = 8 },
            new FilmedAt { MovieId = 2, LocationId = 4, NumberOfShootingDays = 20 },
            new FilmedAt { MovieId = 3, LocationId = 3, NumberOfShootingDays = 12 },
            new FilmedAt { MovieId = 3, LocationId = 6, NumberOfShootingDays = 9 },
            new FilmedAt { MovieId = 4, LocationId = 1, NumberOfShootingDays = 5 },
            new FilmedAt { MovieId = 4, LocationId = 5, NumberOfShootingDays = 7 },
            new FilmedAt { MovieId = 4, LocationId = 4, NumberOfShootingDays = 18 },
            new FilmedAt { MovieId = 5, LocationId = 2, NumberOfShootingDays = 4 },
            new FilmedAt { MovieId = 5, LocationId = 3, NumberOfShootingDays = 6 }
        );

        // ---------- Cinemas (4) ----------
        modelBuilder.Entity<Cinema>().HasData(
            new Cinema { CinemaId = 1, Name = "Grand Biograf", Address = "Axeltorv 2", City = "Copenhagen", OpeningYear = 1912 },
            new Cinema { CinemaId = 2, Name = "Harbour Lights Cinema", Address = "Havnegade 10", City = "Aarhus", OpeningYear = 1998 },
            new Cinema { CinemaId = 3, Name = "Odense Filmpalads", Address = "Vestergade 5", City = "Odense", OpeningYear = 2005 },
            new Cinema { CinemaId = 4, Name = "Aalborg Cineplex", Address = "Jyllandsgade 20", City = "Aalborg", OpeningYear = 2015 }
        );

        // ---------- Screens (8) ----------
        modelBuilder.Entity<Screen>().HasData(
            new Screen { ScreenId = 1, CinemaId = 1, ScreenNumber = 1, SeatingCapacity = 220, ScreenType = "IMAX" },
            new Screen { ScreenId = 2, CinemaId = 1, ScreenNumber = 2, SeatingCapacity = 120, ScreenType = "2D" },
            new Screen { ScreenId = 3, CinemaId = 2, ScreenNumber = 1, SeatingCapacity = 180, ScreenType = "3D" },
            new Screen { ScreenId = 4, CinemaId = 2, ScreenNumber = 2, SeatingCapacity = 90, ScreenType = "2D" },
            new Screen { ScreenId = 5, CinemaId = 3, ScreenNumber = 1, SeatingCapacity = 150, ScreenType = "4DX" },
            new Screen { ScreenId = 6, CinemaId = 3, ScreenNumber = 2, SeatingCapacity = 100, ScreenType = "2D" },
            new Screen { ScreenId = 7, CinemaId = 4, ScreenNumber = 1, SeatingCapacity = 200, ScreenType = "3D" },
            new Screen { ScreenId = 8, CinemaId = 4, ScreenNumber = 2, SeatingCapacity = 80, ScreenType = "2D" }
        );

        // ---------- Screenings (12) ----------
        // Screening 1-3 ligger i fortiden, 4-12 i fremtiden (til "upcoming screenings").
        modelBuilder.Entity<Screening>().HasData(
            new Screening { ScreeningId = 1, MovieId = 4, ScreenId = 1, ScreeningDate = new DateOnly(2026, 10, 10), StartTime = new TimeOnly(19, 30), Price = 150m },
            new Screening { ScreeningId = 2, MovieId = 4, ScreenId = 3, ScreeningDate = new DateOnly(2026, 10, 17), StartTime = new TimeOnly(20, 0), Price = 110m },
            new Screening { ScreeningId = 3, MovieId = 3, ScreenId = 2, ScreeningDate = new DateOnly(2026, 10, 24), StartTime = new TimeOnly(17, 0), Price = 95m },
            new Screening { ScreeningId = 4, MovieId = 1, ScreenId = 4, ScreeningDate = new DateOnly(2026, 11, 2), StartTime = new TimeOnly(19, 0), Price = 90m },
            new Screening { ScreeningId = 5, MovieId = 2, ScreenId = 1, ScreeningDate = new DateOnly(2026, 11, 5), StartTime = new TimeOnly(20, 15), Price = 160m },
            new Screening { ScreeningId = 6, MovieId = 3, ScreenId = 5, ScreeningDate = new DateOnly(2026, 11, 7), StartTime = new TimeOnly(16, 0), Price = 130m },
            new Screening { ScreeningId = 7, MovieId = 4, ScreenId = 7, ScreeningDate = new DateOnly(2026, 11, 12), StartTime = new TimeOnly(21, 0), Price = 120m },
            new Screening { ScreeningId = 8, MovieId = 1, ScreenId = 2, ScreeningDate = new DateOnly(2026, 11, 14), StartTime = new TimeOnly(18, 30), Price = 95m },
            new Screening { ScreeningId = 9, MovieId = 2, ScreenId = 6, ScreeningDate = new DateOnly(2026, 11, 20), StartTime = new TimeOnly(19, 45), Price = 100m },
            new Screening { ScreeningId = 10, MovieId = 3, ScreenId = 8, ScreeningDate = new DateOnly(2026, 11, 22), StartTime = new TimeOnly(15, 0), Price = 85m },
            new Screening { ScreeningId = 11, MovieId = 1, ScreenId = 3, ScreeningDate = new DateOnly(2026, 12, 3), StartTime = new TimeOnly(20, 0), Price = 115m },
            new Screening { ScreeningId = 12, MovieId = 2, ScreenId = 7, ScreeningDate = new DateOnly(2026, 12, 10), StartTime = new TimeOnly(19, 0), Price = 125m }
        );

        // ---------- Ticket sales (19) ----------
        // TotalPrice = Quantity * screeningens pris. Ingen screening overskrider skærmens kapacitet.
        modelBuilder.Entity<TicketSale>().HasData(
            new TicketSale { TicketSaleId = 1, ScreeningId = 1, SaleDateTime = new DateTime(2026, 10, 5, 14, 20, 0), Quantity = 2, TotalPrice = 300m },
            new TicketSale { TicketSaleId = 2, ScreeningId = 1, SaleDateTime = new DateTime(2026, 10, 8, 18, 5, 0), Quantity = 4, TotalPrice = 600m },
            new TicketSale { TicketSaleId = 3, ScreeningId = 1, SaleDateTime = new DateTime(2026, 10, 9, 11, 30, 0), Quantity = 3, TotalPrice = 450m },
            new TicketSale { TicketSaleId = 4, ScreeningId = 2, SaleDateTime = new DateTime(2026, 10, 12, 16, 45, 0), Quantity = 2, TotalPrice = 220m },
            new TicketSale { TicketSaleId = 5, ScreeningId = 2, SaleDateTime = new DateTime(2026, 10, 15, 20, 10, 0), Quantity = 5, TotalPrice = 550m },
            new TicketSale { TicketSaleId = 6, ScreeningId = 3, SaleDateTime = new DateTime(2026, 10, 20, 9, 15, 0), Quantity = 3, TotalPrice = 285m },
            new TicketSale { TicketSaleId = 7, ScreeningId = 3, SaleDateTime = new DateTime(2026, 10, 23, 13, 40, 0), Quantity = 2, TotalPrice = 190m },
            new TicketSale { TicketSaleId = 8, ScreeningId = 4, SaleDateTime = new DateTime(2026, 10, 18, 19, 25, 0), Quantity = 4, TotalPrice = 360m },
            new TicketSale { TicketSaleId = 9, ScreeningId = 5, SaleDateTime = new DateTime(2026, 10, 22, 12, 0, 0), Quantity = 6, TotalPrice = 960m },
            new TicketSale { TicketSaleId = 10, ScreeningId = 5, SaleDateTime = new DateTime(2026, 10, 26, 17, 50, 0), Quantity = 2, TotalPrice = 320m },
            new TicketSale { TicketSaleId = 11, ScreeningId = 6, SaleDateTime = new DateTime(2026, 10, 21, 10, 10, 0), Quantity = 3, TotalPrice = 390m },
            new TicketSale { TicketSaleId = 12, ScreeningId = 7, SaleDateTime = new DateTime(2026, 10, 24, 15, 35, 0), Quantity = 4, TotalPrice = 480m },
            new TicketSale { TicketSaleId = 13, ScreeningId = 7, SaleDateTime = new DateTime(2026, 10, 27, 21, 5, 0), Quantity = 2, TotalPrice = 240m },
            new TicketSale { TicketSaleId = 14, ScreeningId = 8, SaleDateTime = new DateTime(2026, 10, 25, 8, 50, 0), Quantity = 5, TotalPrice = 475m },
            new TicketSale { TicketSaleId = 15, ScreeningId = 9, SaleDateTime = new DateTime(2026, 10, 26, 14, 15, 0), Quantity = 2, TotalPrice = 200m },
            new TicketSale { TicketSaleId = 16, ScreeningId = 10, SaleDateTime = new DateTime(2026, 10, 27, 18, 30, 0), Quantity = 4, TotalPrice = 340m },
            new TicketSale { TicketSaleId = 17, ScreeningId = 10, SaleDateTime = new DateTime(2026, 10, 27, 20, 0, 0), Quantity = 3, TotalPrice = 255m },
            new TicketSale { TicketSaleId = 18, ScreeningId = 11, SaleDateTime = new DateTime(2026, 10, 27, 12, 20, 0), Quantity = 6, TotalPrice = 690m },
            new TicketSale { TicketSaleId = 19, ScreeningId = 12, SaleDateTime = new DateTime(2026, 10, 26, 19, 0, 0), Quantity = 2, TotalPrice = 250m }
        );
    }
}
