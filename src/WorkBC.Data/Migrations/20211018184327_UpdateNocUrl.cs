using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateNocUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"update systemsettings set [value] = 'The <a href=""http://noc.esdc.gc.ca/"" target=""_blank"" style=""text-decoration: underline;"">National Occupational Classification</a> system classifies all occupations in Canada.', [DefaultValue] = 'The <a href=""http://noc.esdc.gc.ca/"" target=""_blank"" style=""text-decoration: underline;"">National Occupational Classification</a> system classifies all occupations in Canada.' where [name] = 'shared.tooltips.nocCode'");

            migrationBuilder.Sql(
            @"UPDATE SystemSettings SET [Value] = '<p>These recommended jobs are suggested to you if they have the same:</p>
<ul>
    <li>Job title as one of your saved jobs.</li>
    <li><a href=""http://noc.esdc.gc.ca/"" target=""_blank"">NOC</a> code as one of your saved jobs.</li>
    <li>Employer as one of your saved jobs.</li>
    <li>City listed as you specified in your Personal Settings.</li>
    <li>Self-identified group as in your Personal Settings.</li>
</ul>
<p>Jobs you have already saved to your profile will be omitted from these recommended jobs.</p>',
[DefaultValue] = '<p>These recommended jobs are suggested to you if they have the same:</p>
<ul>
    <li>Job title as one of your saved jobs.</li>
    <li><a href=""http://noc.esdc.gc.ca/"" target=""_blank"">NOC</a> code as one of your saved jobs.</li>
    <li>Employer as one of your saved jobs.</li>
    <li>City listed as you specified in your Personal Settings.</li>
    <li>Self-identified group as in your Personal Settings.</li>
</ul>
<p>Jobs you have already saved to your profile will be omitted from these recommended jobs.</p>'
WHERE [Name] = 'jbAccount.recommendedJobs.introText'");


            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = '<p>You do not have any recommended jobs yet. Jobs are suggested to you if they have the same:</p>
<ul>
    <li>Job title as one of your saved jobs.</li>
    <li><a href=""http://noc.esdc.gc.ca/"" target=""_blank"">NOC</a> code as one of your saved jobs.</li>
    <li>Employer as one of your saved jobs.</li>
    <li>City listed as you specified in your Personal Settings.</li>
    <li>Self-identified group as in your Personal Settings.</li>
</ul>
<p class=""no-recommended-jobs""> To increase the number of recommendations, you can either <a href=""#/saved-jobs"">save a job</a>, <a href=""#/personal-settings#location"">change your location to a city in B.C.</a> or <a href=""#/personal-settings"">add group(s) that you self-identify as</a> in your <em>Personal Settings</em>. </p>',
[DefaultValue] = '<p>You do not have any recommended jobs yet. Jobs are suggested to you if they have the same:</p>
<ul>
    <li>Job title as one of your saved jobs.</li>
    <li><a href=""http://noc.esdc.gc.ca/"" target=""_blank"">NOC</a> code as one of your saved jobs.</li>
    <li>Employer as one of your saved jobs.</li>
    <li>City listed as you specified in your Personal Settings.</li>
    <li>Self-identified group as in your Personal Settings.</li>
</ul>
<p class=""no-recommended-jobs""> To increase the number of recommendations, you can either <a href=""#/saved-jobs"">save a job</a>, <a href=""#/personal-settings#location"">change your location to a city in B.C.</a> or <a href=""#/personal-settings"">add group(s) that you self-identify as</a> in your <em>Personal Settings</em>. </p>'
WHERE [Name] = 'jbAccount.recommendedJobs.introTextNoRecommendedJobs'");

            migrationBuilder.Sql(
                @"update systemsettings set [DefaultValue] = [Value] WHERE [Name] in ('jbAccount.errors.forgotPasswordInvalidToken','jbAccount.login.forgotPasswordConfirmationBody')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
