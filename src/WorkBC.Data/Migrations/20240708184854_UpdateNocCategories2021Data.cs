using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateNocCategories2021Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var allNocsCategories = GetNocCategories2021();

            foreach (var noc in allNocsCategories)
            {
                if (!String.IsNullOrEmpty(noc.noc_2021))
                {                    
                    string categoryCode = noc.noc_2021.ToString();
                    byte level = noc.level;
                    string title = noc.label.ToString();

                    //Writing the data to the table dbo.NocCodes2021
                    migrationBuilder.InsertData(
                        table: "NocCategories2021",
                        columns: new[] { "CategoryCode", "Level", "Title" },
                        values: new object[] {categoryCode, level, title });
                }

            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {        
            //Delete all records from the table.
            migrationBuilder.Sql(@"Delete from [dbo].[NocCategories2021]");
        }
        public List<NocCodeCategoriesSSOT> GetNocCategories2021()
        {

            List<NocCodeCategoriesSSOT> newSSOTCategories = new List<NocCodeCategoriesSSOT>();
            //Read the ssot_categories json file from the path:~\workbc-jb\src\WorkBC.Web
            string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "ssot_categories.json"));
            // convert string to stream
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(byteArray);

            newSSOTCategories = JsonSerializer.Deserialize<List<NocCodeCategoriesSSOT>>(jsonStream);

            return newSSOTCategories;

        }
    }
    public class NocCodeCategoriesSSOT
    {
        public string noc_2021 { get; set; }
        public string label { get; set; }
        public byte level { get; set; }

    }
}
