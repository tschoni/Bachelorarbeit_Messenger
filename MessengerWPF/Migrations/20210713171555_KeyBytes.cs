using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerWPF.Migrations
{
    public partial class KeyBytes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeyString",
                table: "Keys",
                newName: "KeyBytes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KeyBytes",
                table: "Keys",
                newName: "KeyString");
        }
    }
}
