using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class WhyIdentifyCopyUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE SystemSettings SET [Value] = 
'<strong>Why identify?</strong>
<p> Some employers encourage applications from these groups. Select those that apply and we will alert you when jobs are posted that match. </p>'
WHERE NAME = 'jbAccount.shared.whyIdentify'"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}