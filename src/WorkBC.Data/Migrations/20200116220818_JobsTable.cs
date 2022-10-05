using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SecurityQuestion_SecurityQuestionId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "JobSkill");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "SkillCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SecurityQuestion",
                table: "SecurityQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobView",
                table: "JobView");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Job",
                table: "Job");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpiredJob",
                table: "ExpiredJob");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Country",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "DisplayUntil",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "Job");

            migrationBuilder.RenameTable(
                name: "SecurityQuestion",
                newName: "SecurityQuestions");

            migrationBuilder.RenameTable(
                name: "JobView",
                newName: "JobViews");

            migrationBuilder.RenameTable(
                name: "Job",
                newName: "Jobs");

            migrationBuilder.RenameTable(
                name: "ExpiredJob",
                newName: "ExpiredJobs");

            migrationBuilder.RenameTable(
                name: "Country",
                newName: "Countries");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Jobs",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "PositionsAvailable",
                table: "Jobs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<short>(
                name: "Noc",
                table: "Jobs",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "EmployerName",
                table: "Jobs",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Casual",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Jobs",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFirstSeen",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "FullTime",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "LeadingToFullTime",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Jobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "NaicsId",
                table: "Jobs",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OriginalDatePosted",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "PartTime",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Permanent",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalarySortAscending",
                table: "Jobs",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SalarySortDescending",
                table: "Jobs",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SalarySummary",
                table: "Jobs",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Seasonal",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Temporary",
                table: "Jobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecurityQuestions",
                table: "SecurityQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobViews",
                table: "JobViews",
                column: "JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs",
                column: "JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpiredJobs",
                table: "ExpiredJobs",
                column: "JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SecurityQuestions_SecurityQuestionId",
                table: "AspNetUsers",
                column: "SecurityQuestionId",
                principalTable: "SecurityQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SecurityQuestions_SecurityQuestionId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SecurityQuestions",
                table: "SecurityQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobViews",
                table: "JobViews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpiredJobs",
                table: "ExpiredJobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Casual",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "DateFirstSeen",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "FullTime",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LeadingToFullTime",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "NaicsId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "OriginalDatePosted",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PartTime",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Permanent",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SalarySortAscending",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SalarySortDescending",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SalarySummary",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Seasonal",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Temporary",
                table: "Jobs");

            migrationBuilder.RenameTable(
                name: "SecurityQuestions",
                newName: "SecurityQuestion");

            migrationBuilder.RenameTable(
                name: "JobViews",
                newName: "JobView");

            migrationBuilder.RenameTable(
                name: "Jobs",
                newName: "Job");

            migrationBuilder.RenameTable(
                name: "ExpiredJobs",
                newName: "ExpiredJob");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "Country");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PositionsAvailable",
                table: "Job",
                type: "int",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<int>(
                name: "Noc",
                table: "Job",
                type: "int",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<string>(
                name: "EmployerName",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Job",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DisplayUntil",
                table: "Job",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "Job",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecurityQuestion",
                table: "SecurityQuestion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobView",
                table: "JobView",
                column: "JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Job",
                table: "Job",
                column: "JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpiredJob",
                table: "ExpiredJob",
                column: "JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Country",
                table: "Country",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JobSkill",
                columns: table => new
                {
                    JobId = table.Column<long>(type: "bigint", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSkill", x => new { x.JobId, x.SkillId });
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    SkillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.SkillId);
                });

            migrationBuilder.CreateTable(
                name: "SkillCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillCategory", x => new { x.CategoryId, x.Name });
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SecurityQuestion_SecurityQuestionId",
                table: "AspNetUsers",
                column: "SecurityQuestionId",
                principalTable: "SecurityQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
