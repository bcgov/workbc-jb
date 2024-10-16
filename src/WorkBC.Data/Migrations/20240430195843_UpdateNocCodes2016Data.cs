using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;
using System;
using WorkBC.Data.Model.Enterprise;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateNocCodes2016Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Add new columns to dbo.NocCodes2021 tables.
            migrationBuilder.AddColumn<string>(
            name: "Code2016",
            table: "NocCodes2021",
            type: "varchar(30)",
            maxLength: 30,
            nullable: true);

            var allNocs = GetNocCodes2016();

            foreach (var noc in allNocs)
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

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "Code2016",
            table: "NocCodes2021");
        }

        public List<NocCodeSSOTwith2016> GetNocCodes2016()
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

    public class NocCodeSSOTwith2016

    {
        public string noc_2021 { get; set; }
        public string label { get; set; }
        public string label_fr { get; set; }
        public string noc_2016 { get; set; }

    }
}