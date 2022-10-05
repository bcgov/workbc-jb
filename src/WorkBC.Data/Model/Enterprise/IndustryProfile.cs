namespace WorkBC.Data.Model.Enterprise
{
    public partial class IndustryProfile
    {
        public int IndustryProfileId { get; set; }
        public int NaicsId { get; set; }
        public byte[] IndustryImage { get; set; }
        public string KeyPoints { get; set; }
        public string Overview { get; set; }
        public string Employment { get; set; }
        public string Workforce { get; set; }
        public string EmploymentTypeFullTime { get; set; }
        public string EmploymentTypeSelfEmployed { get; set; }
        public string EmploymentTypeTemp { get; set; }
        public string WorkEnvironment { get; set; }
        public string WorkEnvironmentPrivateSector { get; set; }
        public string WorkEnvironmentSmallerWorkspace { get; set; }
        public string WorkEnvironmentUnion { get; set; }
        public string WagesSummary { get; set; }
        public string RegionalDistributionSummary { get; set; }
        public string RelatedCareersIntro { get; set; }
        public string Outlook { get; set; }
        public bool IsFeatured { get; set; }
        public byte[] IndustryHeaderImage { get; set; }
        public string TableGraph1FootNote { get; set; }
        public string TableGraph2FootNote { get; set; }
        public string TableGraph3FootNote { get; set; }
        public string TableGraph4FootNote { get; set; }
        public string TableGraph5FootNote { get; set; }
        public string TableGraph6FootNote { get; set; }
        public string TableGraph7FootNote { get; set; }
        public string TableGraph8FootNote { get; set; }
        public string TableGraph9FootNote { get; set; }
        public string TableGraph10FootNote { get; set; }
        public string TableGraph11FootNote { get; set; }
        public string TableGraph12FootNote { get; set; }
        public string TableGraph13FootNote { get; set; }
        public string TableGraph14FootNote { get; set; }
        public string TableGraph15FootNote { get; set; }
        public string TableGraph16FootNote { get; set; }
        public string TableGraph17FootNote { get; set; }
        public string TableGraph18FootNote { get; set; }
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string PageKeywords { get; set; }
        public int? AnnotationNumber { get; set; }
    }
}