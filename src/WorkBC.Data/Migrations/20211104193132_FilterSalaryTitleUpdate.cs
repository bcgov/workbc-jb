using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FilterSalaryTitleUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    UPDATE [dbo].[SystemSettings]
                       SET 
                           [Value] = 'Salary & Benefits'
                          ,[DefaultValue] = 'Salary & Benefits'
	                      ,[DateUpdated] = GETDATE()
                     WHERE [Name] = 'shared.filters.salaryTitle'
                "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
