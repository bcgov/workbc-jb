using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class DefaultSearchRadius : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
                                       ([Name]
                                       ,[Value]
                                       ,[Description]
                                       ,[FieldType]
                                       ,[ModifiedByAdminUserId]
                                       ,[DateUpdated])
                                 VALUES
                                       ('shared.settings.defaultSearchRadius'
                                       ,'15'
                                       ,'Default radius (km) for location searches. Valid values are 10, 15, 25, 50, 75 or 100. If you enter and invalid value then 15 will be used instead.'
                                       ,3
                                       ,1
                                       ,GetDate())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}