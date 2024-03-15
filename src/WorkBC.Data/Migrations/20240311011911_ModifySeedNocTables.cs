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
            _ssotBaseUrl = Environment.GetEnvironmentVariable("SSOT_URL");
            if (_ssotBaseUrl == null)
            {
                throw new Exception("SSOT_URL not set");
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

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "NocCategories",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);
            
            migrationBuilder.AddColumn<string>(
                name: "NocCode",
                table: "Jobs",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);
            
            migrationBuilder.AddColumn<string>(
                name: "NocCode",
                table: "JobVersions",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);
            
            // Append to NocCategories table from SSOT API
            var allNocsCategories = await GetAllNocsCategories();
            foreach (var nocsCategory in allNocsCategories)
            {
                migrationBuilder.Sql(@"INSERT INTO [dbo].[NocCategories]
                       ([CategoryCode],[Level],[Title])
                 VALUES
                       (nocsCategory.noc_2021, nocsCategory.level, nocsCategory.label)");
            }
            
            // Append Nocs table from SSOT API
            var allNocs = await GetAllNocs();
            foreach (var noc in allNocs)
            {
                migrationBuilder.Sql(@"INSERT INTO [dbo].[NocCodes]
                       ([Code],[Title],[FrenchTitle])
                 VALUES
                       (noc.noc_2021, noc.label, noc.label_fr)");
            }
            
            // Populate new NocCode column in the Jobs table
            migrationBuilder.Sql(@"INSERT INTO [dbo].[Jobs]
                       ([NocCode])
                 SELECT nc.Code FROM [WorkBC_JobBoard].[dbo].[Jobs] j INNER JOIN NocCodes nc ON j.NocCodeId = nc.Id;");
            
            
            // Populate new NocCode column in the JobVersions table
            migrationBuilder.Sql(@"INSERT INTO [dbo].[JobVersions]
                       ([NocCode])
                 SELECT nc.Code FROM [WorkBC_JobBoard].[dbo].[JobVersions] j INNER JOIN NocCodes nc ON j.NocCodeId = nc.Id;");
            
            // Drop old foreign keys
            migrationBuilder.DropForeignKey("FK_Jobs_NocCodes_NocCodeId", "Jobs");
            migrationBuilder.DropForeignKey("FK_JobVersions_NocCodes_NocCodeId", "JobVersions");

            // Create new foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_NocCodes_NocCode",
                table: "Jobs",
                column: "NocCode",
                principalTable: "NocCodes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
            
            migrationBuilder.AddForeignKey(
                name: "FK_JobVersions_NocCodes_NocCode",
                table: "JobVersions",
                column: "NocCode",
                principalTable: "NocCodes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
            
            // The NocCodes.Id column is no longer needed
            migrationBuilder.DropColumn(name: "Id", table: "NocCodes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // this migration is too complicated to reverse, restore from backup instead
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
