using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateJobDetailNameForAction1LinkUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"update SystemSettings 
                  set [Name] = 'jbSearch.jobDetail.callToAction1LinkUrl'  
                  where [Name] like 'jbSearch.jobDetail.callToAction1LinkUrl%'
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
