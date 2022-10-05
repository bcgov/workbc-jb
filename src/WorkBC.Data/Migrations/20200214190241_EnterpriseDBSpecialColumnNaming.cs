using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class EnterpriseDBSpecialColumnNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LocationLookups_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_LocationLookups_LocationId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "DateFirstSeen",
                table: "JobIds",
                newName: "DateFirstImported");

            migrationBuilder.RenameColumn(
                name: "RemovedFromIndex",
                table: "ExpiredJobs",
                "RemovedFromElasticsearch");

            migrationBuilder.RenameColumn(
                name: "IndustryProfileId",
                table: "SavedIndustryProfiles",
                newName: "EDM_IndustryProfile_IndustryProfileId");

            migrationBuilder.RenameColumn(
                name: "CareerProfileId",
                table: "SavedCareerProfiles",
                newName: "EDM_CareerProfile_CareerProfileId");

            migrationBuilder.RenameColumn(
                name: "RegionId",
                table: "LocationLookups",
                newName: "EDM_Location_RegionLocationId");

            migrationBuilder.RenameColumn(
                name: "DistrictId",
                table: "LocationLookups",
                newName: "EDM_Location_DistrictLocationId");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "Jobs",
                newName: "EDM_Location_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_LocationId",
                table: "Jobs",
                newName: "IX_Jobs_EDM_Location_LocationId");

            migrationBuilder.RenameColumn(
                name: "ProvinceId",
                table: "AspNetUsers",
                newName: "WBC_Province_ProvinceId");

            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "AspNetUsers",
                newName: "EDM_Location_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_EDM_Location_LocationId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLocked",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LockedByAdminUserId",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLocked",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LockedByAdminUserId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_LockedByAdminUserId",
                table: "Jobs",
                column: "LockedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LockedByAdminUserId",
                table: "AspNetUsers",
                column: "LockedByAdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_LocationLookups_EDM_Location_LocationId",
                table: "AspNetUsers",
                column: "EDM_Location_LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AdminUsers_LockedByAdminUserId",
                table: "AspNetUsers",
                column: "LockedByAdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_LocationLookups_EDM_Location_LocationId",
                table: "Jobs",
                column: "EDM_Location_LocationId",
                principalTable: "LocationLookups",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AdminUsers_LockedByAdminUserId",
                table: "Jobs",
                column: "LockedByAdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_LocationLookups_EDM_Location_LocationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AdminUsers_LockedByAdminUserId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_LocationLookups_EDM_Location_LocationId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AdminUsers_LockedByAdminUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_LockedByAdminUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LockedByAdminUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateLocked",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LockedByAdminUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "DateFirstImported",
                table: "JobIds");

            migrationBuilder.DropColumn(
                name: "RemovedFromElasticsearch",
                table: "ExpiredJobs");

            migrationBuilder.DropColumn(
                name: "DateLocked",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LockedByAdminUserId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "EDM_IndustryProfile_IndustryProfileId",
                table: "SavedIndustryProfiles",
                newName: "IndustryProfileId");

            migrationBuilder.RenameColumn(
                name: "EDM_CareerProfile_CareerProfileId",
                table: "SavedCareerProfiles",
                newName: "CareerProfileId");

            migrationBuilder.RenameColumn(
                name: "EDM_Location_RegionLocationId",
                table: "LocationLookups",
                newName: "RegionId");

            migrationBuilder.RenameColumn(
                name: "EDM_Location_DistrictLocationId",
                table: "LocationLookups",
                newName: "DistrictId");

            migrationBuilder.RenameColumn(
                name: "EDM_Location_LocationId",
                table: "Jobs",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_EDM_Location_LocationId",
                table: "Jobs",
                newName: "IX_Jobs_LocationId");

            migrationBuilder.RenameColumn(
                name: "WBC_Province_ProvinceId",
                table: "AspNetUsers",
                newName: "ProvinceId");

            migrationBuilder.RenameColumn(
                name: "EDM_Location_LocationId",
                table: "AspNetUsers",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_EDM_Location_LocationId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_LocationId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFirstSeen",
                table: "JobIds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "RemovedFromIndex",
                table: "ExpiredJobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
        }
    }
}
