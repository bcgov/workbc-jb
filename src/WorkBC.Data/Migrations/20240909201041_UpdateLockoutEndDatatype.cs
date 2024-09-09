using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateLockoutEndDatatype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "datetime",
                nullable: true,
                oldType: "datetimeoffset"); 
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
