using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddErrorsToSystemSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM SystemSettings where " +
                "[name] = 'jbSearch.errors.invalidPostalCode' " +
                "OR [name] = 'jbSearch.errors.duplicatePostal' " +
                "OR [name] = 'jbSearch.errors.duplicateCity' " +
                "OR [name] = 'jbSearch.search.noSearchResults' " +
                "OR [name] = 'shared.errors.jobAlertTitleRequired' " +
                "OR [name] = 'shared.errors.jobAlertTitleDuplicate' " +
                "OR [name] = 'jbSearch.errors.invalidCity'"
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
                           ('shared.errors.jobAlertTitleDuplicate'
                           ,'The title already exists'
                           ,'Error message for existing job alert title'
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
                           ('shared.errors.jobAlertTitleRequired'
                           ,'Please specify a title for this alert'
                           ,'Error message for required job alert title'
                           ,1
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbSearch.errors.invalidPostalCode'
                           ,'The postal code <strong>{0}</strong> is invalid. Please ensure the format is correct.'
                           ,'Error message for invalid postal code / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbSearch.errors.invalidCity'
                           ,'The city <strong>{0}</strong> could not be found. Please ensure the spelling is correct.'
                           ,'Error message for invalid city / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbSearch.errors.duplicatePostal'
                           ,'The postal code <strong>{0}</strong> has already been added.'
                           ,'Error message for duplicate postal / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbSearch.errors.duplicateCity'
                           ,'The city <strong>{0}</strong> has already been added.'
                           ,'Error message for duplicate city / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            var value = 
@"<h4>There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
    <li>Check your spelling</li>
    <li>Try broader search terms</li>
    <li>Use different synonyms</li>
    <li>Replace abbreviations with the entire word</li>
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
                           ('jbSearch.search.noSearchResults'
                           ,'{value}'
                           ,'Error message for no search results / HTML version'
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
