using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class MissingOneMapPin : Migration
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
                       ('jbSearch.errors.missingOneMapPin'
                       ,'One of the jobs in your search results does not have a location specified and because of this, it does not appear on the map. All jobs that match your filter (with locations or otherwise) will appear in the list of job search results.'
                       ,'Information message shown when there are more job search results than map pins to show.'
                       ,2
                       ,1
                       ,GETDATE()
                       ,'One of the jobs in your search results does not have a location specified and because of this, it does not appear on the map. All jobs that match your filter (with locations or otherwise) will appear in the list of job search results.')");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
