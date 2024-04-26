using Microsoft.EntityFrameworkCore.Migrations;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;

#nullable disable

namespace WorkBC.Data.Migrations
{
    public partial class UpdateNocCodes2021Data : Migration
    {
        public UpdateNocCodes2021Data() : base()
        {

        }
        protected async override void Up(MigrationBuilder migrationBuilder)
        {
            //Obsolete migration: Relevant code moved to 20240424212524_UpdateNocCodes2021Data.cs

        }

        protected async override void Down(MigrationBuilder migrationBuilder)
        {
            //Obsolete migration: Relevant code moved to 20240411194800_AddNocCodes2021.cs
        }
    }
}