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
        private readonly string _ssotUrl;

        public InsertNocCodes2021() : base()
        {
            _httpClient = new HttpClient();
            _ssotUrl = Environment.GetEnvironmentVariable("SSOT_URL");
            if (_ssotUrl == null)
            {
                throw new Exception("SSOT_URL not set");
            }
        }
        protected async override void Up(MigrationBuilder migrationBuilder)
        {
            var allNocs = await GetNocCodes2021(_ssotUrl);

            foreach (var nocs in allNocs)
            {

                if (!String.IsNullOrEmpty(nocs.noc_2021))
                {
                        int.TryParse(nocs.noc_2021, out int id);

                        string code = nocs.noc_2021.ToString();
                        string title = nocs.label_en.ToString();
                        string frenchTitle = nocs.label_fr.ToString();

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
        public string label_en { get; set; }
        public string label_fr { get; set; }

    }
}
