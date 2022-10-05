using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class LoggingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobHistory");

            migrationBuilder.DropTable(
                name: "JobSeekerHistory");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SystemSettings",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "JobAdminLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<long>(nullable: false),
                    Field = table.Column<string>(maxLength: 50, nullable: true),
                    OldValue = table.Column<string>(nullable: true),
                    NewValue = table.Column<string>(nullable: true),
                    ModifiedByAdminUserId = table.Column<int>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "JobSeekerAdminComments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUserId = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    IsPinned = table.Column<bool>(nullable: false),
                    EnteredByAdminUserId = table.Column<int>(nullable: false),
                    DateEntered = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerAdminComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekerAdminComments_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekerAdminComments_AdminUsers_EnteredByAdminUserId",
                        column: x => x.EnteredByAdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSeekerAdminLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUserId = table.Column<string>(nullable: true),
                    Field = table.Column<string>(maxLength: 50, nullable: true),
                    OldValue = table.Column<string>(nullable: true),
                    NewValue = table.Column<string>(nullable: true),
                    ModifiedByAdminUserId = table.Column<int>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "JobSeekerEventLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUserId = table.Column<string>(nullable: true),
                    EventTypeId = table.Column<int>(nullable: false),
                    DateLogged = table.Column<DateTime>(nullable: false),
                    Year = table.Column<short>(nullable: false),
                    Month = table.Column<byte>(nullable: false),
                    Week = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerEventLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekerEventLog_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAdminLog_JobId",
                table: "JobAdminLog",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobAdminLog_ModifiedByAdminUserId",
                table: "JobAdminLog",
                column: "ModifiedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerAdminComments_AspNetUserId",
                table: "JobSeekerAdminComments",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerAdminComments_EnteredByAdminUserId",
                table: "JobSeekerAdminComments",
                column: "EnteredByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerAdminLog_AspNetUserId",
                table: "JobSeekerAdminLog",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerAdminLog_ModifiedByAdminUserId",
                table: "JobSeekerAdminLog",
                column: "ModifiedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerEventLog_AspNetUserId",
                table: "JobSeekerEventLog",
                column: "AspNetUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAdminLog");

            migrationBuilder.DropTable(
                name: "JobSeekerAdminComments");

            migrationBuilder.DropTable(
                name: "JobSeekerAdminLog");

            migrationBuilder.DropTable(
                name: "JobSeekerEventLog");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SystemSettings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "JobHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobId = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedByAdminUserId = table.Column<int>(type: "int", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedByAdminUserId = table.Column<int>(type: "int", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
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
    }
}
