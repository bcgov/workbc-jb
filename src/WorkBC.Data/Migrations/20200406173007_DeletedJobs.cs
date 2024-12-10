using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class DeletedJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAdminLog");

            migrationBuilder.CreateTable(
                name: "DeletedJobs",
                columns: table => new
                {
                    JobId = table.Column<long>(nullable: false),
                    DeletedByAdminUserId = table.Column<int>(nullable: false),
                    DateDeleted = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedJobs", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_DeletedJobs_AdminUsers_DeletedByAdminUserId",
                        column: x => x.DeletedByAdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeletedJobs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeletedJobs_DeletedByAdminUserId",
                table: "DeletedJobs",
                column: "DeletedByAdminUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletedJobs");

            migrationBuilder.CreateTable(
                name: "JobAdminLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Field = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    JobId = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedByAdminUserId = table.Column<int>(type: "int", nullable: false),
                    NewValue = table.Column<string>(type: "varchar", nullable: true),
                    OldValue = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAdminLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobAdminLog_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobAdminLog_AdminUsers_ModifiedByAdminUserId",
                        column: x => x.ModifiedByAdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAdminLog_JobId",
                table: "JobAdminLog",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobAdminLog_ModifiedByAdminUserId",
                table: "JobAdminLog",
                column: "ModifiedByAdminUserId");
        }
    }
}
