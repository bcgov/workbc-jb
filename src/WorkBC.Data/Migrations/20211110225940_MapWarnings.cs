using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class MapWarnings : Migration
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
                       ('jbSearch.errors.noMapPins'
                       ,'You have added a filter so that only virtual jobs are returned and they do not have physical work locations. Because of this, there are no locations to display. If you would like to look for jobs that have physical work locations, please remove this filter.'
                       ,'You have added a filter so that only virtual jobs are returned and they do not have physical work locations. Because of this, there are no locations to display. If you would like to look for jobs that have physical work locations, please remove this filter.'
                       ,1
                       ,1
                       ,GETDATE()
                       ,'Information message shown when there are job search results but no map pins.')");


            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
                       ([Name]
                       ,[Value]
                       ,[Description]
                       ,[FieldType]
                       ,[ModifiedByAdminUserId]
                       ,[DateUpdated]
                       ,[DefaultValue])
                 VALUES
                       ('jbSearch.errors.tooManyMapPins'
                       ,'Your search resulted in {0} jobs. The first {1} are displayed on the map based on your sorting order. Refine your search to show only the most relevant jobs.'
                       ,'Your search resulted in {0} jobs. The first {1} are displayed on the map based on your sorting order. Refine your search to show only the most relevant jobs.'
                       ,1
                       ,1
                       ,GETDATE()
                       ,'Information message shown when there are job search results but no map pins.')");

            migrationBuilder.Sql(@"update SystemSettings set Description = DefaultValue where Name = 'jbSearch.errors.noMapPins'");

            migrationBuilder.Sql(@"update SystemSettings set DefaultValue = [Value] where Name = 'jbSearch.errors.noMapPins'");

            migrationBuilder.Sql(@"update SystemSettings set DefaultValue = [Value],
                    Description = 'Information message shown when there are more than 5000 map pins.'
                    where Name = 'jbSearch.errors.tooManyMapPins'");

            migrationBuilder.Sql("Update SystemSettings Set FieldType = 2 where [Name] like '%pins%'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
