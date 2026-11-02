using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieVault.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "ActorId", "BirthDate", "Gender", "Name", "Nationality" },
                values: new object[,]
                {
                    { 1, new DateOnly(1978, 4, 12), "Male", "Jonas Beck", "Danish" },
                    { 2, new DateOnly(1990, 9, 3), "Female", "Emma Larsson", "Swedish" },
                    { 3, new DateOnly(1965, 1, 22), "Male", "Henrik Dahl", "Norwegian" },
                    { 4, new DateOnly(1985, 7, 30), "Female", "Sofia Marino", "Italian" },
                    { 5, new DateOnly(1958, 11, 8), "Female", "Margaret Ellis", "British" },
                    { 6, new DateOnly(1995, 2, 17), "Female", "Aino Virtanen", "Finnish" },
                    { 7, new DateOnly(1992, 6, 25), "Male", "Lucas Moreau", "French" },
                    { 8, new DateOnly(1988, 12, 1), "Male", "Daniel Okafor", "Nigerian" },
                    { 9, new DateOnly(1950, 5, 19), "Male", "Peter Holm", "Danish" },
                    { 10, new DateOnly(1999, 3, 9), "Female", "Yuki Tanaka", "Japanese" }
                });

            migrationBuilder.InsertData(
                table: "Cinemas",
                columns: new[] { "CinemaId", "Address", "City", "Name", "OpeningYear" },
                values: new object[,]
                {
                    { 1, "Axeltorv 2", "Copenhagen", "Grand Biograf", 1912 },
                    { 2, "Havnegade 10", "Aarhus", "Harbour Lights Cinema", 1998 },
                    { 3, "Vestergade 5", "Odense", "Odense Filmpalads", 2005 },
                    { 4, "Jyllandsgade 20", "Aalborg", "Aalborg Cineplex", 2015 }
                });

            migrationBuilder.InsertData(
                table: "Directors",
                columns: new[] { "DirectorId", "BirthDate", "Name", "Nationality" },
                values: new object[,]
                {
                    { 1, new DateOnly(1975, 8, 14), "Anna Kristensen", "Danish" },
                    { 2, new DateOnly(1969, 3, 27), "Marcus Webb", "American" },
                    { 3, new DateOnly(1982, 10, 5), "Elise Fontaine", "French" },
                    { 4, new DateOnly(1980, 1, 19), "Rajesh Nair", "Indian" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "LocationId", "City", "Country", "LocationType", "Name" },
                values: new object[,]
                {
                    { 1, "Copenhagen", "Denmark", "street", "Nyhavn Waterfront" },
                    { 2, "Aarhus", "Denmark", "building", "Old Customs House" },
                    { 3, "Svolvaer", "Norway", "landscape", "Lofoten Coastline" },
                    { 4, "Los Angeles", "United States", "set", "Soundstage 4" },
                    { 5, "Paris", "France", "street", "Rue des Lanternes" },
                    { 6, "Ilulissat", "Greenland", "landscape", "Icefjord Plateau" }
                });

            migrationBuilder.InsertData(
                table: "ProductionCompanies",
                columns: new[] { "ProductionCompanyId", "Country", "FoundedYear", "Name" },
                values: new object[,]
                {
                    { 1, "Denmark", 1998, "Nordic Lights Pictures" },
                    { 2, "United States", 1985, "Silverline Studios" },
                    { 3, "Sweden", 2005, "Aurora Film Group" },
                    { 4, "United Kingdom", 2012, "Blue Meridian Productions" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "AgeRating", "DirectorId", "MovieStatus", "ProductionCompanyId", "ReleaseDate", "Runtime", "Title" },
                values: new object[,]
                {
                    { 1, "PG-13", 1, "released", 1, new DateOnly(2024, 3, 15), 118, "Midnight Harbor" },
                    { 2, "R", 2, "released", 2, new DateOnly(2025, 6, 20), 132, "The Glass Orchard" },
                    { 3, "PG", 3, "released", 1, new DateOnly(2025, 11, 7), 105, "Echoes of Tundra" },
                    { 4, "R", 1, "released", 3, new DateOnly(2026, 10, 2), 126, "Neon Requiem" },
                    { 5, "PG-13", 4, "in production", 4, new DateOnly(2027, 2, 14), 111, "Paper Kingdoms" },
                    { 6, "PG-13", 2, "planned", 2, new DateOnly(2027, 8, 1), 120, "The Last Lighthouse" }
                });

            migrationBuilder.InsertData(
                table: "Screens",
                columns: new[] { "ScreenId", "CinemaId", "ScreenNumber", "ScreenType", "SeatingCapacity" },
                values: new object[,]
                {
                    { 1, 1, 1, "IMAX", 220 },
                    { 2, 1, 2, "2D", 120 },
                    { 3, 2, 1, "3D", 180 },
                    { 4, 2, 2, "2D", 90 },
                    { 5, 3, 1, "4DX", 150 },
                    { 6, 3, 2, "2D", 100 },
                    { 7, 4, 1, "3D", 200 },
                    { 8, 4, 2, "2D", 80 }
                });

            migrationBuilder.InsertData(
                table: "Studios",
                columns: new[] { "StudioId", "Capacity", "City", "CompanyId", "Country", "Name", "NumberOfSoundStages" },
                values: new object[,]
                {
                    { 1, 400, "Copenhagen", 1, "Denmark", "Nordic Lights Studio Copenhagen", 6 },
                    { 2, 1200, "Los Angeles", 2, "United States", "Silverline Burbank Lot", 12 },
                    { 3, 800, "Atlanta", 2, "United States", "Silverline Atlanta Stages", 8 },
                    { 4, 350, "Stockholm", 3, "Sweden", "Aurora Stockholm Studios", 5 },
                    { 5, 600, "London", 4, "United Kingdom", "Blue Meridian London Studios", 7 }
                });

            migrationBuilder.InsertData(
                table: "FilmedAt",
                columns: new[] { "LocationId", "MovieId", "NumberOfShootingDays" },
                values: new object[,]
                {
                    { 1, 1, 10 },
                    { 2, 1, 6 },
                    { 4, 1, 14 },
                    { 4, 2, 20 },
                    { 5, 2, 8 },
                    { 3, 3, 12 },
                    { 6, 3, 9 },
                    { 1, 4, 5 },
                    { 4, 4, 18 },
                    { 5, 4, 7 },
                    { 2, 5, 4 },
                    { 3, 5, 6 }
                });

            migrationBuilder.InsertData(
                table: "MovieCasts",
                columns: new[] { "ActorId", "MovieId", "CharacterName", "RoleType" },
                values: new object[,]
                {
                    { 1, 1, "Captain Elias Rowe", "lead" },
                    { 2, 1, "Mara Quinn", "lead" },
                    { 3, 1, "Dockmaster Hale", "supporting" },
                    { 9, 1, "Old Fisherman", "cameo" },
                    { 1, 2, "Viktor", "supporting" },
                    { 2, 2, "Clara Voss", "lead" },
                    { 4, 2, "Detective Anton Brandt", "lead" },
                    { 5, 2, "Mrs. Lindqvist", "supporting" },
                    { 3, 3, "Professor Lund", "supporting" },
                    { 6, 3, "Aino", "lead" },
                    { 7, 3, "Kasper", "lead" },
                    { 10, 3, "Research Student", "cameo" },
                    { 2, 4, "Nightclub Singer", "cameo" },
                    { 4, 4, "Vera Santoro", "lead" },
                    { 5, 4, "The Collector", "supporting" },
                    { 8, 4, "Jax Mercer", "lead" },
                    { 6, 5, "Princess Lena", "lead" },
                    { 7, 5, "The Cartographer", "supporting" },
                    { 8, 5, "General Oyo", "supporting" },
                    { 1, 6, "Keeper Anders", "lead" },
                    { 9, 6, "Harbor Master", "supporting" }
                });

            migrationBuilder.InsertData(
                table: "ProductionRecords",
                columns: new[] { "ProductionRecordId", "Budget", "MovieId", "ProductionEndDate", "ProductionStartDate" },
                values: new object[,]
                {
                    { 1, 45000000m, 1, new DateOnly(2023, 9, 30), new DateOnly(2023, 1, 10) },
                    { 2, 80000000m, 2, new DateOnly(2024, 12, 15), new DateOnly(2024, 2, 1) },
                    { 3, 28000000m, 3, new DateOnly(2025, 2, 28), new DateOnly(2024, 5, 5) },
                    { 4, 62500000m, 4, new DateOnly(2026, 1, 31), new DateOnly(2025, 3, 1) },
                    { 5, 35000000m, 5, new DateOnly(2026, 11, 30), new DateOnly(2026, 3, 15) }
                });

            migrationBuilder.InsertData(
                table: "Screenings",
                columns: new[] { "ScreeningId", "MovieId", "Price", "ScreenId", "ScreeningDate", "StartTime" },
                values: new object[,]
                {
                    { 1, 4, 150m, 1, new DateOnly(2026, 10, 10), new TimeOnly(19, 30, 0) },
                    { 2, 4, 110m, 3, new DateOnly(2026, 10, 17), new TimeOnly(20, 0, 0) },
                    { 3, 3, 95m, 2, new DateOnly(2026, 10, 24), new TimeOnly(17, 0, 0) },
                    { 4, 1, 90m, 4, new DateOnly(2026, 11, 2), new TimeOnly(19, 0, 0) },
                    { 5, 2, 160m, 1, new DateOnly(2026, 11, 5), new TimeOnly(20, 15, 0) },
                    { 6, 3, 130m, 5, new DateOnly(2026, 11, 7), new TimeOnly(16, 0, 0) },
                    { 7, 4, 120m, 7, new DateOnly(2026, 11, 12), new TimeOnly(21, 0, 0) },
                    { 8, 1, 95m, 2, new DateOnly(2026, 11, 14), new TimeOnly(18, 30, 0) },
                    { 9, 2, 100m, 6, new DateOnly(2026, 11, 20), new TimeOnly(19, 45, 0) },
                    { 10, 3, 85m, 8, new DateOnly(2026, 11, 22), new TimeOnly(15, 0, 0) },
                    { 11, 1, 115m, 3, new DateOnly(2026, 12, 3), new TimeOnly(20, 0, 0) },
                    { 12, 2, 125m, 7, new DateOnly(2026, 12, 10), new TimeOnly(19, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "TicketSales",
                columns: new[] { "TicketSaleId", "Quantity", "SaleDateTime", "ScreeningId", "TotalPrice" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 10, 5, 14, 20, 0, 0, DateTimeKind.Unspecified), 1, 300m },
                    { 2, 4, new DateTime(2026, 10, 8, 18, 5, 0, 0, DateTimeKind.Unspecified), 1, 600m },
                    { 3, 3, new DateTime(2026, 10, 9, 11, 30, 0, 0, DateTimeKind.Unspecified), 1, 450m },
                    { 4, 2, new DateTime(2026, 10, 12, 16, 45, 0, 0, DateTimeKind.Unspecified), 2, 220m },
                    { 5, 5, new DateTime(2026, 10, 15, 20, 10, 0, 0, DateTimeKind.Unspecified), 2, 550m },
                    { 6, 3, new DateTime(2026, 10, 20, 9, 15, 0, 0, DateTimeKind.Unspecified), 3, 285m },
                    { 7, 2, new DateTime(2026, 10, 23, 13, 40, 0, 0, DateTimeKind.Unspecified), 3, 190m },
                    { 8, 4, new DateTime(2026, 10, 18, 19, 25, 0, 0, DateTimeKind.Unspecified), 4, 360m },
                    { 9, 6, new DateTime(2026, 10, 22, 12, 0, 0, 0, DateTimeKind.Unspecified), 5, 960m },
                    { 10, 2, new DateTime(2026, 10, 26, 17, 50, 0, 0, DateTimeKind.Unspecified), 5, 320m },
                    { 11, 3, new DateTime(2026, 10, 21, 10, 10, 0, 0, DateTimeKind.Unspecified), 6, 390m },
                    { 12, 4, new DateTime(2026, 10, 24, 15, 35, 0, 0, DateTimeKind.Unspecified), 7, 480m },
                    { 13, 2, new DateTime(2026, 10, 27, 21, 5, 0, 0, DateTimeKind.Unspecified), 7, 240m },
                    { 14, 5, new DateTime(2026, 10, 25, 8, 50, 0, 0, DateTimeKind.Unspecified), 8, 475m },
                    { 15, 2, new DateTime(2026, 10, 26, 14, 15, 0, 0, DateTimeKind.Unspecified), 9, 200m },
                    { 16, 4, new DateTime(2026, 10, 27, 18, 30, 0, 0, DateTimeKind.Unspecified), 10, 340m },
                    { 17, 3, new DateTime(2026, 10, 27, 20, 0, 0, 0, DateTimeKind.Unspecified), 10, 255m },
                    { 18, 6, new DateTime(2026, 10, 27, 12, 20, 0, 0, DateTimeKind.Unspecified), 11, 690m },
                    { 19, 2, new DateTime(2026, 10, 26, 19, 0, 0, 0, DateTimeKind.Unspecified), 12, 250m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "FilmedAt",
                keyColumns: new[] { "LocationId", "MovieId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 6, 3 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 10, 3 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 8, 4 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 8, 5 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "MovieCasts",
                keyColumns: new[] { "ActorId", "MovieId" },
                keyValues: new object[] { 9, 6 });

            migrationBuilder.DeleteData(
                table: "ProductionRecords",
                keyColumn: "ProductionRecordId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductionRecords",
                keyColumn: "ProductionRecordId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductionRecords",
                keyColumn: "ProductionRecordId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductionRecords",
                keyColumn: "ProductionRecordId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductionRecords",
                keyColumn: "ProductionRecordId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Studios",
                keyColumn: "StudioId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Studios",
                keyColumn: "StudioId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Studios",
                keyColumn: "StudioId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Studios",
                keyColumn: "StudioId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Studios",
                keyColumn: "StudioId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "TicketSales",
                keyColumn: "TicketSaleId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "LocationId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "ScreeningId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Directors",
                keyColumn: "DirectorId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductionCompanies",
                keyColumn: "ProductionCompanyId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Screens",
                keyColumn: "ScreenId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Screens",
                keyColumn: "ScreenId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Screens",
                keyColumn: "ScreenId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Screens",
                keyColumn: "ScreenId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Screens",
                keyColumn: "ScreenId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Screens",
                keyColumn: "ScreenId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Screens",
                keyColumn: "ScreenId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Screens",
                keyColumn: "ScreenId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cinemas",
                keyColumn: "CinemaId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cinemas",
                keyColumn: "CinemaId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cinemas",
                keyColumn: "CinemaId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cinemas",
                keyColumn: "CinemaId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Directors",
                keyColumn: "DirectorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Directors",
                keyColumn: "DirectorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Directors",
                keyColumn: "DirectorId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductionCompanies",
                keyColumn: "ProductionCompanyId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductionCompanies",
                keyColumn: "ProductionCompanyId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductionCompanies",
                keyColumn: "ProductionCompanyId",
                keyValue: 3);
        }
    }
}
