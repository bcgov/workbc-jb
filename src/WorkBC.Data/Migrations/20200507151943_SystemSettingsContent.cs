using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM SYSTEMSETTINGS WHERE [NAME] IN (
                                        'jbAccount.registration.confirmationBody',
                                        'jbAccount.registration.confirmationTitle ',
                                        'jbAccount.registration.activationTitle',
                                        'jbAccount.registration.activationBody',
                                        'jbAccount.dashboard.introText',
                                        'jbAccount.dashboard.jobsDescription',
                                        'jbAccount.dashboard.careersDescription',
                                        'jbAccount.dashboard.accountDescription',
                                        'jbAccount.dashboard.resource1Title',
                                        'jbAccount.dashboard.resource1Body',
                                        'jbAccount.dashboard.resource1Url',
                                        'jbAccount.dashboard.resource2Title',
                                        'jbAccount.dashboard.resource2Body',
                                        'jbAccount.dashboard.resource2Url',
                                        'jbAccount.dashboard.resource3Title',
                                        'jbAccount.dashboard.resource3Body',
                                        'jbAccount.dashboard.resource3Url',
                                        'jbSearch.jobDetail.callToAction1Intro',
                                        'jbSearch.jobDetail.callToAction1Title',
                                        'jbSearch.jobDetail.callToAction1BodyText',
                                        'jbSearch.jobDetail.callToAction1LinkText',
                                        'jbSearch.jobDetail.callToAction1LinkUrl ',
                                        'jbSearch.jobDetail.callToAction2Intro',
                                        'jbSearch.jobDetail.callToAction2Title',
                                        'jbSearch.jobDetail.callToAction2BodyText',
                                        'jbSearch.jobDetail.callToAction2LinkText',
                                        'jbSearch.jobDetail.callToAction2LinkUrl',
                                        'shared.filters.howIsSalaryCalculatedBody',
                                        'shared.filters.howIsSalaryCalculatedTitle',
                                        'shared.filters.locationRadiusNote',
                                        'shared.filters.educationNote',
                                        'shared.filters.locationTitle',
                                        'shared.filters.locationSearchLabel',
                                        'shared.filters.locationRegionSearchLabel',
                                        'shared.filters.jobTypeTitle',
                                        'shared.filters.salaryTitle',
                                        'shared.filters.industryTitle',
                                        'shared.filters.educationTitle',
                                        'shared.filters.datePostedTitle',
                                        'shared.filters.moreFiltersTitle',
                                        'jbAccount.recommendedJobs.introText',
                                        'jbAccount.recommendedJobs.introTextNoRecommendedJobs',
                                        'jbAccount.recommendedJobs.filterIntroText ',
                                        'jbAccount.jobAlerts.noEmailHelpQuestion',
                                        'jbAccount.jobAlerts.noEmailHelpAnswer',
                                        'jbAccount.careerProfiles.callToAction1BodyText',
                                        'jbAccount.careerProfiles.callToAction1LinkText',
                                        'jbAccount.careerProfiles.callToAction1LinkUrl',
                                        'jbAccount.careerProfiles.callToAction2BodyText',
                                        'jbAccount.careerProfiles.callToAction2LinkText',
                                        'jbAccount.careerProfiles.callToAction2LinkUrl',
                                        'jbAccount.industryProfiles.callToAction1BodyText',
                                        'jbAccount.industryProfiles.callToAction1LinkText',
                                        'jbAccount.industryProfiles.callToAction1LinkUrl',
                                        'jbAccount.industryProfiles.callToAction2BodyText',
                                        'jbAccount.industryProfiles.callToAction2LinkText',
                                        'jbAccount.industryProfiles.callToAction2LinkUrl',
                                        'jbAccount.shared.whyIdentify',
                                        'jbAccount.shared.passwordComplexity',
                                        'jbAccount.login.forgotPasswordIntroText')");

            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.registration.confirmationBody','<p>
    <strong>
        An activation email has been sent to {0}. Please follow the instructions in that email to activate your account.
    </strong>
