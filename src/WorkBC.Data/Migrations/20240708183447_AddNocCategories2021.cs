using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class AddNocCategories2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NocCategories2021",
                columns: table => new
                {
                    CategoryCode = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NocCategories2021", x => x.CategoryCode);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NocCategories2021");
        }
    }
}
