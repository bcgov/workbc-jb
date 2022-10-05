using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AuditHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE NocCodes SET ID = Code");

            migrationBuilder.Sql("UPDATE Jobs set NocCodeId = null  where NocCodeId = 0");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NocCodes",
                table: "NocCodes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_NocCodeId",
                table: "Jobs",
                column: "NocCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_NocCodes_NocCodeId",
                table: "Jobs",
                column: "NocCodeId",
                principalTable: "NocCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateTable(
                name: "JobHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<long>(nullable: false),
                    Field = table.Column<string>(maxLength: 50, nullable: true),
                    OldValue = table.Column<string>(maxLength: 500, nullable: true),
                    NewValue = table.Column<string>(nullable: true),
                    ModifiedByAdminUserId = table.Column<int>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobHistory_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobHistory_AdminUsers_ModifiedByAdminUserId",
                        column: x => x.ModifiedByAdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSeekerHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUserId = table.Column<string>(nullable: true),
                    Field = table.Column<string>(maxLength: 50, nullable: true),
                    OldValue = table.Column<string>(maxLength: 500, nullable: true),
                    NewValue = table.Column<string>(nullable: true),
                    ModifiedByAdminUserId = table.Column<int>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekerHistory_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekerHistory_AdminUsers_ModifiedByAdminUserId",
                        column: x => x.ModifiedByAdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobHistory_JobId",
                table: "JobHistory",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobHistory_ModifiedByAdminUserId",
                table: "JobHistory",
                column: "ModifiedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerHistory_AspNetUserId",
                table: "JobSeekerHistory",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerHistory_ModifiedByAdminUserId",
                table: "JobSeekerHistory",
                column: "ModifiedByAdminUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobHistory");

            migrationBuilder.DropTable(
                name: "JobSeekerHistory");
        }
    }
}
