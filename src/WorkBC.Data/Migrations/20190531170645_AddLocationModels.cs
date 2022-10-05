using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddLocationModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_City",
                table: "City");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "City");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "City");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "City",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "City",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "City",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_City",
                table: "City",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "GeocodedLocationCache",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    DateGeocoded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeocodedLocationCache", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_City_RegionId",
                table: "City",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_City_Region_RegionId",
                table: "City",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_City_Region_RegionId",
                table: "City");

            migrationBuilder.DropTable(
                name: "GeocodedLocationCache");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropPrimaryKey(
                name: "PK_City",
                table: "City");

            migrationBuilder.DropIndex(
                name: "IX_City_RegionId",
                table: "City");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "City");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "City");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "City",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "City",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "City",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_City",
                table: "City",
                column: "Name");
        }
    }
}
