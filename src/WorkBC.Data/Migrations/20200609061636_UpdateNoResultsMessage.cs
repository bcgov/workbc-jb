using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkBC.Data.Migrations
{
    public partial class UpdateNoResultsMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 
'<h4>There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
  <li>Check your spelling</li>
  <li>Try broader search terms</li>
  <li>Use different synonyms</li>
  <li>Replace abbreviations with the entire word</li>
</ul>' WHERE [Name] = 'jbSearch.errors.noSearchResults'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 
'<h4 class=""mt-4"">There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
<li>Clear all your filters</li>
<li>Add the filters back one at a time</li>
<li>Click the Apply Filter button after every selection</li>
</ul>' WHERE [Name] = 'jbAccount.errors.recommendedJobsNoResultsMultipleCheckboxes'");

            migrationBuilder.Sql(@"UPDATE SystemSettings SET [Value] = 
'<h4 class=""mt-4"">There are no results matching your search criteria</h4>
<p>Search suggestions:</p>
<ul>
<li>Clear all your filters</li>
<li >Select a different filter</li>
</ul>' WHERE [Name] = 'jbAccount.errors.recommendedJobsNoResultsOneCheckbox'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}