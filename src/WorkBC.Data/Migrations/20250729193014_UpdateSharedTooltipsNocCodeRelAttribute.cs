using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateSharedTooltipsNocCodeRelAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string name = "shared.tooltips.nocCode";
            string value = "The <a href=\"http://noc.esdc.gc.ca/\" target=\"_blank\" rel=\"noopener noreferrer\" style=\"text-decoration: underline;\">National Occupational Classification</a> system classifies all occupations in Canada.";
            migrationBuilder.UpdateData(
            table: "SystemSettings",
            keyColumn: "Name",
            keyValue: name,
            column: "Value",
            value: value);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
