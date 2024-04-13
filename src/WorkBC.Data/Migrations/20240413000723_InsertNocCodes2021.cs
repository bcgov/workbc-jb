using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using WorkBC.Data.Model.JobBoard;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WorkBC.Data.Model.Enterprise;
using System.IO;
using Microsoft.VisualBasic;
using System.Text;
using System.Linq;

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
            Console.WriteLine("Hello");
            // Append to NocCodes2021  table from SSOT API
            var allNocs = await GetNocCodes2021(_ssotUrl);

            Console.WriteLine("allNocs: " + allNocs.Count.ToString());
            Console.WriteLine("Inside Up method");

            //migrationBuilder.Sql(@"INSERT INTO [dbo].[NocCodes2021]
            //           ([Id],[Code],[Title], [FrenchTitle])
            //     VALUES
            //           (1, '1', 'test1, test2', 'testFr1')");


            foreach (var nocs in allNocs)
            {

                if (String.IsNullOrEmpty(nocs.noc_2021))
                {
                    Console.WriteLine("null noc");
                }
                else
                {
                    try
                    {
                        int.TryParse(nocs.noc_2021, out int id);

                        string code = nocs.noc_2021.ToString();
                        string title = nocs.label_en.ToString();
                        string frenchTitle = nocs.label_fr.ToString();

                        //Console.WriteLine("id: " + id);
                        //Console.WriteLine("code: " + code);
                        //Console.WriteLine("title: " + title);
                        //Console.WriteLine("frenchTitle: " + frenchTitle);

                        migrationBuilder.InsertData(
                            table: "NocCodes2021",
                            columns: new[] { "Id", "Code", "Title", "FrenchTitle" },
                            values: new object[] { id, code, title, frenchTitle });

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Inside catch in up: " + ex.Message.ToString());
                    }
                }


            }

        }

        protected async override void Down(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine("Inside Insert Down method");
            //Truncate table- delete all records
            migrationBuilder.Sql(@"TRUNCATE TABLE [dbo].[NocCodes2021]");
        }
        public async Task<List<NocCodeSSOT>> GetNocCodes2021(string nocCodes2021SSOTUrl)
        {

            List<NocCodeSSOT> newSSOTCodes = new List<NocCodeSSOT>();
            try
            {
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
                        Console.WriteLine("Inside GetNoc method");

                        // convert string to stream
                        byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
                        //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                        MemoryStream jsonStream = new MemoryStream(byteArray);
                        Console.WriteLine("After stream method");
                        newSSOTCodes = await JsonSerializer.DeserializeAsync<List<NocCodeSSOT>>(jsonStream);

                        Console.WriteLine("After deserialize method");
                    }

                }


            }
            catch (Exception ex)
            {
                throw new Exception("Inside catch" + ex.Message.ToString());
            }
            return newSSOTCodes;


        }

    }
}
