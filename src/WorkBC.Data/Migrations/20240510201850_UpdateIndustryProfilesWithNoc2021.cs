using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateIndustryProfilesWithNoc2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Add new column to dbo.SavedIndustryProfiles tables.
            migrationBuilder.AddColumn<string>(
            name: "IndustryId",
            table: "SavedIndustryProfiles",
            type: "smallint",
            nullable: true);

            //Add new column to dbo.Industries table.
            migrationBuilder.AddColumn<string>(
            name: "TitleBC",
            table: "Industries",
            type: "nvarchar(150)",
            maxLength: 150,
            nullable: true);

            //Add Foreign Key FK_SavedIndustryProfiles_Industries_Id
            migrationBuilder.AddForeignKey(
            name: "FK_SavedIndustryProfiles_Industries_Id",
            table: "SavedIndustryProfiles",
            column: "IndustryId",
            principalTable: "Industries",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

            //Populate TitleBC column in dbo.Industries table.
            migrationBuilder.Sql(
            @"  Update [WorkBC_jobboard_dev].[dbo].Industries
                  Set TitleBC = e.Sector
                  From [WorkBC_jobboard_dev].[dbo].[Industries] i
                  INNER JOIN [WorkBC_jobboard_dev].[dbo].[IndustryNaics] n
                  ON i.Id = n.IndustryId
                  INNER JOIN [WorkBC_Enterprise_DEV].[dbo].[EDM_NAICS] e
                  On e.NAICS_ID = n.NaicsId
                  Go");

            //Populate IndustryId column in dbo.SavedCareerProfile table.
            migrationBuilder.Sql(
            @"  Update [WorkBC_jobboard_dev].[dbo].SavedIndustryProfiles
                  Set IndustryId = n.IndustryId
                  From [WorkBC_jobboard_dev].[dbo].SavedIndustryProfiles scp
                  INNER JOIN [WorkBC_Enterprise_DEV].[dbo].EDM_IndustryProfile e
                  on scp.EDM_IndustryProfile_IndustryProfileId = e.IndustryProfileID
                  INNER JOIN [WorkBC_jobboard_dev].[dbo].[IndustryNaics] n
                  on n.NaicsId = e.NAICS_ID
                  INNER JOIN [WorkBC_jobboard_dev].[dbo].[Industries] i
                  on i.Id = n.IndustryId
                  Go");

            //Drop the older redundant column after populating IndustryId column.
            migrationBuilder.DropColumn(
            name: "EDM_IndustryProfile_IndustryProfileId",
            table: "SavedIndustryProfiles");

            //Drop the table "IndustryNaics"
            migrationBuilder.DropTable(
            name: "IndustryNaics");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Restore the dbo.IndustryNaics table with its contents.
            migrationBuilder.CreateTable(
            name: "IndustryNaics",
            columns: table => new
            {
                IndustryId = table.Column<short>(nullable: false),
                NaicsId = table.Column<short>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IndustryNaics", x => new { x.IndustryId, x.NaicsId });
                table.ForeignKey(
                    name: "FK_IndustryNaics_Industries_IndustryId",
                    column: x => x.IndustryId,
                    principalTable: "Industries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
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


            //Restore older column to dbo.SavedIndustryProfiles tables.
            migrationBuilder.AddColumn<string>(
            name: "EDM_IndustryProfile_IndustryProfileId",
            table: "SavedIndustryProfiles",
            type: "int",
            nullable: true);

            //Restore the values of EDM_IndustryProfile_IndustryProfileId column in dbo.SavedIndustryProfiles table.
            migrationBuilder.Sql(
            @"Update [WorkBC_jobboard_dev].[dbo].SavedIndustryProfiles
                  Set EDM_IndustryProfile_IndustryProfileId = e.IndustryProfileID
                  From [WorkBC_jobboard_dev].[dbo].SavedIndustryProfiles scp
                  INNER JOIN [WorkBC_jobboard_dev].[dbo].[IndustryNaics] n
                  on scp.IndustryId = n.IndustryId
                  INNER JOIN [WorkBC_Enterprise_DEV].[dbo].EDM_IndustryProfile e
                  on e.NAICS_ID = n.NaicsId
                  INNER JOIN [WorkBC_jobboard_dev].[dbo].[Industries] i
                  on i.Id = n.IndustryId
                  Go");

            //Drop the Foreign Key
            migrationBuilder.DropForeignKey(
            name: "FK_SavedIndustryProfiles_Industries_Id",
            table: "SavedIndustryProfiles");

            //Drop the new column after adding the older column for Industries and SavedIndustryProfiles.
            migrationBuilder.DropColumn(
            name: "TitleBC",
            table: "Industries");

            migrationBuilder.DropColumn(
            name: "IndustryId",
            table: "SavedIndustryProfiles");
        }
    }
}