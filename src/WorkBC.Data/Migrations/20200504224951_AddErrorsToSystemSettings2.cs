using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddErrorsToSystemSettings2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM SystemSettings where " +
                "[name] = 'jbAccount.errors.termsOfUseRequired' " +
                "OR [name] = 'jbAccount.errors.loginFailed' " +
                "OR [name] = 'jbAccount.errors.forgotPasswordEmailNotFound' " +
                "OR [name] = 'jbAccount.errors.invalidEmail' " +
                "OR [name] = 'jbAccount.errors.recommendedJobsNoResultsOneCheckbox' " +
                "OR [name] = 'jbAccount.errors.recommendedJobsNoResultsMultipleCheckboxes' " +
                "OR [name] = 'jbAccount.errors.forgotPasswordInvalidToken'"
                );

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.errors.invalidEmail'
                           ,'Please enter a valid email address'
                           ,'Error message for invalid email'
                           ,1
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.errors.loginFailed'
                           ,'The password you entered is incorrect'
                           ,'Error message for incorrect password'
                           ,1
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.errors.termsOfUseRequired'
                           ,'You must agree to the WorkBC Terms of Use before continuing'
                           ,'Error message for required terms of use'
                           ,1
                           ,1
                           ,GETDATE())
                ");

            var value = 
@"<strong>Oops! The email address could not be found.</strong>
<div>Try again or create a new account.</div>
";

            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.errors.forgotPasswordEmailNotFound'
                           ,'{value}'
                           ,'Error message for forgot password - email not found / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            value = 
@"<h4 class=""mt-4"">There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
    <li>Clear all your filters</li>
    <l >Select a different filter</li>
</ul>
";

            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.errors.recommendedJobsNoResultsOneCheckbox'
                           ,'{value}'
                           ,'Error message for no recommended jobs based on one checkbox / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            value = 
@"<h4 class=""mt-4"">There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
    <li>Clear all your filters</li>
    <li>Add the filters back one at a time</li>
    <li>Click the Apply Filter button after every selection</li>
</ul>
";

            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.errors.recommendedJobsNoResultsMultipleCheckboxes'
                           ,'{value}'
                           ,'Error message for no recommended jobs based on multiple checkboxes / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            value = 
@"<h3>Invalid Link</h3>
<p>The link has expired. <a href=""#/account/login"">Click here</a> to login.</p>
";

            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.errors.forgotPasswordInvalidToken'
                           ,'{value}'
                           ,'Error message for invalid password reset token / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
