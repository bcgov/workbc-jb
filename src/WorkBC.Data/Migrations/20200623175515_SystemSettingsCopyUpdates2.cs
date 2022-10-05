using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsCopyUpdates2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update SystemSettings set
                    [DefaultValue] = 'Stay organized by saving, viewing and managing your favourite jobs.'
                WHERE [name] = 'jbAccount.dashboard.jobsDescription'");

            migrationBuilder.Sql(@"update SystemSettings set
                    [DefaultValue] = 'Explore and save your favourite career and industry profiles to help you decide on the right career path.'
                WHERE [name] = 'jbAccount.dashboard.careersDescription'");

            migrationBuilder.Sql(@"update SystemSettings set [Value] = [DefaultValue]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
