using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class MissingMapPins : Migration
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
                       ('jbSearch.errors.missingMapPins'
                       ,'{0} of the jobs in your search results do not have a location specified and because of this, they do not appear on the map. All jobs that match your filter (with locations or otherwise) will appear in the list of job search results.'
                       ,'Information message shown when there are more job search results than map pins to show.'
                       ,2
                       ,1
                       ,GETDATE()
                       ,'{0} of the jobs in your search results do not have a location specified and because of this, they do not appear on the map. All jobs that match your filter (with locations or otherwise) will appear in the list of job search results.')");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
