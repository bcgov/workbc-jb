using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class GeoCodeUnavailable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update locations set latitude = '54.5000992', longitude = '-125.1159973' where LocationId = 0");
            migrationBuilder.Sql("insert into GeocodedLocationCache(Name, Latitude, Longitude, DateGeocoded, IsPermanent) values ('Unavailable, BC, CANADA','54.5000992', '-125.1159973', getdate(), 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
