using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class CappedResultsWarning : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
                       ([Name]
                       ,[Value]
                       ,[Description]
                       ,[FieldType]
                       ,[ModifiedByAdminUserId]
                       ,[DateUpdated]
                       ,[DefaultValue])
                 VALUES
                       ('jbSearch.errors.cappedResultsWarning'
                       ,'The first {0} jobs are displayed but there are more jobs that may be relevant to you. Narrow your search results by adding a keyword(s), location or applying another filter.'
                       ,'Information message shown on the last page when there are more than 10,000 search results.'
                       ,2
                       ,1
                       ,GETDATE()
                       ,'The first {0} jobs are displayed but there are more jobs that may be relevant to you. Narrow your search results by adding a keyword(s), location or applying another filter.')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
