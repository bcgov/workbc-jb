using Newtonsoft.Json;

namespace WorkBC.Web.Models.Search
{
    public partial class JobDetailsPageLabels
    {
        [JsonProperty("salary", NullValueHandling = NullValueHandling.Ignore)]
        public string Salary { get; set; }

        [JsonProperty("jobType", NullValueHandling = NullValueHandling.Ignore)]
        public string JobType { get; set; }

        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        [JsonProperty("english", NullValueHandling = NullValueHandling.Ignore)]
        public string English { get; set; }

        [JsonProperty("positionsAvailable", NullValueHandling = NullValueHandling.Ignore)]
        public string PositionsAvailable { get; set; }

        [JsonProperty("nocGroup", NullValueHandling = NullValueHandling.Ignore)]
        public string NocGroup { get; set; }

        [JsonProperty("expiresIn", NullValueHandling = NullValueHandling.Ignore)]
        public string ExpiresIn { get; set; }

        [JsonProperty("days", NullValueHandling = NullValueHandling.Ignore)]
        public string Days { get; set; }

        [JsonProperty("expires", NullValueHandling = NullValueHandling.Ignore)]
        public string Expires { get; set; }

        [JsonProperty("posted", NullValueHandling = NullValueHandling.Ignore)]
        public string Posted { get; set; }

        [JsonProperty("lastUpdated", NullValueHandling = NullValueHandling.Ignore)]
        public string LastUpdated { get; set; }

        [JsonProperty("numberOfViews", NullValueHandling = NullValueHandling.Ignore)]
        public string NumberOfViews { get; set; }

        [JsonProperty("views", NullValueHandling = NullValueHandling.Ignore)]
        public string Views { get; set; }

        [JsonProperty("jobNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string JobNumber { get; set; }

        [JsonProperty("save", NullValueHandling = NullValueHandling.Ignore)]
        public string Save { get; set; }

        [JsonProperty("print", NullValueHandling = NullValueHandling.Ignore)]
        public string Print { get; set; }

        [JsonProperty("share", NullValueHandling = NullValueHandling.Ignore)]
        public string Share { get; set; }

        [JsonProperty("jobRequirements", NullValueHandling = NullValueHandling.Ignore)]
        public string JobRequirements { get; set; }

        [JsonProperty("jobLocations", NullValueHandling = NullValueHandling.Ignore)]
        public string JobLocations { get; set; }

        [JsonProperty("experience", NullValueHandling = NullValueHandling.Ignore)]
        public string Experience { get; set; }

        [JsonProperty("credentials", NullValueHandling = NullValueHandling.Ignore)]
        public string Credentials { get; set; }

        [JsonProperty("additionalSkills", NullValueHandling = NullValueHandling.Ignore)]
        public string AdditionalSkills { get; set; }

        [JsonProperty("workSetting", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkSetting { get; set; }

        [JsonProperty("specificSkills", NullValueHandling = NullValueHandling.Ignore)]
        public string SpecificSkills { get; set; }

        [JsonProperty("securitySafety", NullValueHandling = NullValueHandling.Ignore)]
        public string SecuritySafety { get; set; }

        [JsonProperty("workSiteEnvironment", NullValueHandling = NullValueHandling.Ignore)]
        public string WorksiteEnvironment { get; set; }

        [JsonProperty("workLocationInformation", NullValueHandling = NullValueHandling.Ignore)]
        public string workLocationInformation { get; set; }

        [JsonProperty("personalSuitability", NullValueHandling = NullValueHandling.Ignore)]
        public string PersonalSuitability { get; set; }

        [JsonProperty("applyNow", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyNow { get; set; }

        [JsonProperty("online", NullValueHandling = NullValueHandling.Ignore)]
        public string Online { get; set; }

        [JsonProperty("byEmail", NullValueHandling = NullValueHandling.Ignore)]
        public string ByEmail { get; set; }

        [JsonProperty("byFax", NullValueHandling = NullValueHandling.Ignore)]
        public string ByFax { get; set; }

        [JsonProperty("byPhone", NullValueHandling = NullValueHandling.Ignore)]
        public string ByPhone { get; set; }

        [JsonProperty("benefits", NullValueHandling = NullValueHandling.Ignore)]
        public string Benefits { get; set; }

        [JsonProperty("byMail", NullValueHandling = NullValueHandling.Ignore)]
        public string ByMail { get; set; }

        [JsonProperty("inPerson", NullValueHandling = NullValueHandling.Ignore)]
        public string InPerson { get; set; }

        [JsonProperty("startDate", NullValueHandling = NullValueHandling.Ignore)]
        public string StartDate { get; set; }

        [JsonProperty("asSoonAsPossible", NullValueHandling = NullValueHandling.Ignore)]
        public string AsSoonAsPossible { get; set; }

        [JsonProperty("jobPosting", NullValueHandling = NullValueHandling.Ignore)]
        public string JobPosting { get; set; }

        [JsonProperty("education", NullValueHandling = NullValueHandling.Ignore)]
        public string Education { get; set; }

        [JsonProperty("workSchedule", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkSchedule { get; set; }
        
        [JsonProperty("workplaceType", NullValueHandling = NullValueHandling.Ignore)]
        public string WorkplaceType { get; set; }
    }
}