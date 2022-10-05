using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobSeekerChangeEventFieldLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "Field",
                "JobSeekerChangeLog",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.Sql(
                @"UPDATE SystemSettings SET [Value] = 'The postal code <strong>{0}</strong> could not be found. Please enter a postal code in British Columbia.',
[DefaultValue] = 'The postal code <strong>{0}</strong> could not be found. Please enter a postal code in British Columbia.'
WHERE [Name] = 'jbSearch.errors.outOfProvincePostal'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "Field",
                "JobSeekerChangeLog",
                "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}