</p>
<p>
    <i>
        If you have not received the activation email, try checking your junk folder. 
        If you use a spam or security filter for your emails, make sure to configure it to allow messages from noreply@gov.bc.ca. 
        You can also resend the activation email.
    </i>
</p>','The body message displayed after a user completes the registration form.  Placeholder {0} is for the user''s email address.',5,1,GETDATE())");

            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.registration.confirmationTitle ','Thanks for registering!','The title of the message displayed after a user completes the registration form',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.registration.activationTitle','You have successfully activated your account!','The title of the message displayed after a user clicks the email confirmation link to activate their account',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.registration.activationBody','You are now being redirected to log in or you can <a href=""#/account/login"">click here</a>.','The body of the message displayed after a user clicks the email confirmation link to activate their account.',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.introText','Manage and personalize the job search process in your account. 
<br>
Find job opportunities that match your skills and experience, and learn about careers and industries in B.C.','Text displayed at the top of the user dashboard',5,1,GETDATE())");
    
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.jobsDescription','Stay organized by saving your favourite jobs and employers. You can view and manage then without having to run a search.','Text displayed in the Jobs widget on the account dashboard',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.careersDescription','Explore and save your favourite career and industry profiles to help you decide on the right career path for you.','Text displayed in the Careers & Industries widget on the account dashboard',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.accountDescription','Keep your account up-to-date. Access and manage your personal settings.','Text displayed in the Manage Account widget on the account dashboard.',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource1Title','View high demand occupations','Title for the 1st recommended resource at the bottom of the account dashboard',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource1Body','The occupations that are expected to have above-average opportunities are highlighted in this section.','Description for the 1st recommended resource at the bottom of the account dashboard',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource1Url','/Labour-Market-Industry/High-Opportunity-Occupations.aspx','URL for the 1st recommended resource at the bottom of the account dashboard',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource2Title','Visit a WorkBC Centre','Title for the 2nd recommended resource at the bottom of the account dashboard',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource2Body','WorkBC Employment Services Centres support you in finding a job and keepint it.','Description for the 2nd recommended resource at the bottom of the account dashboard',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource2Url','/Employment-Services/WorkBC-Centres.aspx','URL for the 2nd recommended resource at the bottom of the account dashboard',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource3Title','Explore Regional Profiles','Title for the 3rd recommended resource at the bottom of the account dashboard',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource3Body','Find labour market details on each of the seven regions in B.C., as well as the employment outlooks for the years to come.','Description for the 3rd recommended resource at the bottom of the account dashboard',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.dashboard.resource3Url','/Labour-Market-Industry/Regional-Profiles.aspx','URL for the 3rd recommended resource at the bottom of the account dashboard',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction1Intro','CAREER PROFILES','Introductory  title for the 1st call-to-action on the job detail page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction1Title','EXPLORE CAREERS','Main title for the 1st call-to-action on the job detail page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction1BodyText','Get the details you need on 500 jobs - including duties, education required, salary and employment outlook.','Description for the 1st call-to-action on the job detail page',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction1LinkText','View Career Profiles','Button text for the 1st call-to-action on the job detail page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction1LinkUrl ','/Jobs-Careers/Explore-Careers.aspx','Link URL for the 1st call-to-action on the job detail page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction2Intro','LABOUR MARKET INFORMATION','Introductory  title for the 2nd call-to-action on the job detail page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction2Title','HIGH DEMAND JOBS','Main title for the 2nd call-to-action on the job detail page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction2BodyText','Discover jobs that are expected to be in high demand over the next decade.','Description for the 2nd call-to-action on the job detail page',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction2LinkText','View High Demand Jobs','Button text for the 2nd call-to-action on the job detail page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbSearch.jobDetail.callToAction2LinkUrl','/Labour-Market-Industry/High-Demand-Occupations.aspx','Link URL for the 2nd call-to-action on the job detail page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.howIsSalaryCalculatedBody','<p>When you select a value from the salary drop-down menu, that figure is automatically converted to the equivalent annual salary even when a pay period other than Annually is selected. </p>
<p>The conversion is calculated as follows:</p>
<ul>
    <li>Hourly - hourly rate is multiplied by 2080 (approximate number of work hours in a year)</li>
    <li>Weekly - weekly rate is multiplied by 52 (number of weeks in a year)</li>
    <li>Bi-Weekly - bi-weekly rate is multiplied by 26 (half the number of weeks in a year)</li>
    <li>Monthly - monthly rate is multiplied by 12 (number of months in a year)</li>
