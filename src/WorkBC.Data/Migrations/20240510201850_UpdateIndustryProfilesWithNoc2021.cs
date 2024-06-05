using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System;
using System.Linq;
using WorkBC.Data.Model.Enterprise;
using WorkBC.Data.Model.JobBoard;

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


            var allNaics = GetEDMNaics();
            var allIndustryProfiles = GetEDMIndustryProfiles();
            var allIndustryNaics = GetJBIndustryNaics();

            var allIndustryNaicProfiles = allNaics //EDM_NAICS list
                .Join(allIndustryNaics, //Join with IndustryNaics list
                      an => an.NAICS_ID,
                      ain => ain.NaicsId,
                      (an, ain) => new { allNaic = an, allIndnaic = ain })
                .Join(allIndustryProfiles, //Join with EDM_IndustryProfiles list
                      ni => ni.allNaic.NAICS_ID,
                      ip => ip.NAICS_ID,
                      (ni, ip) => new { naicsInd = ni, indProfile = ip })
                ;

            foreach (var x in allIndustryNaicProfiles)
            {

                //get the IndustryId for corresponidng Naics from the super list
                short industryId = x.naicsInd.allIndnaic.IndustryId;
                string titlebc = x.naicsInd.allNaic.Sector;
                int industryProfileId = x.indProfile.IndustryProfileID;

                //Populate TitleBC column in dbo.Industries table.
                migrationBuilder.UpdateData(
                table: "Industries",
                keyColumn: "Id",
                keyValue: industryId,
                column: "TitleBC",
                value: titlebc);

                //Populate IndustryId column in dbo.SavedCareerProfile table.
                migrationBuilder.UpdateData(
                table: "SavedIndustryProfiles",
                keyColumn: "EDM_IndustryProfile_IndustryProfileId",
                keyValue: industryProfileId,
                column: "IndustryId",
                value: industryId);
            }
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

            //Restore older column to dbo.SavedIndustryProfiles tables.
            migrationBuilder.AddColumn<string>(
            name: "EDM_IndustryProfile_IndustryProfileId",
            table: "SavedIndustryProfiles",
            type: "int",
            nullable: true);

            var allNaics = GetEDMNaics();
            var allIndustryProfiles = GetEDMIndustryProfiles();
            var allIndustryNaics = GetJBIndustryNaics();



            var allIndustryNaicProfiles = allNaics //EDM_NAICS list
                .Join(allIndustryNaics, //Join with IndustryNaics list
                      an => an.NAICS_ID,
                      ain => ain.NaicsId,
                      (an, ain) => new { allNaic = an, allIndnaic = ain })
                .Join(allIndustryProfiles, //Join with EDM_IndustryProfiles list
                      ni => ni.allNaic.NAICS_ID,
                      ip => ip.NAICS_ID,
                      (ni, ip) => new { naicsInd = ni, indProfile = ip })
                ;
            //Restore the contents of dbo.IndustryNaics table.
            foreach (var x in allIndustryNaics)
            {
                migrationBuilder.InsertData("IndustryNaics",
                new[] { "IndustryId", "NaicsId" },
                new object[] { x.IndustryId, x.NaicsId });
            }


            foreach (var x in allIndustryNaicProfiles)
            {
                //get the IndustryId for corresponding Naics from the super list
                short industryId = x.naicsInd.allIndnaic.IndustryId;
                string titlebc = x.naicsInd.allNaic.Sector;
                int industryProfileId = x.indProfile.IndustryProfileID;

                //Restore the values of EDM_IndustryProfile_IndustryProfileId column in dbo.SavedIndustryProfiles table.
                migrationBuilder.Sql("Update SavedIndustryProfiles SET EDM_IndustryProfile_IndustryProfileId =" +
                    industryProfileId + "where IndustryId =" + industryId);
            }

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
        public List<EDM_NAICS> GetEDMNaics()
        {
            //Read the edm_naics json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "edm_naics.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            List<EDM_NAICS> edm_naics = JsonSerializer.Deserialize<List<EDM_NAICS>>(jsonStream);

            return edm_naics;

        }

        public List<EDM_INDUSTRYPROFILES> GetEDMIndustryProfiles()
        {
            //Read the edm_industryprofiles json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "edm_industryprofiles.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            List<EDM_INDUSTRYPROFILES> edm_ips = JsonSerializer.Deserialize<List<EDM_INDUSTRYPROFILES>>(jsonStream);


            return edm_ips;
        }

        public List<JB_IndustryNaics> GetJBIndustryNaics()
        {
            //Read the jb_industryNaics json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "jb_industryNaics.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            List<JB_IndustryNaics> jb_indNaics = JsonSerializer.Deserialize<List<JB_IndustryNaics>>(jsonStream);


            return jb_indNaics;
        }
}
    public class EDM_NAICS

    {
        public int NAICS_ID { get; set; }
        public string Sector { get; set; }
        public bool Enabled { get; set; }

    }

    public class EDM_INDUSTRYPROFILES

    {
        public int IndustryProfileID { get; set; }
        public int NAICS_ID { get; set; }

    }
    public class JB_IndustryNaics

    {
        public short IndustryId { get; set; }
        public short NaicsId { get; set; }

    }
}