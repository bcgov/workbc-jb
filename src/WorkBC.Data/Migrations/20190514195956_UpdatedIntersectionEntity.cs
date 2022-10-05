using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdatedIntersectionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSkill_Job_JobId",
                table: "JobSkill");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSkill_Skill_SkillId",
                table: "JobSkill");

            migrationBuilder.DropIndex(
                name: "IX_JobSkill_SkillId",
                table: "JobSkill");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobSkill_SkillId",
                table: "JobSkill",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkill_Job_JobId",
                table: "JobSkill",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSkill_Skill_SkillId",
                table: "JobSkill",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "SkillId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
