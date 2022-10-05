using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class IndexJobSeekers2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_City",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DateRegistered",
                table: "AspNetUsers",
                column: "DateRegistered")
                .Annotation("SqlServer:Include", new[] { "LastName", "FirstName", "Email", "AccountStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LastModified",
                table: "AspNetUsers",
                column: "LastModified")
                .Annotation("SqlServer:Include", new[] { "LastName", "FirstName", "Email", "AccountStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FirstName_LastName",
                table: "AspNetUsers",
                columns: new[] { "FirstName", "LastName" })
                .Annotation("SqlServer:Include", new[] { "Email", "AccountStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LastName_FirstName",
                table: "AspNetUsers",
                columns: new[] { "LastName", "FirstName" })
                .Annotation("SqlServer:Include", new[] { "Email", "AccountStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AccountStatus_LastName_FirstName",
                table: "AspNetUsers",
                columns: new[] { "AccountStatus", "LastName", "FirstName" })
                .Annotation("SqlServer:Include", new[] { "Email" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_City_LastName_FirstName",
                table: "AspNetUsers",
                columns: new[] { "City", "LastName", "FirstName" })
                .Annotation("SqlServer:Include", new[] { "Email", "AccountStatus" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DateRegistered",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LastModified",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FirstName_LastName",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LastName_FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AccountStatus_LastName_FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_City_LastName_FirstName",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_City",
                table: "AspNetUsers",
                column: "City")
                .Annotation("SqlServer:Include", new[] { "Id", "Email", "AccountStatus", "CountryId", "ProvinceId", "FirstName", "LastName", "LastModified", "DateRegistered", "LockedByAdminUserId", "DateLocked" });
        }
    }
}
