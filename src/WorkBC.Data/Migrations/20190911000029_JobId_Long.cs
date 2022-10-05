using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobId_Long : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_JobSkill", "JobSkill");

            migrationBuilder.AlterColumn<long>(
                name: "JobId",
                table: "JobSkill",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey("PK_JobSkill", "JobSkill", new[]{"JobId", "SkillId"});

            migrationBuilder.DropPrimaryKey("PK_Job", "Job");

            migrationBuilder.AlterColumn<long>(
                name: "JobId",
                table: "Job",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey("PK_Job", "Job", "JobId");

            migrationBuilder.DropPrimaryKey("PK_ImportedJobsFederal", "ImportedJobsFederal");

            migrationBuilder.AlterColumn<long>(
                name: "JobId",
                table: "ImportedJobsFederal",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey("PK_ImportedJobsFederal", "ImportedJobsFederal", "JobId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "JobSkill",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "Job",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "ImportedJobsFederal",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
