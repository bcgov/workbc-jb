using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class FixErrorPlaceholder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"update SystemSettings 
                  set [value] = Replace([value],'<strong>0</strong>','<strong>{0}</strong>') 
                  where [value] like '%<strong>0</strong>%'"
            );

            migrationBuilder.Sql(
                @"update SystemSettings 
                  set [description] = Replace([description],' / HTML version','') 
                  where [name] like '%errors%' and Description like '%HTML version%'"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}