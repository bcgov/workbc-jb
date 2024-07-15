using Microsoft.EntityFrameworkCore.Migrations;
using WorkBC.Data.Model.Enterprise;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateSharedTooltipsNocCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string name = "shared.tooltips.nocCode";
            string description = "Tooltip for 2021 NOC code in the more filters filter";
            migrationBuilder.UpdateData(
            table: "SystemSettings",
            keyColumn: "Name",
            keyValue: name,
            column: "Description",
            value: description) ;
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
