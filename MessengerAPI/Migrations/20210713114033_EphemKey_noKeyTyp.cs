using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerAPI.Migrations
{
    public partial class EphemKey_noKeyTyp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyType",
                table: "EphemeralKeys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KeyType",
                table: "EphemeralKeys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
