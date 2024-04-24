﻿using Microsoft.EntityFrameworkCore.Migrations;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateNocCodes2021Data : Migration
    {
        public UpdateNocCodes2021Data() : base()
        {

        }
        protected async override void Up(MigrationBuilder migrationBuilder)
        {

            var allNocs = GetNocCodes2021();

            foreach (var noc in allNocs)
            {
                if (!String.IsNullOrEmpty(noc.noc_2021))
                {
                    //The field 'Id' is derived from the field 'Code' and is not an auto-generated Identity field.
                    int.TryParse(noc.noc_2021.ToString(), out int id);

                    string code = noc.noc_2021.ToString();
                    string title = noc.label.ToString();
                    string frenchTitle = noc.label_fr.ToString();

                    //Writing the data to the table dbo.NocCodes2021
                    migrationBuilder.InsertData(
                        table: "NocCodes2021",
                        columns: new[] { "Id", "Code", "Title", "FrenchTitle" },
                        values: new object[] { id, code, title, frenchTitle });

                }

            }

        }

        protected async override void Down(MigrationBuilder migrationBuilder)
        {

        }
        public List<NocCodeSSOT> GetNocCodes2021()
        {

            List<NocCodeSSOT> newSSOTCodes = new List<NocCodeSSOT>();
            string fileName = "C:\\Users\\SunanditaS\\source\\repos\\workbc-jb\\src\\WorkBC.Data\\Data\\ssot_nocs.json";
            string jsonString = File.ReadAllText(fileName);
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            newSSOTCodes = JsonSerializer.Deserialize<List<NocCodeSSOT>>(jsonStream);

            return newSSOTCodes;

        }

    }
    public class NocCodeSSOT
    {
        public string noc_2021 { get; set; }
        public string label { get; set; }
        public string label_fr { get; set; }

    }
}