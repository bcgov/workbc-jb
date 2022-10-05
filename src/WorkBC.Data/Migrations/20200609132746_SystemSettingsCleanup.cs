using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsCleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"update systemsettings set [value] = '<strong>Ooops! The email address could not be found.</strong>
<div>Try again or create a new account.</div>' where [name] = 'jbAccount.errors.forgotPasswordEmailNotFound'");

            migrationBuilder.Sql(
                @"Update SystemSettings set [value] = Replace([value], '        ','    ') where [value] like '%        %'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}