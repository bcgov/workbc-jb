using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RemovedPropertiesSkillCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillCategory_Skill_SkillId",
                table: "SkillCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillCategory",
                table: "SkillCategory");

            migrationBuilder.DropIndex(
                name: "IX_SkillCategory_SkillId",
                table: "SkillCategory");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "SkillCategory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillCategory",
                table: "SkillCategory",
                columns: new[] { "CategoryId", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillCategory",
                table: "SkillCategory");

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "SkillCategory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillCategory",
                table: "SkillCategory",
                columns: new[] { "CategoryId", "Name", "SkillId" });

            migrationBuilder.CreateIndex(
                name: "IX_SkillCategory_SkillId",
                table: "SkillCategory",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillCategory_Skill_SkillId",
                table: "SkillCategory",
                column: "SkillId",
                principalTable: "Skill",
                principalColumn: "SkillId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