</ul>
<p>Using this conversion, the search is able to return results matching your salary requirements regardless of whether you select Annually, Hourly, Weekly, Bi-weekly or Monthly as the pay period. The search will include all jobs posted with a salary equal to or higher than the amount selected.</p>
<p>Example: A user searches for salaries: $15 hourly. This will be converted by multiplying $15 by the conversion factor of 2080 (in this case, approximate number of work hours in a year)</p>
<p>Salary searches apply to jobs where employers publicly posted salary amounts (e.g., annual, monthly, bi-weekly, weekly or hourly). Note: Some employers do not specify a salary amount.</p>','Explanatory text that appears at the bottom of the salary filter',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.howIsSalaryCalculatedTitle','How is salary calculated?',' Title for the explanatory text at the bottom of the salary filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.locationRadiusNote','<i>Note: You can select a search radius around a <strong>single city or postal code</strong>. The search radius doesn''t apply when you enter more than one location.</i>','Explanatory text that appears below the radius dropdown on the location filter',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.educationNote','<i>Note: We use an algorithm to get minimum education levels for job postings that are external to WorkBC. Because of this, you may see fewer results than you were expecting. If this is the case then try removing this filter.</i>','Explanatory text that appears on the education filter',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.locationTitle','Filter by city name, postal code or region','Main title for the location filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.locationSearchLabel','Narrow down by city name or postal code','The label of the city or postal code search input on the location filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.locationRegionSearchLabel','Narrow down by region','The label for the region checkboxes on the location filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.jobTypeTitle','Select all options that apply','Main title for the job type filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.salaryTitle','Salary and Benefits','Main title for the salary filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.industryTitle','Industry','Main title for the industry filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.educationTitle','Minimum education required','Main title for the education filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.datePostedTitle','Date posted','Main title for the date posted filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.moreFiltersTitle','Additional job filters','Main title for the more filters filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.recommendedJobs.introText','<p>Welcome to your recommended jobs! Jobs are suggested to you if they:</p>
<ul>
    <li>have the same job title as one of your saved jobs</li>
    <li>have the same <a href=""""http://noc.esdc.gc.ca/English/noc/welcome.aspx?ver=16"""" target=""""_blank"""">NOC</a> code as one of your saved jobs</li>
    <li>have the same employer one of your saved jobs</li>
    <li>are in the same city as you specified in your <em>Personal Settings</em>, or if</li>
    <li>an employer has chosen to encourage applicants from a group you have self-identified as in your <em>Personal Settings</em>. </li>
</ul>
<p>To make sure the job recommendations that come up are the most meaningful to you, we have not included jobs that you have already saved in your profile.</p>','Introductory text that appears at the top of the recommended jobs page',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.recommendedJobs.introTextNoRecommendedJobs','<p>You do not have any recommended jobs yet. Jobs are suggested to you if they:</p>
<ul>
    <li>have the same job title as one of your saved jobs</li>
    <li>have the same <a href=""""http://noc.esdc.gc.ca/English/noc/welcome.aspx?ver=16"""" target=""""_blank"""">NOC</a> code as one of your saved jobs</li>
    <li>have the same employer one of your saved jobs</li>
    <li>are in the same city as you specified in your <em>Personal Settings</em>, or if</li>
    <li>an employer has chosen to encourage applicants from a group you have self-identified as in your <em>Personal Settings</em>. </li>
