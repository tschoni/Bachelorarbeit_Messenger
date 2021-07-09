using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MessengerWPF.Migrations
{
    public partial class SignedKeys_KeyTypes_Admins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "KeyString",
                table: "Keys",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "GroupUser1",
                columns: table => new
                {
                    AdminOfGroupsId = table.Column<long>(type: "INTEGER", nullable: false),
                    AdminsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUser1", x => new { x.AdminOfGroupsId, x.AdminsId });
                    table.ForeignKey(
                        name: "FK_GroupUser1_Groups_AdminOfGroupsId",
                        column: x => x.AdminOfGroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupUser1_Users_AdminsId",
                        column: x => x.AdminsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser1_AdminsId",
                table: "GroupUser1",
                column: "AdminsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupUser1");

            migrationBuilder.AlterColumn<string>(
                name: "KeyString",
                table: "Keys",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);
        }
    }
}
