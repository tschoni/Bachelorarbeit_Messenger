using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerAPI.Migrations
{
    public partial class Added_EphemKeys_SignedKeys_KeyAreByteArrays : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyString",
                table: "PublicKeys");

            migrationBuilder.DropColumn(
                name: "CipherText",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MAC",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "PublicKeys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "InitiatorId",
                table: "PublicKeys",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "KeyBytes",
                table: "PublicKeys",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "CipherText",
                table: "Messages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicKeys_InitiatorId",
                table: "PublicKeys",
                column: "InitiatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PublicKeys_Users_InitiatorId",
                table: "PublicKeys",
                column: "InitiatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublicKeys_Users_InitiatorId",
                table: "PublicKeys");

            migrationBuilder.DropIndex(
                name: "IX_PublicKeys_InitiatorId",
                table: "PublicKeys");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "PublicKeys");

            migrationBuilder.DropColumn(
                name: "InitiatorId",
                table: "PublicKeys");

            migrationBuilder.DropColumn(
                name: "KeyBytes",
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
