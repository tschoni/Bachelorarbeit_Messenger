using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerAPI.Migrations
{
    public partial class Added_DeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EphemeralKeys_Users_InitiatorId",
                table: "EphemeralKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicKeys_Users_OwnerId",
                table: "PublicKeys");

            migrationBuilder.AddForeignKey(
                name: "FK_EphemeralKeys_Users_InitiatorId",
                table: "EphemeralKeys",
                column: "InitiatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicKeys_Users_OwnerId",
                table: "PublicKeys",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EphemeralKeys_Users_InitiatorId",
                table: "EphemeralKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicKeys_Users_OwnerId",
                table: "PublicKeys");

            migrationBuilder.AddForeignKey(
                name: "FK_EphemeralKeys_Users_InitiatorId",
                table: "EphemeralKeys",
                column: "InitiatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicKeys_Users_OwnerId",
                table: "PublicKeys",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
