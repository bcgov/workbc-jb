using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ResetPasswordCopyChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE SystemSettings SET [Value] = 'Please check your email.' WHERE [Name] = 'jbAccount.login.forgotPasswordConfirmationTitle'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}