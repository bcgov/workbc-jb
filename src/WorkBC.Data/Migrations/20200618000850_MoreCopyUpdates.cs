using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class MoreCopyUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"update systemsettings set [value] = '<p>When you select a value from the salary drop-down menu, that figure is automatically converted to the equivalent annual salary even when a pay period other than Annually is selected. </p>
<p>The conversion is calculated as follows:</p>
<ul>
    <li>Hourly - hourly rate is multiplied by 2080 (approximate number of work hours in a year)</li>
    <li>Weekly - weekly rate is multiplied by 52 (number of weeks in a year)</li>
    <li>Biweekly - biweekly rate is multiplied by 26 (half the number of weeks in a year)</li>
    <li>Monthly - monthly rate is multiplied by 12 (number of months in a year)</li>
</ul>
<p>Using this conversion, the search is able to return results matching your salary requirements regardless of whether you select Annually, Hourly, Weekly, Biweekly or Monthly as the pay period. The search will include all jobs posted with a salary equal to or higher than the amount selected.</p>
<p>Example: A user searches for salaries: $15 hourly. This will be converted by multiplying $15 by the conversion factor of 2080 (in this case, approximate number of work hours in a year)</p>
<p>Salary searches apply to jobs where employers publicly posted salary amounts (e.g., annual, monthly, biweekly, weekly or hourly). Note: Some employers do not specify a salary amount.</p>' WHERE [Name] = 'shared.filters.howIsSalaryCalculatedBody'");

            migrationBuilder.Sql(
                @"update systemsettings set [value] = 'More Filters' WHERE [name] = 'shared.filters.moreFiltersTitle'");

            migrationBuilder.Sql(
                @"update systemsettings set [value] = 'Youth are people between 15 and 30 years of age.' where [name] = 'shared.tooltips.youth'");

            migrationBuilder.Sql(
                @"update systemsettings set [value] = 'The <a href=""http://noc.esdc.gc.ca/English/noc/welcome.aspx?ver=16"" target=""_blank"" style=""text-decoration: underline;"">National Occupational Classification</a> system classifies all occupations in Canada.' where [name] = 'shared.tooltips.nocCode'");

            migrationBuilder.Sql(
                @"update systemsettings set [value] = 'WorkBC jobs are verified by the National Job Bank. External jobs are B.C. job postings from other job boards (For example Monster and Indeed).' where [name] = 'shared.tooltips.jobSource'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}