using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerAPI.Migrations
{
    public partial class UserToken_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserToken",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserToken",
                table: "Users",
                column: "UserToken",
                unique: true,
                filter: "[UserToken] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserToken",
                table: "Users");
        }
    }
}
