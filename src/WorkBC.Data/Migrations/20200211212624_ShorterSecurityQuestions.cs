using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ShorterSecurityQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE SecurityQuestions SET QuestionText = 'Where was my first airplane trip to?' WHERE ID=1");
            migrationBuilder.Sql(
                "UPDATE SecurityQuestions SET QuestionText = 'What is my mother’s middle name?' WHERE ID=2");
            migrationBuilder.Sql(
                "UPDATE SecurityQuestions SET QuestionText = 'Where did I first meet my partner?' WHERE ID=3");
            migrationBuilder.Sql(
                "UPDATE SecurityQuestions SET QuestionText = 'What was the make of my first car?' WHERE ID=4");
            migrationBuilder.Sql(
                "UPDATE SecurityQuestions SET QuestionText = 'Who is my favourite author?' WHERE ID=5");
            migrationBuilder.Sql(
                "UPDATE SecurityQuestions SET QuestionText = 'What was my childhood nickname?' WHERE ID=6");
            migrationBuilder.Sql(
                "UPDATE SecurityQuestions SET QuestionText = 'Which magazine do I read most often?' WHERE ID=7");
            migrationBuilder.Sql(
                "UPDATE SecurityQuestions SET QuestionText = 'What is the name of my first pet?' WHERE ID=8");
            migrationBuilder.Sql(
                "insert into SecurityQuestions ([Id],[QuestionText]) Values (9,'Where did my mother and father meet?')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from SecurityQuestions where id = 9");
        }
    }
}