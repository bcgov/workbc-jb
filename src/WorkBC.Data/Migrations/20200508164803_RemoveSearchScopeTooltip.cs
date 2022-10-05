using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RemoveSearchScopeTooltip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM SystemSettings WHERE [NAME] = 'shared.tooltips.jobNumberOnly'");

            migrationBuilder.Sql(@"
update SystemSettings
set Description = Description + '. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl, {4} = emailSubject.'
where [name] = 'email.jobAlert.bodyHtml';

update SystemSettings
set Description = Description + '. {0} = firstName, {1} = frequency, {2} = jobAlertTitle, {3} = notificationUrl.'
where [name] = 'email.jobAlert.bodyText';

update SystemSettings
set Description = Description + '. {0} = jobAlertTitle.'
where [name] = 'email.jobAlert.subject';

update SystemSettings
set Description = Description + '. {0} = firstName, {1} = lastName,  {2} = linkUrl, {3} = emailSubject.'
where [name] = 'email.passwordReset.bodyHtml';

update SystemSettings
set Description = Description + '. {0} = firstName, {1} = lastName, {2} = linkUrl.'
where [name] = 'email.passwordReset.bodyText';

update SystemSettings
set Description = Description + '. {0} = firstName, {1} = linkUrl, {2} = emailSubject.'
where [name] = 'email.registration.bodyHtml';

update SystemSettings
set Description = Description + '. {0} = firstName, {1} = linkUrl.'
where [name] = 'email.registration.bodyText';

update SystemSettings 
set Description = 'The body message displayed after a user completes the registration form. {0} = emailAddress.'
where [name] = 'jbAccount.registration.confirmationBody';

update SystemSettings 
set Description = 'Error message displayed when a search returns zero results'
where [name] = 'jbSearch.errors.noSearchResults';

update SystemSettings 
set Value = 'The <a href=""http://noc.esdc.gc.ca/English/noc/welcome.aspx?ver=16"" target=""_blank"" style=""text-decoration: underline;"">National Occupational Classification</a> system classifies all occupations in Canada'
where [name] = 'shared.tooltips.nocCode';

");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}