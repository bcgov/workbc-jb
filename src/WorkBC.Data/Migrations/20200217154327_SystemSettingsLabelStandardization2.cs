using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class SystemSettingsLabelStandardization2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update SystemSettings set name = 'shared.tooltips.youth' where name = 'jbSearch.tooltips.youth'");

            var howIsSalaryCalculated =
                @"<p> When you select a value from the salary drop-down menu, that figure is automatically converted to the equivalent annual salary even when a pay period other than Annually is selected. </p>
<p>The conversion is calculated as follows:</p>
<ul>
    <li>Hourly - hourly rate is multiplied by 2080 (approximate number of work hours in a year)</li>
    <li>Weekly - weekly rate is multiplied by 52 (number of weeks in a year)</li>
    <li>Bi-Weekly - bi-weekly rate is multiplied by 26 (half the number of weeks in a year)</li>
    <li>Monthly - monthly rate is multiplied by 12 (number of months in a year)</li>
</ul>
<p>Using this conversion, the search is able to return results matching your salary requirements regardless of whether you select Annually, Hourly, Weekly, Bi-weekly or Monthly as the pay period. The search will include all jobs posted with a salary equal to or higher than the amount selected.</p>
<p>Example: A user searches for salaries: $15 hourly. This will be converted by multiplying $15 by the conversion factor of 2080 (in this case, approximate number of work hours in a year)</p>
<p>Salary searches apply to jobs where employers publicly posted salary amounts (e.g., annual, monthly, bi-weekly, weekly or hourly). Note: Some employers do not specify a salary amount.</p>";

            migrationBuilder.Sql(
                $"INSERT INTO [SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.howIsSalaryCalculatedBody','{howIsSalaryCalculated}','Explanatory text that appears at the bottom of the salary filter.',5,1,GetDate())");

            migrationBuilder.Sql(
                $"INSERT INTO [SystemSettings] ([Name],[Value],[Description],[FieldType],[ModifiedByAdminUserId],[DateUpdated]) VALUES ('shared.filters.howIsSalaryCalculatedTitle','How is salary calculated?','Title for the eplanatory text at the bottom of the salary filter.',1,1,GetDate())");



        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
