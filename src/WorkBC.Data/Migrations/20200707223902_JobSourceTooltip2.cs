using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobSourceTooltip2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM [SystemSettings] WHERE [Name] = 'shared.tooltips.jobSource'

GO

INSERT [dbo].[SystemSettings] ([Name], [Value], [Description], [FieldType], [ModifiedByAdminUserId], [DateUpdated], [DefaultValue]) VALUES (N'shared.tooltips.jobSource', N'WorkBC jobs are verified by the National Job Bank. External jobs are B.C. job postings from other job boards (for example Monster and Indeed).', N'Tooltip for Job Source in the more filters filter', 1, 1, GETDATE(), N'WorkBC jobs are verified by the National Job Bank. External jobs are B.C. job postings from other job boards (for example Monster and Indeed).')
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
