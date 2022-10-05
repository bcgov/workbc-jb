using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class LoginFailedMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 'The password you entered is incorrect.  Please try again. You have {0} remaining before your account is locked.',
[DefaultValue] = 'The password you entered is incorrect.  Please try again. You have {0} remaining before your account is locked.'
WHERE [Name] = 'jbAccount.errors.loginFailed'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
