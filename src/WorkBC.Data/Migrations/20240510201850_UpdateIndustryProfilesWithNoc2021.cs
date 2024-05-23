using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System;
using System.Linq;

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
            var allIndustryNaics = GetEDMIndustryNaics();

            var allIndustryNaicProfiles = new List<EDM_IndustryNaicsProfiles>(allNaics.Count +
                                    allIndustryNaics.Count +
                                    allIndustryProfiles.Count);

            allNaics.ForEach(p => allIndustryNaicProfiles.Add(p));

            foreach (var naic in allNaics)
            {
                if (!String.IsNullOrEmpty(naic.naics_id))
                {
                    //get the IndustryId for corresponidng Naics from 
                    migrationBuilder.Sql("Select * from ");

                    //Populate TitleBC column in dbo.Industries table.
                    //migrationBuilder.UpdateData(
                    //table: "Industries",
                    //keyColumn: "Id",
                    //keyValue: id,
                    //column: "TitleBC",
                    //value: naic.sector);

                    //Populate TitleBC column in dbo.Industries table.
                    migrationBuilder.Sql(
                  @"  Update Industries
                  Set TitleBC = e.Sector
                  From Industries i
                  INNER JOIN IndustryNaics n
                  ON i.Id = n.IndustryId
                  INNER JOIN [WorkBC_Enterprise_DEV].[dbo].[EDM_NAICS] e
                  On e.NAICS_ID = n.NaicsId
                  Go");
                }
            }



            //Populate IndustryId column in dbo.SavedCareerProfile table.
            migrationBuilder.Sql(
            @"  Update SavedIndustryProfiles
                  Set IndustryId = n.IndustryId
                  From [WorkBC_jobboard_dev].[dbo].SavedIndustryProfiles scp
                  INNER JOIN [WorkBC_Enterprise_DEV].[dbo].EDM_IndustryProfile e
                  on scp.EDM_IndustryProfile_IndustryProfileId = e.IndustryProfileID
                  INNER JOIN IndustryNaics n
                  on n.NaicsId = e.NAICS_ID
                  INNER JOIN Industries i
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
            @"Update SavedIndustryProfiles
                  Set EDM_IndustryProfile_IndustryProfileId = e.IndustryProfileID
                  From SavedIndustryProfiles scp
                  INNER JOIN IndustryNaics n
                  on scp.IndustryId = n.IndustryId
                  INNER JOIN [WorkBC_Enterprise_DEV].[dbo].EDM_IndustryProfile e
                  on e.NAICS_ID = n.NaicsId
                  INNER JOIN [Industries] i
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
        public List<EDM_NAICS> GetEDMNaics()
        {
            List<EDM_NAICS> edm_naics = new List<EDM_NAICS>();

            //Read the edm_naics json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "edm_naics.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            edm_naics = JsonSerializer.Deserialize<List<EDM_NAICS>>(jsonStream);


            return edm_naics;

        }

        public List<EDM_INDUSTRYPROFILES> GetEDMIndustryProfiles()
        {
            List<EDM_INDUSTRYPROFILES> edm_ips = new List<EDM_INDUSTRYPROFILES>();

            //Read the edm_industryprofiles json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "edm_industryprofiles.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            edm_ips = JsonSerializer.Deserialize<List<EDM_INDUSTRYPROFILES>>(jsonStream);


            return edm_ips;
        }

        public List<EDM_IndustryNaics> GetEDMIndustryNaics()
        {
            List<EDM_IndustryNaics> edm_indNaics = new List<EDM_IndustryNaics>();

            //Read the edm_industryprofiles json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "edm_industryNaics.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            edm_indNaics = JsonSerializer.Deserialize<List<EDM_IndustryNaics>>(jsonStream);


            return edm_indNaics;
        }
    }
    public class EDM_NAICS

    {
        public string naics_id { get; set; }
        public string sector { get; set; }
        public string sectorType { get; set; }
        public string enabled { get; set; }

    }

    public class EDM_INDUSTRYPROFILES

    {
        public string industryProfileID { get; set; }
        public string naics_id { get; set; }

    }
    public class EDM_IndustryNaics

    {
        public string industryId { get; set; }
        public string naicsId { get; set; }

    }
    public class EDM_IndustryNaicsProfiles

    {
        public string industryId { get; set; }
        public string naicsId { get; set; }
        public string industryProfileID { get; set; }
        public string sector { get; set; }
        public string sectorType { get; set; }
        public string enabled { get; set; }

    }
}