using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ReportMetaDataUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Update [JobSeekerStatLabels] set [Label] = 'Indigenous' where [Label] = 'Indigenous Person'");

            migrationBuilder.Sql(@"Update [JobSeekerStatLabels] set [Label] = 'Mature' where [Label] = 'Mature Worker'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
