using Microsoft.EntityFrameworkCore.Migrations;
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
    public partial class InsertNocCodes2021 : Migration
    {
        private readonly HttpClient _httpClient;
        private readonly string _ssotBaseUrl;
        private readonly string _ssotUrl;


        public InsertNocCodes2021() : base()
        {
            _httpClient = new HttpClient();
            _ssotBaseUrl = Environment.GetEnvironmentVariable("SSOT_URL");
            if (_ssotBaseUrl == null)
            {
                throw new Exception("SSOT_URL not set");
            }
            _ssotUrl = _ssotBaseUrl + "/nocs_nocs";           

        }
        protected async override void Up(MigrationBuilder migrationBuilder)
        {
            var allNocs = await GetNocCodes2021(_ssotUrl);            

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
            //Truncate table- delete all records
            migrationBuilder.Sql(@"TRUNCATE TABLE [dbo].[NocCodes2021]");
        }
        public async Task<List<NocCodeSSOT>> GetNocCodes2021(string nocCodes2021SSOTUrl)
        {

            List<NocCodeSSOT> newSSOTCodes = new List<NocCodeSSOT>();

                using (var _httpClient = new HttpClient())
                {
                    var response = await _httpClient.GetAsync(nocCodes2021SSOTUrl);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("NO SSOT response: " + response.Content);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();

                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
                        //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                        MemoryStream jsonStream = new MemoryStream(byteArray);
                       
                        newSSOTCodes = await JsonSerializer.DeserializeAsync<List<NocCodeSSOT>>(jsonStream);

                    }

                }

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
