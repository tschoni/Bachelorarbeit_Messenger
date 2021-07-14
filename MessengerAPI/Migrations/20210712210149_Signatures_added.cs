using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerAPI.Migrations
{
    public partial class Signatures_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyString",
                table: "PublicKeys");

            migrationBuilder.DropColumn(
                name: "MAC",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "CipherText",
                table: "Messages");

            migrationBuilder.AddColumn<byte[]>(
                name: "KeyBytes",
                table: "PublicKeys",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Signature",
                table: "PublicKeys",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "CipherText",
                table: "Messages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EphemeralKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    OwnerId = table.Column<long>(type: "bigint", nullable: true),
                    KeyType = table.Column<int>(type: "int", nullable: false),
                    InitiatorId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EphemeralKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EphemeralKeys_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EphemeralKeys_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EphemeralKeys_InitiatorId",
                table: "EphemeralKeys",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_EphemeralKeys_OwnerId",
                table: "EphemeralKeys",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EphemeralKeys");

            migrationBuilder.DropColumn(
                name: "KeyBytes",
                table: "PublicKeys");

            migrationBuilder.DropColumn(
                name: "Signature",
                table: "PublicKeys");

            migrationBuilder.DropColumn(
                name: "CipherText",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "KeyString",
                table: "PublicKeys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CipherText",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MAC",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
