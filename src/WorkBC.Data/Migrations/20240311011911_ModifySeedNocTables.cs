using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;
using WorkBC.Data.Model.Ssot;

#nullable disable

namespace WorkBC.Data.Migrations
{
    
    public partial class ModifySeedNocTables : Migration
    {
        private readonly HttpClient _httpClient;
        private readonly string _ssotBaseUrl;

        public ModifySeedNocTables() : base()
        {
            _httpClient = new HttpClient();
            _ssotBaseUrl = Environment.GetEnvironmentVariable("ASPNETCORE_SSOT_BASE_URL");
            if (_ssotBaseUrl == null)
            {
                throw new Exception("ASPNETCORE_SSOT_BASE_URL not set");
            }
        }
        
        protected async override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "NocCodes",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Noc2016",
                table: "NocCodes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "NocCategories",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);
            
            // CAUTION - delete all data from the tables!!
            migrationBuilder.Sql(@"DELETE FROM NocCategories");
            migrationBuilder.Sql(@"DELETE FROM NocCodes"); 

            // Seed Nocs
            var allNocs = await GetAllNocs();
            foreach (var noc in allNocs)
            {
                migrationBuilder.Sql(@"INSERT INTO [dbo].[NocCodes]
                       ([Code],[Title],[FrenchTitle])
                 VALUES
                       (noc.noc_2021, noc.label, noc.label_fr)");
            }
            
            // Seed NocCategories
            var allNocsCategories = await GetAllNocsCategories();
            foreach (var nocsCategory in allNocsCategories)
            {
                migrationBuilder.Sql(@"INSERT INTO [dbo].[NocCategories]
                       ([CategoryCode],[Level],[Title])
                 VALUES
                       (nocsCategory.noc_2021, nocsCategory.level, nocsCategory.label)");
            }
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Noc2016",
                table: "NocCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "NocCodes",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "NocCategories",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4);
        }
        
        private async Task<List<Noc>> GetAllNocs()
        {
            try
            {
                using (_httpClient)
                {
                    var response = await _httpClient.GetAsync(_ssotBaseUrl + "/nocs_nocs");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<Noc>>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new List<Noc>();
        }
    
        private async Task<List<NocCategory>> GetAllNocsCategories()
        {
            try
            {
                using (_httpClient)
                {
                    var response = await _httpClient.GetAsync(_ssotBaseUrl + "/nocs_categories");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<NocCategory>>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new List<NocCategory>();
        }
    }
}
