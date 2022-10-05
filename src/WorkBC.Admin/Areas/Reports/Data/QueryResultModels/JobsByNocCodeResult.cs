namespace WorkBC.Admin.Areas.Reports.Data.QueryResultModels
{
    public class JobsByNocCodeResult
    {
        public string NocCode { get; set; }
        public string NocTitle { get; set; }
        public int Vacancies { get; set; }
        public int Postings { get; set; }
    }
}