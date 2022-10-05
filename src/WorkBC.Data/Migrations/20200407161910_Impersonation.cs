using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class Impersonation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImpersonationLog",
                columns: table => new
                {
                    Token = table.Column<string>(maxLength: 200, nullable: false),
                    AspNetUserId = table.Column<string>(nullable: true),
                    AdminUserId = table.Column<int>(nullable: false),
                    DateTokenCreated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpersonationLog", x => x.Token);
                    table.ForeignKey(
                        name: "FK_ImpersonationLog_AdminUsers_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImpersonationLog_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImpersonationLog_AdminUserId",
                table: "ImpersonationLog",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpersonationLog_AspNetUserId",
                table: "ImpersonationLog",
                column: "AspNetUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImpersonationLog");
        }
    }
}
