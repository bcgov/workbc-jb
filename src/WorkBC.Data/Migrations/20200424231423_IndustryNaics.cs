using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class IndustryNaics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "JobSeekerAdminLog", null, "JobSeekerChangeLog");

            migrationBuilder.CreateTable(
                name: "IndustryNaics",
                columns: table => new
                {
                    IndustryId = table.Column<short>(nullable: false),
                    NaicsId = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryNaics", x => new { x.IndustryId, x.NaicsId });
                    table.ForeignKey(
                        name: "FK_IndustryNaics_Industries_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Industries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.DropIndex("IX_JobSeekerAdminLog_AspNetUserId", "JobSeekerChangeLog");

            migrationBuilder.DropIndex("IX_JobSeekerAdminLog_ModifiedByAdminUserId", "JobSeekerChangeLog");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerChangeLog_AspNetUserId",
                table: "JobSeekerChangeLog",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerChangeLog_ModifiedByAdminUserId",
                table: "JobSeekerChangeLog",
                column: "ModifiedByAdminUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndustryNaics");

            migrationBuilder.DropTable(
                name: "JobSeekerChangeLog");

            migrationBuilder.CreateTable(
                name: "JobSeekerAdminLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedByAdminUserId = table.Column<int>(type: "int", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerAdminLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekerAdminLog_AdminUsers_ModifiedByAdminUserId",
                        column: x => x.ModifiedByAdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerAdminLog_AspNetUserId",
                table: "JobSeekerAdminLog",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerAdminLog_ModifiedByAdminUserId",
                table: "JobSeekerAdminLog",
                column: "ModifiedByAdminUserId");
        }
    }
}
