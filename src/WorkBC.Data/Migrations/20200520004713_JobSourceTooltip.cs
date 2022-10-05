using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobSourceTooltip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "update Industries set title = 'Management of companies and enterprises' where title = 'Management of Companies and Enterprises'");

            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
           ([Name]
           ,[Value]
           ,[Description]
           ,[FieldType]
           ,[ModifiedByAdminUserId]
           ,[DateUpdated])
     VALUES
           ('shared.tooltips.jobSource'
           ,'WorkBC jobs are verified by the National Job Bank. External jobs are B.C. job postings from other job boards (e.g., Monster and Indeed).'
           ,'Tooltip for Job Source in the more filters filter'
           ,1
           ,1
           ,GETDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
