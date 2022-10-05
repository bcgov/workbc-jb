using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ProvinceAndRegionTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LocationLookups_EDM_Location_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_LocationLookups_EDM_Location_LocationId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobVersions_LocationLookups_EDM_Location_LocationId",
                table: "JobVersions");

            migrationBuilder.RenameColumn(
                name: "EDM_Location_RegionLocationId",
                table: "LocationLookups",
                newName: "RegionId");

            migrationBuilder.RenameColumn(
                name: "EDM_Location_LocationId",
                table: "JobVersions",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_JobVersions_EDM_Location_LocationId",
                table: "JobVersions",
                newName: "IX_JobVersions_LocationId");

            migrationBuilder.RenameColumn(
                name: "EDM_Location_LocationId",
                table: "Jobs",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_EDM_Location_LocationId",
                table: "Jobs",
                newName: "IX_Jobs_LocationId");

            migrationBuilder.RenameColumn(
                name: "WBC_Province_ProvinceId",
                table: "AspNetUsers",
                newName: "ProvinceId");

            migrationBuilder.RenameColumn(
                name: "EDM_Location_LocationId",
                table: "AspNetUsers",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_EDM_Location_LocationId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_LocationId");

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ProvinceId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    ShortName = table.Column<string>(maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ProvinceId);
                });

            migrationBuilder.Sql("insert into Provinces values (1,'Alberta','AB')");
            migrationBuilder.Sql("insert into Provinces values (2,'British Columbia','BC')");
            migrationBuilder.Sql("insert into Provinces values (3,'Manitoba','MB')");
            migrationBuilder.Sql("insert into Provinces values (4,'New Brunswick','NB')");
            migrationBuilder.Sql("insert into Provinces values (5,'Newfoundland and Labrador','NL')");
            migrationBuilder.Sql("insert into Provinces values (6,'Nova Scotia','NS')");
            migrationBuilder.Sql("insert into Provinces values (7,'Ontario','ON')");
            migrationBuilder.Sql("insert into Provinces values (8,'Prince Edward Island','PE')");
            migrationBuilder.Sql("insert into Provinces values (9,'Quebec','QC')");
            migrationBuilder.Sql("insert into Provinces values (10,'Saskatchewan','SK')");
            migrationBuilder.Sql("insert into Provinces values (11,'Northwest Territories','NT')");
            migrationBuilder.Sql("insert into Provinces values (12,'Nunavut','NU')");
            migrationBuilder.Sql("insert into Provinces values (13,'Yukon','YT')");

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    ListOrder = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.Sql("insert into Regions values (2,'Cariboo',2)");
            migrationBuilder.Sql("insert into Regions values (3,'Kootenay',3)");
            migrationBuilder.Sql("insert into Regions values (4,'Mainland / Southwest',4)");
            migrationBuilder.Sql("insert into Regions values (5,'North Coast & Nechako',5)");
            migrationBuilder.Sql("insert into Regions values (6,'Northeast',6)");
            migrationBuilder.Sql("insert into Regions values (7,'Thompson-Okanagan',7)");
            migrationBuilder.Sql("insert into Regions values (8,'Vancouver Island / Coast',8)");

            migrationBuilder.CreateIndex(
                name: "IX_LocationLookups_RegionId",
                table: "LocationLookups",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProvinceId",
                table: "AspNetUsers",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_LocationLookups_LocationId",
                table: "AspNetUsers",
                column: "LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Provinces_ProvinceId",
                table: "AspNetUsers",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "ProvinceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_LocationLookups_LocationId",
                table: "Jobs",
                column: "LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobVersions_LocationLookups_LocationId",
                table: "JobVersions",
                column: "LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationLookups_Regions_RegionId",
                table: "LocationLookups",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LocationLookups_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Provinces_ProvinceId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_LocationLookups_LocationId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobVersions_LocationLookups_LocationId",
                table: "JobVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationLookups_Regions_RegionId",
                table: "LocationLookups");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_LocationLookups_RegionId",
                table: "LocationLookups");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProvinceId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "RegionId",
                table: "LocationLookups",
                newName: "EDM_Location_RegionLocationId");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "JobVersions",
                newName: "EDM_Location_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_JobVersions_LocationId",
                table: "JobVersions",
                newName: "IX_JobVersions_EDM_Location_LocationId");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Jobs",
                newName: "EDM_Location_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_LocationId",
                table: "Jobs",
                newName: "IX_Jobs_EDM_Location_LocationId");

            migrationBuilder.RenameColumn(
                name: "ProvinceId",
                table: "AspNetUsers",
                newName: "WBC_Province_ProvinceId");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "AspNetUsers",
                newName: "EDM_Location_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_EDM_Location_LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_LocationLookups_EDM_Location_LocationId",
                table: "AspNetUsers",
                column: "EDM_Location_LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_LocationLookups_EDM_Location_LocationId",
                table: "Jobs",
                column: "EDM_Location_LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobVersions_LocationLookups_EDM_Location_LocationId",
                table: "JobVersions",
                column: "EDM_Location_LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
