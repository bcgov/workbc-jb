using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class Geocoding__Add_LocationLookup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "LocationLookups",
                table => new
                {
                    LocationId = table.Column<int>(),
                    DistrictId = table.Column<int>(),
                    RegionId = table.Column<int>(),
                    FederalCityId = table.Column<int>(nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    Label = table.Column<string>(maxLength: 50, nullable: true),
                    IsDuplicate = table.Column<bool>(),
                    IsHidden = table.Column<bool>(),
                    Latitude = table.Column<string>(maxLength: 25, nullable: true),
                    Longitude = table.Column<string>(maxLength: 25, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_LocationLookups", x => x.LocationId); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "LocationLookups");
        }
    }
}