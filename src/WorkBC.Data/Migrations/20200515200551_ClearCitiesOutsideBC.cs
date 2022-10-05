using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ClearCitiesOutsideBC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "update aspnetusers set city = '', LocationId = null where (city <> '' or LocationId is not null) and ProvinceId <> 2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
