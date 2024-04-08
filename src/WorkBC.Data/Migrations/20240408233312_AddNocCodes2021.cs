using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class AddNocCodes2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NocCodes2021",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FrenchTitle = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NocCodes2021", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NocCodes2021");
        }
    }
}
