using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateCareerprofilesFor2006NocCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            UpdateNocCodes2016Data obj = new UpdateNocCodes2016Data();
            var allNocs = obj.GetNocCodes2016();

            foreach (var noc in allNocs)
            {
                if (!String.IsNullOrEmpty(noc.noc_2021))
                {
                    string code2016 = noc.noc_2016.ToString();

                    //The field 'Id' is derived from the field 'Code' and is not an auto-generated Identity field.
                    int.TryParse(noc.noc_2021.ToString(), out int id);

                    //Updating Code2016 column in the table dbo.NocCodes2021.
                    migrationBuilder.UpdateData(
                    table: "NocCodes2021",
                    keyColumn: "Id",
                    keyValue: id,
                    column: "Code2016",
                    value: code2016);
                }
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
