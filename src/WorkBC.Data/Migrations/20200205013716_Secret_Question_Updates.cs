using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class Secret_Question_Updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE SecurityQuestions SET QuestionText = 'What is the last name of your favourite high school teacher?' WHERE ID=1");
            migrationBuilder.Sql("UPDATE SecurityQuestions SET QuestionText = 'Where was the destination of your first airplane trip?' WHERE ID=2");
            migrationBuilder.Sql("UPDATE SecurityQuestions SET QuestionText = 'What is the name of the first company you worked for?' WHERE ID=3");
            migrationBuilder.Sql("UPDATE SecurityQuestions SET QuestionText = 'What was the make of your first car?' WHERE ID=4");
            migrationBuilder.Sql("UPDATE SecurityQuestions SET QuestionText = 'Where did you first meet your significant other?' WHERE ID=5");
            migrationBuilder.Sql("UPDATE SecurityQuestions SET QuestionText = 'What is the last name of a historical figure important to me?' WHERE ID=6");
            migrationBuilder.Sql("UPDATE SecurityQuestions SET QuestionText = 'What is your favourite food?' WHERE ID=7");
            migrationBuilder.Sql("UPDATE SecurityQuestions SET QuestionText = 'What magazine do you read most often?' WHERE ID=8");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
