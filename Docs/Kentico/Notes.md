1.  Create the page template

* Log in to Kentico with a Global Admin account
* Add a new top-level Page Template category called "WorkBC JobBoard"
* Add a Page Template to the new category called "WorkBC Angular Page"
* On the "General" tab of the new Page Template, select "Only the nearest master page"
* Add the following lines to the "Layout" tab

<div class="layout-container"> 
  <cms:CMSWebPartZone ZoneID="TopZone" runat="server" />
  <cms:CMSWebPartZone ZoneID="MiddleZone" runat="server" />
  <cms:CMSWebPartZone ZoneID="BottomZone" runat="server" />
</div>

* Save the template (use defaults for any settings not mentioned above)


2.  Create 2 pages using the new page template

First page:  /Account.aspx

* Add a new "Account" page
* Page Type = JTST Generic Page
* Template = WorkBC Angular Page (Clone as Ad-Hoc)
* Page nesting = Only the nearest master page
* Output caching = NO

Second page:  /Jobs & Careers/Find Jobs/Jobs.aspx

* Delete the existing "Job Seach Results" page (MAKE A NOTE OF ALL ALIASES BEFORE DELETING)
* Add a new "Job Board" page
* Page Type = JTST Generic Page
* Template = WorkBC Angular Page (Clone as Ad-Hoc)
* Page nesting = Only the nearest master page
* Set the page alias on the URLs tab to "Jobs"
* Output caching = NO
* Clear the kentico cache from the system application to get rid of the 404 error

3.  Register 2 new web parts in the WorkBC folder

WorkBC JBAccount Application
WorkBC\CMSWebParts\JobBoard\JBAccount.ascx

WorkBC JBSearch Application   
WorkBC\CMSWebParts\JobBoard\JBSearch.ascx

4.  Add the webparts to the MiddleZone of the 2 corresponding pages created above


*KENTICO Search tile updates:
*Update Kentico homepage search tile text

_For the most current copy, see WBCJB-1403_

1. Login to Kentic admin area
2. Click on the top left icon that looks like a star (Applications menu)
3. Choose "Content management"
4. Choose "Pages"
5. A list of all the pages will display. Choose the "Home" page.
6. Click on the "Design" tab in the browser.
7. Click on the hamburger menu on the "JobSearchTitle" widget and choose "Configure"
8. Set the new values (see screenshot "UpdateSearchTile.jpg" & Jira WBCJB-1403)
9. Click on the "Save & close" button.

The Kentico search tile content will now be updated to the values provided.