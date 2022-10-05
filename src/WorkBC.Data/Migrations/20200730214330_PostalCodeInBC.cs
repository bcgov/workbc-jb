using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class PostalCodeInBC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[DefaultValue]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbSearch.errors.outOfProvincePostal'
                           ,'Please enter a postal code in British Columbia.'
                           ,'Please enter a postal code in British Columbia.'
                           ,'Error message for postal code outside B.C.'
                           ,5
                           ,1
                           ,GETDATE())
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
