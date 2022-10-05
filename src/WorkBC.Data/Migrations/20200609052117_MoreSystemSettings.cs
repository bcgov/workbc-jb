using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class MoreSystemSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Description] = 'The body of the message displayed after a user completes the registration form.  Placeholder {0} is for the user''s email address.' WHERE [name] = 'jbAccount.registration.confirmationBody'");

            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings] 
           ([Name]
           ,[Value]
           ,[Description]
           ,[FieldType]
           ,[ModifiedByAdminUserId]
           ,[DateUpdated])
     VALUES
           ('jbAccount.login.forgotPasswordConfirmationBody'
           ,'<p><strong>We''ve sent your password reset instructions to {0}.</strong></p>
<p><i>If you cannot find the email, try checking your junk folder. If you use a spam or security filter, change your email settings to allow messages from noreply@gov.bc.ca.</i></p>
<p><i>For help, contact the WorkBC.ca call centre at: <span class=""text-nowrap"">250-952-6914</span> or toll free at: <span class=""text-nowrap"">1-877-952-6914.</span></i></p>'
           , 'The body of the message displayed after a user resets their password. Placeholder {0} is for the user''s email address.'
           ,5
           ,1
           ,getdate())");

            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
           ([Name]
           ,[Value]
           ,[Description]
           ,[FieldType]
           ,[ModifiedByAdminUserId]
           ,[DateUpdated])
     VALUES
           ('jbAccount.login.forgotPasswordConfirmationTitle'
           ,'Check Your Email'
           ,'The title of the message displayed after a user resets their password.'
           ,1
           ,1
           ,getdate())");

            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
           ([Name]
           ,[Value]
           ,[Description]
           ,[FieldType]
           ,[ModifiedByAdminUserId]
           ,[DateUpdated])
     VALUES
           ('shared.filters.keywordInputPlaceholder'
           ,'Keyword(s)'
           ,'Placeholder text for the main keyword search input'
           ,1
           ,1
           ,getdate())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
