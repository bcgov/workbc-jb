using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System;
using System.Linq;
using System.Collections;
using WorkBC.Data.Model.Enterprise;
using WorkBC.Data.Model.JobBoard;

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
            var allNocs = GetEDMNocs();
            var allCareerProfiles = GetEDMCareerProfiles();
            var allSSOTData = GetNocCodesSSOT();

            var allNocCareerProfiles = allNocs //EDM_NOCS list
                .Join(allCareerProfiles, //Join with EDM_CareerProfiles list
                      cp => cp.NOC_ID,
                      no => no.NOC_ID,
                      (cp, no) => new { allNoc = cp, nocCareer = no })
                ;

            foreach (var x in allNocCareerProfiles)
            {

                //Get the CareerProfileId, Noc ID and Noc Codes from the super list.
                int careerProfileId = x.nocCareer.CareerProfileID;
                string nocCode = x.allNoc.NOCCode;
                int nocId = x.allNoc.NOC_ID;

                var noc2021= allSSOTData.Where(s => s.noc_2016.Contains(nocCode)).Select(s => s.noc_2021).FirstOrDefault();
                
                //Populate NocCodeId2021 column in dbo.SavedCareerProfile table.
                migrationBuilder.UpdateData(
                table: "SavedCareerProfiles",
                keyColumn: "EDM_CareerProfile_CareerProfileId",
                keyValue: careerProfileId,
                column: "NocCodeId2021",
                value: noc2021);               

            }

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

            ////Reverting to the previous version of this SavedCareerProfiles table
            ////Populate the values into EDM_CareerProfile_CareerProfileId column from Code2016 values of NocCodes2021 table.
            var allNocs = GetEDMNocs();
            var allCareerProfiles = GetEDMCareerProfiles();
            var allSSOTData = GetNocCodesSSOT();

            var allNocCareerProfiles = allNocs //EDM_NOCS list
                .Join(allCareerProfiles, //Join with EDM_CareerProfiles list
                      cp => cp.NOC_ID,
                      no => no.NOC_ID,
                      (cp, no) => new { allNoc = cp, nocCareer = no })
                ;

            foreach (var x in allNocCareerProfiles)
            {

                //Get the CareerProfileId, Noc ID and Noc Codes from the super list.
                int careerProfileId = x.nocCareer.CareerProfileID;
                string nocCode = x.allNoc.NOCCode;
                int nocId = x.allNoc.NOC_ID;

                var noc2021 = allSSOTData.Where(s => s.noc_2016.Contains(nocCode)).Select(s => s.noc_2021).FirstOrDefault();
                //Populate EDM_CareerProfile_CareerProfileId column in dbo.SavedCareerProfile table.
                //migrationBuilder.UpdateData(
                //table: "SavedCareerProfiles",
                //keyColumn: "NocCodeId2021",
                //keyValue: noc2021,
                //column: "EDM_CareerProfile_CareerProfileId",
                //value: careerProfileId);
                //migrationBuilder.Sql("Update SavedCareerProfiles SET EDM_CareerProfile_CareerProfileId =" +
                //    careerProfileId + "where NocCodeId2021 =" + noc2021);

            }

            //Drop the new column after adding the older column for Noc Codes.
            migrationBuilder.DropColumn(
            name: "NocCodeId2021",
            table: "SavedCareerProfiles");
        }

        public List<EDM_NOCS> GetEDMNocs()
        {
            //Read the edm_nocs json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "edm_nocs.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            List<EDM_NOCS> edm_nocs = JsonSerializer.Deserialize<List<EDM_NOCS>>(jsonStream);

            return edm_nocs;

        }

        public List<EDM_CAREERPROFILES> GetEDMCareerProfiles()
        {
            //Read the edm_industryprofiles json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "edm_careerprofiles.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            List<EDM_CAREERPROFILES> edm_cps = JsonSerializer.Deserialize<List<EDM_CAREERPROFILES>>(jsonStream);


            return edm_cps;
        }
        public List<NocCodeSSOTwith2016> GetNocCodesSSOT()
        {
            List<NocCodeSSOTwith2016> newSSOTCodes = new List<NocCodeSSOTwith2016>();

            //Read the ssot_nocs json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "ssot_nocs.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            newSSOTCodes = JsonSerializer.Deserialize<List<NocCodeSSOTwith2016>>(jsonStream);


            return newSSOTCodes;

        }

    }
    public class EDM_NOCS

    {
        public int NOC_ID { get; set; }
        public string NOCCode { get; set; }

    }
    public class EDM_CAREERPROFILES

    {
        public int CareerProfileID { get; set; }
        public int NOC_ID { get; set; }

    }
}