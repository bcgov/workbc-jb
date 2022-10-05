using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdatedPrimaryKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportedJobsWanted",
                table: "ImportedJobsWanted");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportedJobsFederal",
                table: "ImportedJobsFederal");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ImportedJobsWanted");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ImportedJobsFederal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportedJobsWanted",
                table: "ImportedJobsWanted",
                column: "JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportedJobsFederal",
                table: "ImportedJobsFederal",
                column: "JobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportedJobsWanted",
                table: "ImportedJobsWanted");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportedJobsFederal",
                table: "ImportedJobsFederal");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ImportedJobsWanted",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ImportedJobsFederal",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportedJobsWanted",
                table: "ImportedJobsWanted",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportedJobsFederal",
                table: "ImportedJobsFederal",
                column: "Id");
        }
    }
}
