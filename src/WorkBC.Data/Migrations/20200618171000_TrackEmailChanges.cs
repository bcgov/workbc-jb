using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class TrackEmailChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "JobSeekerVersions",
                maxLength: 256,
                nullable: true);

            migrationBuilder.Sql(
                @"update jv set jv.Email = u.Email from AspNetUsers u inner join JobSeekerVersions jv on jv.AspNetUserId = u.Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "JobSeekerVersions");
        }
    }
}
