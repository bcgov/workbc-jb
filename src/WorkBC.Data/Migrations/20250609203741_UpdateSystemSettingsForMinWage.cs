using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateSystemSettingsForMinWage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Add new Minimum Wage record to dbo.SystemSettings tables.            
            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Name", "Value", "Description", "FieldType", "ModifiedByAdminUserId", "DateUpdated", "DefaultValue" },
                values: new object[] { "shared.settings.minimumWage", "17.85", "All job posts should adhere to the new minimum wage increase of $17.85/hr as of June 30, 2025. ", 1, 1, DateTime.Now, "17.85"});

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
