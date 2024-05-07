using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System;
using WorkBC.Data.Model.Enterprise;
using WorkBC.Data.Model.JobBoard;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateCareerProfilesWithNoc2021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Add new columns to dbo.SavedCareeProfiles tables.
            migrationBuilder.AddColumn<string>(
            name: "NocCodeId2021",
            table: "SavedCareerProfiles",
            type: "nvarchar(5)",
            maxLength: 5,
            nullable: true);

            //Populate all existing JobBoard.SavedCareerProfiles.NocCodeId2021 columns
            //by retrieving the corresponding 2021 NOC code from JobBoard.NocCodes2021
            //via JobBoard.SavedCareerProfiles.EDM_CareerProfile_CareerProfileId which matches the JobBoard.NocCodes2021.Code2016
            migrationBuilder.Sql(
                  @"  Update [WorkBC_jobboard_dev].[dbo].[SavedCareerProfiles]
                      Set NocCodeId2021 = (
                      Select Code from [WorkBC_jobboard_dev].[dbo].NocCodes2021 nc 
                      where ((Select CAST(EDM_CareerProfile_CareerProfileId As varchar) from [WorkBC_jobboard_dev].[dbo].[SavedCareerProfiles]) in (nc.Code2016))
                      )
                  GO");

            //Drop the older redundant column after populating NocCodeId2021 column.
            migrationBuilder.DropColumn(
            name: "EDM_CareerProfile_CareerProfileId",
            table: "SavedCareerProfiles");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
            @"Update [dbo].[SavedCareerProfiles]
              Set EDM_CareerProfile_CareerProfileId = (
              Select Code2016 from [WorkBC_jobboard_dev].[dbo].NocCodes2021 nc 
              where ((Select NocCodeId2021 from [WorkBC_jobboard_dev].[dbo].[SavedCareerProfiles]) in (nc.Code))
              )
              GO");

            //Drop the new column after adding the older column for Noc Codes.
            migrationBuilder.DropColumn(
            name: "NocCodeId2021",
            table: "SavedCareerProfiles");
        }

    }
}
