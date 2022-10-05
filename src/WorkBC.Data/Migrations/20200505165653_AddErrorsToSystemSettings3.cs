using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddErrorsToSystemSettings3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM SystemSettings where " +
                "[name] = 'jbAccount.errors.emptyPassword'"
                );

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.errors.emptyPassword'
                           ,'Please enter password'
                           ,'Error message for empty password'
                           ,1
                           ,1
                           ,GETDATE())
                ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
