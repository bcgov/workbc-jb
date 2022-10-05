using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsLabelStandardization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update SystemSettings set name = 'jbSearch.settings.newJobPeriodDays' where name = 'jbSearch.job.newJobStatus'");
            migrationBuilder.Sql("update SystemSettings set name = 'email.jobAlert.body' where name = 'jbAccount.jobAlert.email.newAlert.emailBody'");
            migrationBuilder.Sql("update SystemSettings set name = 'jbSearch.tooltips.youth' where name = 'jbSearch.filters.moreFilters.toolTip.youth'");
            migrationBuilder.Sql("update SystemSettings set name = 'jbAccount.settings.surveyEnabled' where name = 'jbAccount.jobSeeker.survey.active'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
