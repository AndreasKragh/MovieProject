using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieVault.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReviewsAndMediaOutlets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaOutlets",
                columns: table => new
                {
                    MediaOutletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OutletType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaOutlets", x => x.MediaOutletId);
                    table.CheckConstraint("CK_MediaOutlet_OutletType", "[OutletType] IN ('newspaper', 'tv', 'radio', 'magazine', 'online')");
                });

            migrationBuilder.CreateTable(
                name: "Reviewers",
                columns: table => new
                {
                    ReviewerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MediaOutletId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.ReviewerId);
                    table.ForeignKey(
                        name: "FK_Reviewers_AspNetUsers_ApiUserId",
                        column: x => x.ApiUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviewers_MediaOutlets_MediaOutletId",
                        column: x => x.MediaOutletId,
                        principalTable: "MediaOutlets",
                        principalColumn: "MediaOutletId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Headline = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    MediaOutletId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.CheckConstraint("CK_Review_Rating", "[Rating] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Reviews_MediaOutlets_MediaOutletId",
                        column: x => x.MediaOutletId,
                        principalTable: "MediaOutlets",
                        principalColumn: "MediaOutletId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Reviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Reviewers",
                        principalColumn: "ReviewerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MediaOutlets",
                columns: new[] { "MediaOutletId", "Name", "OutletType", "Website" },
                values: new object[,]
                {
                    { 1, "Morgenposten", "newspaper", "https://www.morgenposten.example" },
                    { 2, "Kanal Nord", "tv", "https://www.kanalnord.example" },
                    { 3, "CineZine", "online", "https://www.cinezine.example" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaOutlets_Name",
                table: "MediaOutlets",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_ApiUserId",
                table: "Reviewers",
                column: "ApiUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviewers_MediaOutletId",
                table: "Reviewers",
                column: "MediaOutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MediaOutletId",
                table: "Reviews",
                column: "MediaOutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MovieId",
                table: "Reviews",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Reviewers");

            migrationBuilder.DropTable(
                name: "MediaOutlets");
        }
    }
}
