using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateQueenCharlotteCityName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Locations" +
                                 "SET City = 'Daajing Giids ( Queen Charlotte City )', " +
                                 "Label = 'Daajing Giids ( Queen Charlotte City )'" +
                                 "WHERE LocationId = '1594'");

            migrationBuilder.Sql("UPDATE ImportedJobsFederal" +
                                 "SET ReIndexNeeded = 1" +
                                 "WHERE JobId IN (SELECT JobId FROM Jobs where City = 'Queen Charlotte City')");
            
            migrationBuilder.Sql("UPDATE ImportedJobsWanted" +
                                 "SET ReIndexNeeded = 1" +
                                 "WHERE JobId IN (SELECT JobId FROM Jobs where City = 'Queen Charlotte City')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Locations" +
                                 "SET City = 'Queen Charlotte City', " +
                                 "Label = 'Queen Charlotte City'" +
                                 "WHERE LocationId = '1594'");
            
            migrationBuilder.Sql("UPDATE ImportedJobsFederal" +
                                 "SET ReIndexNeeded = 0" +
                                 "WHERE JobId IN (SELECT JobId FROM Jobs where City = 'Queen Charlotte City')");
            
            migrationBuilder.Sql("UPDATE ImportedJobsWanted" +
                                 "SET ReIndexNeeded = 0" +
                                 "WHERE JobId IN (SELECT JobId FROM Jobs where City = 'Queen Charlotte City')");
            
        }
    }
}
