using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherForecast.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTheIsSuperUserPropertyFromApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInputs_AspNetUsers_UserId",
                table: "UserInputs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInputs",
                table: "UserInputs");

            migrationBuilder.DropColumn(
                name: "IsSuperUser",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "UserInputs",
                newName: "Favorites");

            migrationBuilder.RenameIndex(
                name: "IX_UserInputs_UserId",
                table: "Favorites",
                newName: "IX_Favorites_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_UserId",
                table: "Favorites",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_UserId",
                table: "Favorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites");

            migrationBuilder.RenameTable(
                name: "Favorites",
                newName: "UserInputs");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_UserId",
                table: "UserInputs",
                newName: "IX_UserInputs_UserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperUser",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInputs",
                table: "UserInputs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInputs_AspNetUsers_UserId",
                table: "UserInputs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
