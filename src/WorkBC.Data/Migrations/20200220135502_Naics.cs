using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class Naics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "NaicsId",
                table: "Jobs",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.Sql(
                "update jobs set naicsId = null where naicsId not in (1,39,25,21,22,23,24,26,27,28,29,30,31,32,33,34,35,36,37,38)");

            migrationBuilder.CreateTable(
                name: "NaicsCodes",
                columns: table => new
                {
                    Id = table.Column<short>(nullable: false),
                    Title = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaicsCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_NaicsId",
                table: "Jobs",
                column: "NaicsId");

            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (1,'Agriculture, forestry, fishing and hunting')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (39,'Public administration')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (25,'Wholesale trade')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (21,'Mining and oil and gas extraction')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (22,'Utilities')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (23,'Construction')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (24,'Manufacturing')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (26,'Retail trade')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (27,'Transportation and warehousing')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (28,'Information and cultural industries')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (29,'Finance and insurance')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (30,'Real estate and rental and leasing')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (31,'Professional, scientific and technical services')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (32,'Management of Companies and Enterprises')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (33,'Administrative and support, waste management and remediation services')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (34,'Educational services')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (35,'Health care and social assistance')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (36,'Arts, entertainment and recreation')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (37,'Accommodation and food services')");
            migrationBuilder.Sql("insert into NaicsCodes (Id, Title) values (38,'Other services (except public administration)')");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_NaicsCodes_NaicsId",
                table: "Jobs",
                column: "NaicsId",
                principalTable: "NaicsCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

       
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_NaicsCodes_NaicsId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "NaicsCodes");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_NaicsId",
                table: "Jobs");

            migrationBuilder.AlterColumn<short>(
                name: "NaicsId",
                table: "Jobs",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldNullable: true);
        }
    }
}
