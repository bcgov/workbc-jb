namespace WorkBC.Web.Models
{
    public class IndustryProfileModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //public int Count { get; set; }

        // todo: don't delete this until we confirm that we are not adding job search counts and links to saved industry profiles
        // // id's to use in bookmarkable search url
        // // e.g. '/Jobs-Careers/Find-Jobs/Jobs.aspxJobs.aspx#/job-search;industry=40,1,36;'
        //public string IndustryIds { get; set; }
    }
}
