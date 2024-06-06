using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
using WorkBC.Data.Model.JobBoard;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateIndustryProfilesWithNewLabels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Update the TitleBC column in dbo.Industries table for Forestry, Logging and Support Activities.
            migrationBuilder.UpdateData(
            table: "Industries",
            keyColumn: "Title",
            keyValue: "Agriculture, forestry, fishing and hunting",
            column: "TitleBC",
            value: "Forestry, Logging and Support Activities");

            //Update the TitleBC column in dbo.Industries table for Forestry, Logging and Support Activities.
            migrationBuilder.UpdateData(
            table: "Industries",
            keyColumn: "Title",
            keyValue: "Administrative and support, waste management and remediation services",
            column: "TitleBC",
            value: "Agriculture and Fishing");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Update the TitleBC column in dbo.Industries table for Forestry, Logging and Support Activities.
            migrationBuilder.UpdateData(
            table: "Industries",
            keyColumn: "Title",
            keyValue: "Administrative and support, waste management and remediation services",
            column: "TitleBC",
            value: null);
        }
    }
}
