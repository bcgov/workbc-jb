using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SplitUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprentice",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsIndigenousPerson",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsMatureWorker",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsPersonWithDisability",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsReservist",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "SecurityAnswer",
                table: "AspNetUsers",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecurityQuestionId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JobSeekerFlags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobSeekerId = table.Column<string>(nullable: true),
                    IsApprentice = table.Column<bool>(nullable: false),
                    IsIndigenousPerson = table.Column<bool>(nullable: false),
                    IsMatureWorker = table.Column<bool>(nullable: false),
                    IsNewImmigrant = table.Column<bool>(nullable: false),
                    IsPersonWithDisability = table.Column<bool>(nullable: false),
                    IsReservist = table.Column<bool>(nullable: false),
                    IsStudent = table.Column<bool>(nullable: false),
                    IsVeteran = table.Column<bool>(nullable: false),
                    IsVisibleMinority = table.Column<bool>(nullable: false),
                    IsYouth = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerFlags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekerFlags_AspNetUsers_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SecurityQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    QuestionText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityQuestion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SecurityQuestionId",
                table: "AspNetUsers",
                column: "SecurityQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerFlags_JobSeekerId",
                table: "JobSeekerFlags",
                column: "JobSeekerId",
                unique: true,
                filter: "[JobSeekerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SecurityQuestion_SecurityQuestionId",
                table: "AspNetUsers",
                column: "SecurityQuestionId",
                principalTable: "SecurityQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            
            migrationBuilder.InsertData("SecurityQuestion",
                new[] { "Id", "QuestionText" }, 
                new object[] { 1, "My favourite TV show that isn't on anymore?" });

            migrationBuilder.InsertData("SecurityQuestion",
                new[] { "Id", "QuestionText" }, 
                new object[] { 2, "Where did my mother and father meet?" });

            migrationBuilder.InsertData("SecurityQuestion",
                new[] { "Id", "QuestionText" }, 
                new object[] { 3, "Destination of my first airplane trip?" });

            migrationBuilder.InsertData("SecurityQuestion",
                new[] { "Id", "QuestionText" }, 
                new object[] { 4, "My childhood nickname?" });

            migrationBuilder.InsertData("SecurityQuestion",
                new[] { "Id", "QuestionText" }, 
                new object[] { 5, "Name of the first company I worked for?" });

            migrationBuilder.InsertData("SecurityQuestion", 
                new[] { "Id", "QuestionText" }, 
                new object[] { 6, "Make of my first car?" });

            migrationBuilder.InsertData("SecurityQuestion", 
                new[] { "Id", "QuestionText" }, 
                new object[] { 7, "Last name of my favourite high school teacher?" });

            migrationBuilder.InsertData("SecurityQuestion", 
                new[] { "Id", "QuestionText" }, 
                new object[] { 8, "First name of my childhood best friend?" });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SecurityQuestion_SecurityQuestionId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "JobSeekerFlags");

            migrationBuilder.DropTable(
                name: "SecurityQuestion");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SecurityQuestionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SecurityAnswer",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SecurityQuestionId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsApprentice",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIndigenousPerson",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMatureWorker",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPersonWithDisability",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReservist",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }
    }
}
