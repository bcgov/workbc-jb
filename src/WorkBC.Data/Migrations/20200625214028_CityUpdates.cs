using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class CityUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE Locations SET IsHidden = 1 WHERE locationid in (793,1472)
GO

UPDATE Locations SET City = 'Fort Nelson Band 2', [Label] = 'Fort Nelson Band 2' WHERE locationid = 727
GO

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
VALUES (2201,30,7,NULL,'West Kelowna','West Kelowna',0,0,'49.86299','-119.56891',2187)
GO

INSERT INTO [dbo].[GeocodedLocationCache]
           ([Name]
           ,[Latitude]
           ,[Longitude]
           ,[DateGeocoded]
           ,[IsPermanent])
     VALUES
           ('West Kelowna, BC, CANADA'
           ,'49.86299'
           ,'-119.56891'
           ,'2019-01-01 00:00:00.0000000'
           ,1)
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
