using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddedConfigurationEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [ConfigurationSettings] ([Name], [Value], [Description], [FieldType], [JbAccount], [JbSearch]) VALUES ('jbSearch.job.newJobStatus', '3', 'A job is considered new if it was posted withing this number of days', 3, 0, 1)");
            migrationBuilder.Sql("INSERT INTO [ConfigurationSettings] ([Name], [Value], [Description], [FieldType], [JbAccount], [JbSearch]) VALUES ('jbAccount.jobAlert.email.newAlert.emailBody', 'Dear Client \n\nThere\'\'s new jobs based on your job alert.\nPlease see the list below containing a list of jobs that\'\'s matching your job alert criteria.\n\n\nThe WorkBC team', 'This text will be used when sending a job-seeker an email notifying them of new jobs matching their job alert settings.', 2, 1, 0)");
            migrationBuilder.Sql("INSERT INTO [ConfigurationSettings] ([Name], [Value], [Description], [FieldType], [JbAccount], [JbSearch]) VALUES ('jbSearch.filters.moreFilters.toolTip.youth', 'Young people between 15 and 30 years of age', 'The tooltip text for the youth option in the more filters filter.', 1, 0, 1)");
            migrationBuilder.Sql("INSERT INTO [ConfigurationSettings] ([Name], [Value], [Description], [FieldType], [JbAccount], [JbSearch]) VALUES ('jbAccount.jobSeeker.survey.active', '0', 'Toggle the job seekers survey on or off', 4, 1, 0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
