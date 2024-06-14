using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Linq;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateCareerprofilesFor2006NocCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateNocCodes2016Data obj = new UpdateNocCodes2016Data();
            var allSSOTNocs = obj.GetNocCodes2016();

            foreach (var noc in allSSOTNocs)
            {
                if (!String.IsNullOrEmpty(noc.noc_2021))
                {
                    string code2016 = noc.noc_2016.ToString();

                    //The field 'Id' is derived from the field 'Code' and is not an auto-generated Identity field.
                    int.TryParse(noc.noc_2021.ToString(), out int id);

                    //Updating Code2016 column in the table dbo.NocCodes2021.
                    migrationBuilder.UpdateData(
                    table: "NocCodes2021",
                    keyColumn: "Id",
                    keyValue: id,
                    column: "Code2016",
                    value: code2016);
                }
            }

            //Populate all existing JobBoard.SavedCareerProfiles.NocCodeId2021 columns
            //by retrieving the corresponding 2021 NOC code from JobBoard.NocCodes2021
            //via JobBoard.SavedCareerProfiles.EDM_CareerProfile_CareerProfileId which matches the corresponding entries in EDM_Nocs json and EDM_careerProfiles json file.
            UpdateCareerProfilesWithNoc2021 cpObj = new UpdateCareerProfilesWithNoc2021();
            var allNocs = cpObj.GetEDMNocs();
            var allCareerProfiles = cpObj.GetEDMCareerProfiles();
            var allSSOTData = cpObj.GetNocCodesSSOT();

            var allNocCareerProfiles = allNocs //EDM_NOCS list
                .Join(allCareerProfiles, //Join with EDM_CareerProfiles list
                      cp => cp.NOC_ID,
                      no => no.NOC_ID,
                      (cp, no) => new { allNoc = cp, nocCareer = no });

            foreach (var x in allNocCareerProfiles)
            {

                //Get the CareerProfileId, Noc ID and Noc Codes from the super list.
                int careerProfileId = x.nocCareer.CareerProfileID;
                string nocCode = x.allNoc.NOCCode;
                int nocId = x.allNoc.NOC_ID;

                var noc2021 = allSSOTData.Where(s => s.noc_2016.Contains(nocCode)).Select(s => s.noc_2021).FirstOrDefault();

                //Populate NocCodeId2021 column in dbo.SavedCareerProfile table.
                migrationBuilder.UpdateData(
                table: "SavedCareerProfiles",
                keyColumn: "EDM_CareerProfile_CareerProfileId",
                keyValue: careerProfileId,
                column: "NocCodeId2021",
                value: noc2021);

            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}