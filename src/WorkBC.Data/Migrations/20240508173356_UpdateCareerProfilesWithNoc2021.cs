using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateCareerProfilesWithNoc2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Add new columns to dbo.SavedCareerProfiles tables.
            migrationBuilder.AddColumn<string>(
            name: "NocCodeId2021",
            table: "SavedCareerProfiles",
            type: "int",
            maxLength: 5,
            nullable: true);

            //Add Foreign Key FK_SavedCareerProfiles_NocCodes2021_Id
            migrationBuilder.AddForeignKey(
            name: "FK_SavedCareerProfiles_NocCodes2021_Id",
            table: "SavedCareerProfiles",
            column: "NocCodeId2021",
            principalTable: "NocCodes2021",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);


            //Populate all existing JobBoard.SavedCareerProfiles.NocCodeId2021 columns
            //by retrieving the corresponding 2021 NOC code from JobBoard.NocCodes2021
            //via JobBoard.SavedCareerProfiles.EDM_CareerProfile_CareerProfileId which matches the JobBoard.NocCodes2021.Code2016
            migrationBuilder.Sql(
                  @"Update SavedCareerProfiles
                    Set NocCodeId2021 = nc.Id
                    FROM SavedCareerProfiles cp 
                    INNER JOIN NocCodes2021 nc
                    on nc.Code2016 LIKE CONCAT('%', CAST(cp.EDM_CareerProfile_CareerProfileId as nvarchar), '%') 
                    GO");

            //Drop the older redundant column after populating NocCodeId2021 column.
            migrationBuilder.DropColumn(
            name: "EDM_CareerProfile_CareerProfileId",
            table: "SavedCareerProfiles");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Drop the Foreign Key
            migrationBuilder.DropForeignKey(
            name: "FK_SavedCareerProfiles_NocCodes2021_Id",
            table: "SavedCareerProfiles");

            //Add older column to dbo.SavedCareeProfiles tables.
            migrationBuilder.AddColumn<string>(
            name: "EDM_CareerProfile_CareerProfileId",
            table: "SavedCareerProfiles",
            type: "int",
            maxLength: 4,
            nullable: true);

            //Reverting to the previous version of this SavedCareerProfiles table
            //Populate the values into EDM_CareerProfile_CareerProfileId column from Code2016 values of NocCodes2021 table.
            migrationBuilder.Sql(
            @"Update SavedCareerProfiles
              Set EDM_CareerProfile_CareerProfileId = Convert(int, Substring(nc.Code2016, 0, 5))
              FROM SavedCareerProfiles cp 
              INNER JOIN NocCodes2021 nc
              On cp.NocCodeId2021 = nc.Id
              GO");

            //Drop the new column after adding the older column for Noc Codes.
            migrationBuilder.DropColumn(
            name: "NocCodeId2021",
            table: "SavedCareerProfiles");
        }


    }
}