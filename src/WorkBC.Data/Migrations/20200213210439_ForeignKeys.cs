using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class ForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix Foreign Key errors
            migrationBuilder.Sql(
                "update AspNetUsers SET LocationId = null where LocationId not in (SELECT LocationId from LocationLookups)");

            migrationBuilder.Sql("DELETE FROM Jobs WHERE LocationId = 0");

            migrationBuilder.Sql("DELETE FROM JobViews WHERE JobId NOT IN (SELECT JobId FROM Jobs)");

            // Change 'Tsawassen Beach' to 'Tsawassen' to match jobs coming from the federal job board
            migrationBuilder.Sql(
                "update LocationLookups set City = 'Tsawwassen', Label = 'Tsawwassen' where City = 'Tsawwassen Beach'");

            // Lake Country
            migrationBuilder.Sql(
                "update LocationLookups set City = 'Lake Country', Label = 'Lake Country' where City = 'Lake Country, District of'");

            // City of "Unavailable" (I put it in Metro Vancouver since most jobs are in Metro Vancouver)
            migrationBuilder.Sql(
                "INSERT INTO [LocationLookups] ([LocationId],[DistrictId],[RegionId],[City],[Label],[IsDuplicate],[IsHidden]) VALUES (0,23,4,'Unavailable','Unavailable',0,1)");

            migrationBuilder.DropPrimaryKey("PK_NocCodes", "NocCodes");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "SecurityQuestions",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUserId",
                table: "SavedIndustryProfiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUserId",
                table: "SavedCareerProfiles",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "NocCodes",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "NocCodes",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CountryTwoLetterCode",
                table: "Countries",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModifiedByAdminUserId",
                table: "AdminUsers",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_ModifiedByAdminUserId",
                table: "SystemSettings",
                column: "ModifiedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedIndustryProfiles_AspNetUserId",
                table: "SavedIndustryProfiles",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCareerProfiles_AspNetUserId",
                table: "SavedCareerProfiles",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_LocationId",
                table: "Jobs",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CountryId",
                table: "AspNetUsers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_ModifiedByAdminUserId",
                table: "AdminUsers",
                column: "ModifiedByAdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminUsers_AdminUsers_ModifiedByAdminUserId",
                table: "AdminUsers",
                column: "ModifiedByAdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Countries_CountryId",
                table: "AspNetUsers",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_LocationLookups_LocationId",
                table: "AspNetUsers",
                column: "LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_LocationLookups_LocationId",
                table: "Jobs",
                column: "LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobViews_Jobs_JobId",
                table: "JobViews",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedCareerProfiles_AspNetUsers_AspNetUserId",
                table: "SavedCareerProfiles",
                column: "AspNetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedIndustryProfiles_AspNetUsers_AspNetUserId",
                table: "SavedIndustryProfiles",
                column: "AspNetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemSettings_AdminUsers_ModifiedByAdminUserId",
                table: "SystemSettings",
                column: "ModifiedByAdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddPrimaryKey("PK_NocCodes", "NocCodes", "Code");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminUsers_AdminUsers_ModifiedByAdminUserId",
                table: "AdminUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Countries_CountryId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LocationLookups_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_LocationLookups_LocationId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobViews_Jobs_JobId",
                table: "JobViews");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedCareerProfiles_AspNetUsers_AspNetUserId",
                table: "SavedCareerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedIndustryProfiles_AspNetUsers_AspNetUserId",
                table: "SavedIndustryProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemSettings_AdminUsers_ModifiedByAdminUserId",
                table: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_SystemSettings_ModifiedByAdminUserId",
                table: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_SavedIndustryProfiles_AspNetUserId",
                table: "SavedIndustryProfiles");

            migrationBuilder.DropIndex(
                name: "IX_SavedCareerProfiles_AspNetUserId",
                table: "SavedCareerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_LocationId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CountryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AdminUsers_ModifiedByAdminUserId",
                table: "AdminUsers");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "SecurityQuestions",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUserId",
                table: "SavedIndustryProfiles",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AspNetUserId",
                table: "SavedCareerProfiles",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "NocCodes",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "NocCodes",
                type: "varchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CountryTwoLetterCode",
                table: "Countries",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModifiedByAdminUserId",
                table: "AdminUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
