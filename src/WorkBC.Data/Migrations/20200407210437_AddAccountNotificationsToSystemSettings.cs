using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class AddAccountNotificationsToSystemSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM SystemSettings where [name] = 'jbAccount.dashboard.newAccountMessageTitle' " +
                "OR [name] = 'jbAccount.dashboard.newAccountMessageBody' " +
                "OR [name] = 'jbAccount.dashboard.notification1Title' " +
                "OR [name] = 'jbAccount.dashboard.notification1Body' " +
                "OR [name] = 'jbAccount.dashboard.notification1Enabled' " +
                "OR [name] = 'jbAccount.dashboard.notification2Title' " +
                "OR [name] = 'jbAccount.dashboard.notification2Body' " +
                "OR [name] = 'jbAccount.dashboard.notification2Enabled'"
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
                           ('jbAccount.dashboard.newAccountMessageTitle'
                           ,'Welcome to WorkBC job board.'
                           ,'jbAccount dashboard newAccountMessageTitle / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            var value = @"
                            <p>Here is what''s new with the website:<p>
                            <p>1. We have made improvements to your Job Alerts. Your Job Alerts will continue to recommend job postings to you based on your selected preferences, but you can now access them from your Dashboard menu and receive them via email.</p>
                            <p>2. If you are a returning user, your Saved Searches have been merged into the enhanced Job Alerts section of your account.</p>
                        ";
            migrationBuilder.Sql(@$"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.dashboard.newAccountMessageBody'
                           ,'{value}'
                           ,'jbAccount dashboard newAccountMessageBody / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.dashboard.notification1Title'
                           ,'<em>notification 1 title</em>'
                           ,'jbAccount dashboard notification1Title / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.dashboard.notification1Body'
                           ,'notification 1 body ...'
                           ,'jbAccount dashboard notification1Body / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.dashboard.notification1Enabled'
                           ,'1'
                           ,'Toggle the notification 1 on or off'
                           ,4
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.dashboard.notification2Title'
                           ,'notification 2 title'
                           ,'jbAccount dashboard notification2Title / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.dashboard.notification2Body'
                           ,'notification 2 body ...'
                           ,'jbAccount dashboard notification2Body / HTML version'
                           ,5
                           ,1
                           ,GETDATE())
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [dbo].[SystemSettings]
                           ([Name]
                           ,[Value]
                           ,[Description]
                           ,[FieldType]
                           ,[ModifiedByAdminUserId]
                           ,[DateUpdated])
                     VALUES
                           ('jbAccount.dashboard.notification2Enabled'
                           ,'1'
                           ,'Toggle the notification 2 on or off'
                           ,4
                           ,1
                           ,GETDATE())
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
