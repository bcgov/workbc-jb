using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class RenameNoSearchResults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"update SystemSettings 
                  set [Name] = 'jbSearch.errors.noSearchResults'  
                  where [Name] = 'jbSearch.search.noSearchResults'"
            );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
