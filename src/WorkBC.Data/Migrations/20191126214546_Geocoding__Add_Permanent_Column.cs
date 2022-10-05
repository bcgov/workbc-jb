using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Migrations;
using WorkBC.Data.Model.JobBoard;

namespace WorkBC.Data.Migrations
{
    public partial class Geocoding__Add_Permanent_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "Name",
                "GeocodedLocationCache",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Longitude",
                "GeocodedLocationCache",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Latitude",
                "GeocodedLocationCache",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                "IsPermanent",
                "GeocodedLocationCache",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "IsPermanent",
                "GeocodedLocationCache");

            migrationBuilder.AlterColumn<string>(
                "Name",
                "GeocodedLocationCache",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Longitude",
                "GeocodedLocationCache",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Latitude",
                "GeocodedLocationCache",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 25,
                oldNullable: true);
        }
    }
}