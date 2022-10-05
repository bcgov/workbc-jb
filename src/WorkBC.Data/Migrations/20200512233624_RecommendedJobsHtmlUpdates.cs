using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RecommendedJobsHtmlUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"update SystemSettings 
                  set [Value] = Replace([Value], '""""', '''')  
                  where [Name] in (
                                    'jbAccount.recommendedJobs.introText', 
                                    'jbAccount.recommendedJobs.introTextNoRecommendedJobs'
                                  )
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
