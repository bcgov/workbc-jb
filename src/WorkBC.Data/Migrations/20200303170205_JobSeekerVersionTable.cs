using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobSeekerVersionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobSeekerVersions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUserId = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: true),
                    ProvinceId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    DateRegistered = table.Column<DateTime>(nullable: false),
                    AccountStatus = table.Column<short>(type: "smallint", nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    IsApprentice = table.Column<bool>(nullable: false),
                    IsIndigenousPerson = table.Column<bool>(nullable: false),
                    IsMatureWorker = table.Column<bool>(nullable: false),
                    IsNewImmigrant = table.Column<bool>(nullable: false),
                    IsPersonWithDisability = table.Column<bool>(nullable: false),
                    IsReservist = table.Column<bool>(nullable: false),
                    IsStudent = table.Column<bool>(nullable: false),
                    IsVeteran = table.Column<bool>(nullable: false),
                    IsVisibleMinority = table.Column<bool>(nullable: false),
                    IsYouth = table.Column<bool>(nullable: false),
                    DateVersionStart = table.Column<DateTime>(nullable: false),
                    DateVersionEnd = table.Column<DateTime>(nullable: true),
                    IsCurrentVersion = table.Column<bool>(nullable: false),
                    VersionNumber = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSeekerVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSeekerVersions_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekerVersions_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSeekerVersions_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSeekerVersions_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerVersions_AspNetUserId",
                table: "JobSeekerVersions",
                column: "AspNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerVersions_CountryId",
                table: "JobSeekerVersions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerVersions_LocationId",
                table: "JobSeekerVersions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerVersions_ProvinceId",
                table: "JobSeekerVersions",
                column: "ProvinceId");

            migrationBuilder.Sql(
                @"INSERT INTO JobSeekerVersions 
                  SELECT u.Id AS [AspNetUserId],[CountryId],[ProvinceId],[LocationId],[DateCreated] AS [DateRegistered]
						  ,[AccountStatus],[EmailConfirmed]
                          ,isnull(IsApprentice,0) AS IsApprentice
                          ,isnull(IsIndigenousPerson,0) AS IsIndigenousPerson
                          ,isnull(IsMatureWorker,0) AS IsMatureWorker
                          ,isnull(IsNewImmigrant,0) AS IsNewImmigrant
                          ,isnull(IsPersonWithDisability,0) AS IsPersonWithDisability
                          ,isnull(IsReservist,0) AS IsReservist
                          ,isnull(IsStudent,0) AS IsStudent
                          ,isnull(IsVeteran,0) AS IsVeteran
                          ,isnull(IsVisibleMinority,0) AS IsVisibleMinority
                          ,isnull(IsYouth,0) AS IsYouth
                          ,[DateCreated] AS [DateVersionStart],NULL AS [DateVersionEnd]
	                      ,1 AS [IsCurrentVersion],1 AS [VersionNumber] 
                  FROM AspNetUsers u
                  LEFT OUTER JOIN JobSeekerFlags f ON f.AspNetUserId = u.id ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSeekerVersions");
        }
    }
}
