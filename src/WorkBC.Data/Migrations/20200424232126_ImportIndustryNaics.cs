using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ImportIndustryNaics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_JobSeekerAdminLog_AdminUsers_ModifiedByAdminUserId",
                "JobSeekerChangeLog");

            migrationBuilder.AlterColumn<int>(
                "ModifiedByAdminUserId",
                "JobSeekerChangeLog",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                "FK_JobSeekerChangeLog_AdminUsers_ModifiedByAdminUserId",
                "JobSeekerChangeLog",
                "ModifiedByAdminUserId",
                "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (1,2)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (1,8)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (21,13)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (22,12)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (23,4)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (24,11)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (25,18)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (26,19)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (27,17)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (28,10)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (29,6)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (30,6)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (31,15)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (32,3)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (34,5)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (35,9)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (36,10)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (37,1)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (39,16)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (40,3)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (41,3)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (42,3)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (43,14)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (44,14)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (45,14)");
            migrationBuilder.Sql("Insert Into IndustryNaics (IndustryId, NaicsId) Values (46,14)");

            migrationBuilder.Sql(@"ALTER TABLE [dbo].[JobSeekerChangeLog] DROP CONSTRAINT [PK_JobSeekerAdminLog] WITH ( ONLINE = OFF )");
            migrationBuilder.Sql(@"
ALTER TABLE [dbo].[JobSeekerChangeLog] ADD  CONSTRAINT [PK_JobSeekerChangeLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");


            migrationBuilder.Sql(@"ALTER TABLE [dbo].[JobSeekerChangeLog] DROP CONSTRAINT [FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId]");

            migrationBuilder.Sql(@"
ALTER TABLE [dbo].[JobSeekerChangeLog]  WITH CHECK ADD  CONSTRAINT [FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId] FOREIGN KEY([AspNetUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])");

            migrationBuilder.Sql(@"ALTER TABLE [dbo].[JobSeekerChangeLog] CHECK CONSTRAINT [FK_JobSeekerAdminLog_AspNetUsers_AspNetUserId]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_JobSeekerChangeLog_AdminUsers_ModifiedByAdminUserId",
                "JobSeekerChangeLog");

            migrationBuilder.AlterColumn<int>(
                "ModifiedByAdminUserId",
                "JobSeekerChangeLog",
                "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                "FK_JobSeekerChangeLog_AdminUsers_ModifiedByAdminUserId",
                "JobSeekerChangeLog",
                "ModifiedByAdminUserId",
                "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}