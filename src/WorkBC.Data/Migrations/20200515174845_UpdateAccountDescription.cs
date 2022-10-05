using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateAccountDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"update SystemSettings 
                  set [Value] = Replace([Value], 'yout', 'your')  
                  where [Name] = 'jbAccount.dashboard.accountDescription'
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
