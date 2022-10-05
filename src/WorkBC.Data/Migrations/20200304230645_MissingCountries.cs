using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class MissingCountries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries]([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (98,'Iran, Islamic Republic Of','II',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries]([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (118,'Libya','LY',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries]([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (123,'Macedonia','MK',9999)");
            migrationBuilder.Sql("INSERT INTO [dbo].[Countries]([Id],[Name],[CountryTwoLetterCode],[SortOrder]) VALUES (160,'Palestine','PS',9999)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
