using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class IndexJobSeekerCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_City",
                table: "AspNetUsers",
                column: "City")
                .Annotation("SqlServer:Include", new[] { "Id", "Email", "AccountStatus", "CountryId", "ProvinceId", "FirstName", "LastName", "LastModified", "DateRegistered", "LockedByAdminUserId", "DateLocked" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_City",
                table: "AspNetUsers");
        }
    }
}
