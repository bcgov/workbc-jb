using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class PurgeDeletedAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from JobSeekerFlags where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from JobSeekerAdminComments where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from JobSeekerChangeLog where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from JobSeekerEventLog where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from JobAlerts where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from SavedCareerProfiles where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from JobSeekerVersions where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from SavedIndustryProfiles where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from SavedJobs where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from ImpersonationLog where AspNetUserId in (select id from AspNetUsers where AccountStatus = 99)");
            migrationBuilder.Sql("delete from AspNetUsers where AccountStatus = 99");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
