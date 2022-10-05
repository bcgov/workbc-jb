using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class NaicsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update NaicsCodes SET Title = 'Administrative and support services' WHERE ID=40");
            migrationBuilder.Sql("update NaicsCodes SET Title = 'Waste management and remediation services' WHERE ID=41");
            migrationBuilder.Sql("update NaicsCodes SET Title = 'Employment services' WHERE ID=42");
            migrationBuilder.Sql("update NaicsCodes SET Title = 'Repair and maintenance' WHERE ID=43");
            migrationBuilder.Sql("update NaicsCodes SET Title = 'Personal and laundry services' WHERE ID=44");
            migrationBuilder.Sql("update NaicsCodes SET Title = 'Religious, grant-making, civic, and professional and similar organizations' WHERE ID=45");
            migrationBuilder.Sql("update NaicsCodes SET Title = 'Private households' WHERE ID=46");

            migrationBuilder.Sql("delete from NaicsCodes WHERE ID > 46");
            migrationBuilder.Sql("delete from NaicsCodes WHERE ID <> 1 AND ID < 21");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
