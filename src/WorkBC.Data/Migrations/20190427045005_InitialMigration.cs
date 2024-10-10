using System;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImportedJobsFederal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(nullable: false),
                    FileUpdateDate = table.Column<DateTime>(nullable: false),
                    DateImported = table.Column<DateTime>(nullable: false),
                    JobPostEnglish = table.Column<string>(nullable: true),
                    JobPostFrench = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedJobsFederal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportedJobsWanted",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<long>(nullable: false),
                    JobXml = table.Column<string>(nullable: true),
                    DateImported = table.Column<DateTime>(nullable: false),
                    DateRefreshed = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportedJobsWanted", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportedJobsFederal");

            migrationBuilder.DropTable(
                name: "ImportedJobsWanted");
        }
    }
}
