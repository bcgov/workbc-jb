using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AccountStatusEnumChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update AspNetUsers set AccountStatus = 3 where AccountStatus = 100");
            migrationBuilder.Sql("update JobSeekerVersions set AccountStatus = 3 where AccountStatus = 100");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