</ul>
<p class=""""no-recommended-jobs""""> To increase the number of recommendations, you can either <a href=""""#/account/saved-jobs"""">save a job</a>, <a href=""""#/account/personal-settings#location"""">change your city</a> or <a href=""""#/account/personal-settings"""">add group/s that you self-identify as</a> in your <em>Personal Settings</em>. </p>','Introductory text that appears at the top of the recommended jobs page for users with no recommended jobs',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.recommendedJobs.filterIntroText ','You can also filter your recommended jobs by adjusting the reason for the recommendations below.','Explanatory text at the top of the recommended jobs filter',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.jobAlerts.noEmailHelpQuestion',' Where do I find my Job Alerts if email notifications are turned off? ','Title for the explanatory text on the create/edit job alert page for users who select an email alert frequency of ""Never""',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.jobAlerts.noEmailHelpAnswer','<p class=""""pl-2"""">
    If you''ve turned your email notifications off, you will need to log in to your account and navigate to Job Alerts to view jobs that match an alert you''ve created. Once you''re logged in, you can access Job Alerts in two ways:
</p>
<ul>
    <li>Click on <strong>Job Alerts</strong> under the <em>Jobs</em> section in your Account Dashboard</li>
    <li> From the top navigation in your profile, hover your mouse over the <em>Jobs</em> menu and click on <strong>Job Alerts</strong></li>
 </ul>','Explanatory text on the create/edit job alert page for users who select an email alert frequency of ""Never""',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.careerProfiles.callToAction1BodyText','Discover over 500 career options and learn about the duties, salary, education, job prospects, and much more.','Description for the 1st call-to-action on the save career profiles page',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.careerProfiles.callToAction1LinkText','Search Career Profiles','Link text for the 1st call-to-action on the save career profiles page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.careerProfiles.callToAction1LinkUrl','/Jobs-Careers/Explore-Careers.aspx','Link URL for the 1st call-to-action on the save career profiles page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.careerProfiles.callToAction2BodyText','Learn about the kinds of jobs and skills that will be most in demand for the next 10 years.','Description for the 2nd call-to-action on the save career profiles page',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.careerProfiles.callToAction2LinkText','Explore Industry and Sector Outlooks','Link text for the 2nd call-to-action on the save career profiles page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.careerProfiles.callToAction2LinkUrl','/Labour-Market-Industry/Labour-Market-Outlook.aspx','Link URL for the 2nd call-to-action on the save career profiles page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.industryProfiles.callToAction1BodyText','See data for B.C.''s major industries, including employment trends, earning potential and more.','Description for the 1st call-to-action on the save industry profiles page',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.industryProfiles.callToAction1LinkText','View Industry Profiles','Link text for the 1st call-to-action on the save industry profiles page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.industryProfiles.callToAction1LinkUrl','/Labour-Market-Industry/Industry-and-Sector-Information/Industry-Profiles.aspx','Link URL for the 1st call-to-action on the save industry profiles page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.industryProfiles.callToAction2BodyText','Use the sub-industry outlooks to identify trends and opportunities, and help you make career decisions.','Description for the 2nd call-to-action on the save industry profiles page',2,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.industryProfiles.callToAction2LinkText','Explore Industry and Sector Outlooks','Link text for the 2nd call-to-action on the save industry profiles page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.industryProfiles.callToAction2LinkUrl','/Labour-Market-Industry/Labour-Market-Outlook.aspx','Link URL for the 2nd call-to-action on the save industry profiles page',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.shared.whyIdentify','<strong>Why identify?</strong>
<p> Some employers specifically encourage applications from these groups. By filling out this section, we will alert you when jobs are posted that encourage applications from these groups of people. </p>','Explanatory text for the ""Do you identify as?"" section of the registration and personal settings pages',5,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.shared.passwordComplexity','Passwords must be at least 6 characters long, with at least 1 upper-case letter and 1 number','Text explaining our password complexity rules',1,1,GETDATE())");
            
            migrationBuilder.Sql(
                @"INSERT INTO [dbo].[SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('jbAccount.login.forgotPasswordIntroText','To reset your password, type the email address you use to log into your account.','Introductory text on the reset password dialog',1,1,GETDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}