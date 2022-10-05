using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RegistrationCopyUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update SystemSettings set [value] = 'Passwords must be at least six characters long, with at least one upper-case letter and one number.' where [name] = 'jbAccount.shared.passwordComplexity'");
            migrationBuilder.Sql(@"update SystemSettings set [value] = 'Some employers encourage applications from select groups. Choose the boxes that describe how you identify. You will receive an alert when matching jobs are posted.', [Description] = 'Explanatory text for the ""How do you identify?"" section of the registration and personal settings pages' where [name] = 'jbAccount.shared.whyIdentify'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
