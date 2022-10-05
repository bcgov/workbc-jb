using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class IndexJobSeekers3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_City_LastName_FirstName",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL")
                .Annotation("SqlServer:Include", new[] { "LastName", "FirstName", "AccountStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_City_ProvinceId_CountryId_LastName_FirstName",
                table: "AspNetUsers",
                columns: new[] { "City", "ProvinceId", "CountryId", "LastName", "FirstName" })
                .Annotation("SqlServer:Include", new[] { "Email", "AccountStatus" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_City_ProvinceId_CountryId_LastName_FirstName",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_City_LastName_FirstName",
                table: "AspNetUsers",
                columns: new[] { "City", "LastName", "FirstName" })
                .Annotation("SqlServer:Include", new[] { "Email", "AccountStatus" });
        }
    }
}
