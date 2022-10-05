using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class IndexJobSeekers4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_City_ProvinceId_CountryId_LastName_FirstName",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_City_ProvinceId_CountryId_LastName_FirstName",
                table: "AspNetUsers",
                columns: new[] { "City", "ProvinceId", "CountryId", "LastName", "FirstName" })
                .Annotation("SqlServer:Include", new[] { "Email", "AccountStatus" });
        }
    }
}
