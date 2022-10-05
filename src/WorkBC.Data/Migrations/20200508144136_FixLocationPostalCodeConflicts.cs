using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FixLocationPostalCodeConflicts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "update Locations set City = 'Blunden Harbour', Label = 'Blunden Harbour' where LocationId = 1469");
            migrationBuilder.Sql(
                "update Locations set City = 'Lulu Upper Similkameen Band 5', Label = 'Lulu Upper Similkameen Band 5' where LocationId = 1183");
            migrationBuilder.Sql(
                "update Locations set City = 'Snake Fort Nelson Band 5', Label = 'Snake Fort Nelson Band 5' where LocationId = 1809");
            migrationBuilder.Sql("update locations set IsHidden = 1 where LocationId = 43");
            migrationBuilder.Sql(
                "update locations set City = '93 Mile House', Label = '93 Mile House' where LocationId = 50");
            migrationBuilder.Sql("update locations set Label = '93 Mile House', IsHidden = 1 where LocationId = 1395");
            migrationBuilder.Sql("update locations set Label = '70 Mile House', IsHidden = 1 where LocationId = 1736");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}