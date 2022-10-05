using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddTooltipsToSystemSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM SystemSettings where " +
                "[name] = 'shared.tooltips.jobNumberOnly' " +
                "OR [name] = 'shared.tooltips.unknownSalaries' " +
                "OR [name] = 'shared.tooltips.nocCode'"
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
                           ('shared.tooltips.unknownSalaries'
                           ,'Use this filter to include jobs that do not report a salary'
                           ,'The tooltip text for the unknown salaries option in the salary filter'
                           ,1
                           ,1
                           ,GETDATE())
                ");

            var value = @"
                            Narrow the part of a job you''d like to search by using the radio buttons. 
                            For example, if you want to find jobs from a specific employer, 
                            type the employer''s name in the keyword text box above and select the 
                            ""Employer name only"" radio button to ensure returned jobs are from a 
                            specific employer.
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
                           ('shared.tooltips.jobNumberOnly'
                           ,'{value}'
                           ,'The tooltip text for the job number only option in the job search results section'
                           ,1
                           ,1
                           ,GETDATE())
                ");

            value = @"
                The <a href=""http://noc.esdc.gc.ca/English/noc/welcome.aspx?ver=16"" 
                       target=""_blank"" style=""text-decoration: underline;"">
                       National Occupational Classification
                    </a> system classifies all occupations in Canada
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
                           ('shared.tooltips.nocCode'
                           ,'{value}'
                           ,'The tooltip html for 2016 NOC code in the more filters filter'
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
