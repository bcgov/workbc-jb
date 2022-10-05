using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class HowIsSalaryCalculated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"UPDATE SystemSettings SET [Value] = '<p>When you select a value from the salary drop-down menu, that figure is automatically converted to the equivalent annual salary even when a pay period other than <em>Annually</em> is selected.</p>
<p>The conversion is calculated as follows:</p>
<ul>
    <li>Hourly rate is multiplied by 2,080 (approximate number of work hours in a year).</li>
    <li>Weekly rate is multiplied by 52 (number of weeks in a year).</li>
    <li>Biweekly rate (every two weeks) is multiplied by 26 (half the number of weeks in a year).</li>
    <li>Monthly rate is multiplied by 12 (number of months in a year).</li>
</ul>
<p>Using this conversion, the search returns results matching your salary requirements regardless of whether you select <em>Annually</em>, <em>Hourly</em>, <em>Weekly</em>, <em>Biweekly</em> or <em>Monthly</em> as the pay period. The search will include all jobs posted within the salary range you selected.</p>
<p>For example, if you search for a salary at $15 an hour, this will be converted to $31,200 annually ($15 X 2,080).</p>
<p>Salary searches apply to jobs where employers post salary amounts. Note: Some employers may not specify a salary amount.</p>',
[DefaultValue] = '<p>When you select a value from the salary drop-down menu, that figure is automatically converted to the equivalent annual salary even when a pay period other than <em>Annually</em> is selected.</p>
<p>The conversion is calculated as follows:</p>
<ul>
    <li>Hourly rate is multiplied by 2,080 (approximate number of work hours in a year).</li>
    <li>Weekly rate is multiplied by 52 (number of weeks in a year).</li>
    <li>Biweekly rate (every two weeks) is multiplied by 26 (half the number of weeks in a year).</li>
    <li>Monthly rate is multiplied by 12 (number of months in a year).</li>
</ul>
<p>Using this conversion, the search returns results matching your salary requirements regardless of whether you select <em>Annually</em>, <em>Hourly</em>, <em>Weekly</em>, <em>Biweekly</em> or <em>Monthly</em> as the pay period. The search will include all jobs posted within the salary range you selected.</p>
<p>For example, if you search for a salary at $15 an hour, this will be converted to $31,200 annually ($15 X 2,080).</p>
<p>Salary searches apply to jobs where employers post salary amounts. Note: Some employers may not specify a salary amount.</p>'
WHERE [Name] = 'shared.filters.howIsSalaryCalculatedBody'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}