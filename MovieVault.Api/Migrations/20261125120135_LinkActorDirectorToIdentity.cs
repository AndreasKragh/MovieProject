using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieVault.Api.Migrations
{
    /// <inheritdoc />
    public partial class LinkActorDirectorToIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiUserId",
                table: "Directors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiUserId",
                table: "Actors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 1,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 2,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 3,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 4,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 5,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 6,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 7,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 8,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 9,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Actors",
                keyColumn: "ActorId",
                keyValue: 10,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Directors",
                keyColumn: "DirectorId",
                keyValue: 1,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Directors",
                keyColumn: "DirectorId",
                keyValue: 2,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Directors",
                keyColumn: "DirectorId",
                keyValue: 3,
                column: "ApiUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Directors",
                keyColumn: "DirectorId",
                keyValue: 4,
                column: "ApiUserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Directors_ApiUserId",
                table: "Directors",
                column: "ApiUserId",
                unique: true,
                filter: "[ApiUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_ApiUserId",
                table: "Actors",
                column: "ApiUserId",
                unique: true,
                filter: "[ApiUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Actors_AspNetUsers_ApiUserId",
                table: "Actors",
                column: "ApiUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Directors_AspNetUsers_ApiUserId",
                table: "Directors",
                column: "ApiUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actors_AspNetUsers_ApiUserId",
                table: "Actors");

            migrationBuilder.DropForeignKey(
                name: "FK_Directors_AspNetUsers_ApiUserId",
                table: "Directors");

            migrationBuilder.DropIndex(
                name: "IX_Directors_ApiUserId",
                table: "Directors");

            migrationBuilder.DropIndex(
                name: "IX_Actors_ApiUserId",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "ApiUserId",
                table: "Directors");

            migrationBuilder.DropColumn(
                name: "ApiUserId",
                table: "Actors");
        }
    }
}
