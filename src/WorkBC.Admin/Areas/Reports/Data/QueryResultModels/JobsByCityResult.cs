namespace WorkBC.Admin.Areas.Reports.Data.QueryResultModels
{
    public class JobsByCityResult
    {
        public string Region { get; set; }
        public string City { get; set; }
        public int Vacancies { get; set; }
        public int Postings { get; set; }
    }
}