using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class NewToolTips : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
                       ([Name]
                       ,[Value]
                       ,[Description]
                       ,[FieldType]
                       ,[ModifiedByAdminUserId]
                       ,[DateUpdated]
                       ,[DefaultValue])
                 VALUES
                       ('shared.tooltips.onSite'
                       ,'There is no option to work remotely. All work must be completed at the physical location.'
                       ,'Tooltip for workplace type ""On-site only""'
                       ,1
                       ,1
                       ,GETDATE()
                       ,'There is no option to work remotely. All work must be completed at the physical location.')");


            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
                       ([Name]
                       ,[Value]
                       ,[Description]
                       ,[FieldType]
                       ,[ModifiedByAdminUserId]
                       ,[DateUpdated]
                       ,[DefaultValue])
                 VALUES
                       ('shared.tooltips.hybrid'
                       ,'Work can be completed remotely or at the physical work location.'
                       ,'Tooltip for workplace type ""On -site or remote work""'
                       ,1
                       ,1
                       ,GETDATE()
                       ,'Work can be completed remotely or at the physical work location.')");


            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
                       ([Name]
                       ,[Value]
                       ,[Description]
                       ,[FieldType]
                       ,[ModifiedByAdminUserId]
                       ,[DateUpdated]
                       ,[DefaultValue])
                 VALUES
                       ('shared.tooltips.travelling'
                       ,'Frequent or constant travel is required from the employee.'
                       ,'Tooltip for workplace type ""Work location varies""'
                       ,1
                       ,1
                       ,GETDATE()
                       ,'Frequent or constant travel is required from the employee.')");

            migrationBuilder.Sql(@"INSERT INTO [dbo].[SystemSettings]
                       ([Name]
                       ,[Value]
                       ,[Description]
                       ,[FieldType]
                       ,[ModifiedByAdminUserId]
                       ,[DateUpdated]
                       ,[DefaultValue])
                 VALUES
                       ('shared.tooltips.virtual'
                       ,'There is no physical work location. All work must be completed remotely. These job posting are advertised across Canada.'
                       ,'Tooltip for workplace type ""Virtual job""'
                       ,1
                       ,1
                       ,GETDATE()
                       ,'There is no physical work location. All work must be completed remotely. These job posting are advertised across Canada.')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
