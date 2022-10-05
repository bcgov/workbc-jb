using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RegistrationCopyUpdates2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update SystemSettings set [value] = 
'<strong>Why identify?</strong>
<p>Some employers encourage applications from select groups. Choose the boxes that describe how you identify. You will receive an alert when matching jobs are posted.</p>' 
WHERE NAME = 'jbAccount.shared.whyIdentify'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
