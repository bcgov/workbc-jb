using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class Noc_Code_Updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Commissioned officers of the Canadian Armed Forces' WHERE Code = '0433'");
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Payroll administrators' WHERE Code = '1432'");
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Non-commissioned ranks of the Canadian Armed Forces' WHERE Code = '4313'");
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Heating, refrigeration and air conditioning mechanics' WHERE Code = '7313'");
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Supervisors, food and beverage processing' WHERE Code = '9213'");
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Central control and process operators, petroleum, gas and chemical processing' WHERE Code = '9232'");
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Process control and machine operators, food and beverage processing' WHERE Code = '9461'");
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Testers and graders, food and beverage processing' WHERE Code = '9465'");
            migrationBuilder.Sql("UPDATE NocCodes SET Title = 'Labourers in food and beverage processing' WHERE Code = '9617'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
