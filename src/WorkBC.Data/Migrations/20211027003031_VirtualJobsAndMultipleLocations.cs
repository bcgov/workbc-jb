using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class VirtualJobsAndMultipleLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO [dbo].[Regions]
           ([Id]
           ,[Name]
           ,[ListOrder]
           ,[IsHidden])
     VALUES
           (-4
           ,'Virtual Jobs'
           ,1004
           ,1)");

            migrationBuilder.Sql(@"
INSERT INTO [dbo].[Regions]
           ([Id]
           ,[Name]
           ,[ListOrder]
           ,[IsHidden])
     VALUES
           (-5
           ,'Multiple Locations'
           ,1005
           ,1)
");

            migrationBuilder.Sql(@"
INSERT INTO [dbo].[Locations]
           ([LocationId]
           ,[EDM_Location_DistrictLocationId]
           ,[RegionId]
           ,[FederalCityId]
           ,[City]
           ,[Label]
           ,[IsDuplicate]
           ,[IsHidden]
           ,[Latitude]
           ,[Longitude]
           ,[BcStatsPlaceId])
     VALUES
           (-4
           ,1
           ,-4
           ,null
           ,'Virtual Jobs'
           ,'Virtual Jobs'
           ,0
           ,1
           ,'54.5000992'
           ,'-125.1159973'
           ,null)");

            migrationBuilder.Sql(@"
INSERT INTO [dbo].[Locations]
           ([LocationId]
           ,[EDM_Location_DistrictLocationId]
           ,[RegionId]
           ,[FederalCityId]
           ,[City]
           ,[Label]
           ,[IsDuplicate]
           ,[IsHidden]
           ,[Latitude]
           ,[Longitude]
           ,[BcStatsPlaceId])
     VALUES
           (-5
           ,1
           ,-5
           ,null
           ,'Multiple Locations'
           ,'Multiple Locations'
           ,0
           ,1
           ,'54.5000992'
           ,'-125.1159973'
           ,null)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Regions where id in (-4,-5)");
            migrationBuilder.Sql("delete from Locations where LocationId in (-4,-5)");
        }
    }
}
