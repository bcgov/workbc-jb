using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class JobVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobVersions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<long>(nullable: false),
                    EDM_Location_LocationId = table.Column<int>(nullable: false),
                    NocCodeId = table.Column<short>(nullable: true),
                    NaicsId = table.Column<short>(nullable: true),
                    PositionsAvailable = table.Column<short>(nullable: false),
                    DatePosted = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    DateVersionStart = table.Column<DateTime>(nullable: false),
                    DateVersionEnd = table.Column<DateTime>(nullable: true),
                    IsCurrentVersion = table.Column<bool>(nullable: false),
                    VersionNumber = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobVersions_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onUpdate: ReferentialAction.Restrict,
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobVersions_LocationLookups_EDM_Location_LocationId",
                        column: x => x.EDM_Location_LocationId,
                        principalTable: "LocationLookups",
                        principalColumn: "LocationId",
                        onUpdate: ReferentialAction.Restrict,
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobVersions_NaicsCodes_NaicsId",
                        column: x => x.NaicsId,
                        principalTable: "NaicsCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobVersions_NocCodes_NocCodeId",
                        column: x => x.NocCodeId,
                        principalTable: "NocCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobVersions_JobId",
                table: "JobVersions",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobVersions_EDM_Location_LocationId",
                table: "JobVersions",
                column: "EDM_Location_LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobVersions_NaicsId",
                table: "JobVersions",
                column: "NaicsId");

            migrationBuilder.CreateIndex(
                name: "IX_JobVersions_NocCodeId",
                table: "JobVersions",
                column: "NocCodeId");

            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (2,'Agriculture production livestock and animal specialties')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (7,'Agricultural Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (8,'Forestry')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (9,'Fishing, hunting, and trapping')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (10,'Metal Mining')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (12,'Coal Mining')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (13,'Oil And Gas Extraction')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (14,'Mining And Quarrying Of Nonmetallic Minerals, Except Fuels')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (15,'Building Construction General Contractors And Operative Builders')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (16,'Heavy Construction Other Than Building Construction Contractors')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (17,'Construction Special Trade Contractors')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (20,'Food And Kindred Products')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (40,'Railroad Transportation')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (41,'Local And Suburban Transit And Interurban Highway Passenger Transportation')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (42,'Motor Freight Transportation And Warehousing')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (43,'United States Postal Service')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (44,'Water Transportation')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (45,'Transportation By Air')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (46,'Pipelines, Except Natural Gas')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (47,'Transportation Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (48,'Communications')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (49,'Electric, Gas, And Sanitary Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (50,'Wholesale Trade-durable Goods')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (51,'Wholesale Trade-non-durable Goods')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (52,'Building Materials, Hardware, Garden Supply, And Mobile Home Dealers')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (53,'General Merchandise Stores')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (54,'Food Stores')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (55,'Automotive Dealers And Gasoline Service Stations')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (56,'Apparel And Accessory Stores')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (57,'Home Furniture, Furnishings, And Equipment Stores')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (58,'Eating And Drinking Places')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (59,'Miscellaneous Retail')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (60,'Depository Institutions')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (61,'Non-depository Credit Institutions')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (62,'Security And Commodity Brokers, Dealers, Exchanges, And Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (63,'Insurance Carriers')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (64,'Insurance Agents, Brokers, And Service')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (65,'Real Estate')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (67,'Holding And Other Investment Offices')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (70,'Hotels, Rooming Houses, Camps, And Other Lodging Places')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (72,'Personal Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (73,'Business Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (75,'Automotive Repair, Services, And Parking')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (76,'Miscellaneous Repair Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (78,'Motion Pictures')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (79,'Amusement And Recreation Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (80,'Health Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (81,'Legal Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (82,'Educational Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (83,'Social Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (84,'Museums, Art Galleries, And Botanical And Zoological Gardens')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (86,'Membership Organizations')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (87,'Engineering, Accounting, Research, Management, And Related Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (88,'Private Households')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (89,'Miscellaneous Services')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (91,'Executive, Legislative, And General Government, Except Finance')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (92,'Justice, Public Order, And Safety')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (93,'Public Finance, Taxation, And Monetary Policy')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (94,'Administration Of Human Resource Programs')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (95,'Administration Of Environmental Quality And Housing Programs')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (96,'Administration Of Economic Programs')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (97,'National Security And International Affairs')");
            migrationBuilder.Sql("insert into naicsCodes(Id, Title) values (99,'Nonclassifiable Establishments')");


            migrationBuilder.Sql("INSERT INTO JobVersions (JobId,EDM_Location_LocationId,NocCodeId,NaicsId,PositionsAvailable,DatePosted,IsActive,DateVersionStart,DateVersionEnd,IsCurrentVersion,VersionNumber) SELECT JobId,EDM_Location_LocationId,NocCodeId,NaicsId,PositionsAvailable,DatePosted,IsActive,DateFirstImported AS DateVersionStart,NULL AS DateVersionEnd,1 AS IsCurrentVersion,1 AS VersionNumber FROM Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobVersions");
        }
    }
}
