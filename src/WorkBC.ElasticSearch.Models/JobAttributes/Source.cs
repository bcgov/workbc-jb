using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WorkBC.ElasticSearch.Models.Helpers;

namespace WorkBC.ElasticSearch.Models.JobAttributes
{
    public class Source
    {
        [JsonIgnore]
        private string _noc;

        [JsonProperty("JobId", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringToLongConverter))]
        public long JobId { get; set; }

        [JsonProperty("Title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("DatePosted", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DatePosted { get; set; }

        [JsonProperty("ExpireDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ExpireDate { get; set; }

        [JsonProperty("EmployerName", NullValueHandling = NullValueHandling.Ignore)]
        public string EmployerName { get; set; }

        [JsonProperty("Lang", NullValueHandling = NullValueHandling.Ignore)]
        public string Lang { get; set; }

        [JsonProperty("WorkLangCd", NullValueHandling = NullValueHandling.Ignore)]
        public JobType WorkLangCd { get; set; }

        [JsonProperty("Location", NullValueHandling = NullValueHandling.Ignore)]
        public Location[] Location { get; set; }

        [JsonProperty("City", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ListToCsvConverter))]
        public string City { get; set; }

        [JsonProperty("Province", NullValueHandling = NullValueHandling.Ignore)]
        public string Province { get; set; }

        [JsonProperty("Region", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Region { get; set; }

        [JsonProperty("HoursOfWork", NullValueHandling = NullValueHandling.Ignore)]
        public JobType HoursOfWork { get; set; }

        [JsonProperty("PeriodOfEmployment", NullValueHandling = NullValueHandling.Ignore)]
        public JobType PeriodOfEmployment { get; set; }

        [JsonProperty("EmploymentTerms", NullValueHandling = NullValueHandling.Ignore)]
        public JobType EmploymentTerms { get; set; }

        [JsonProperty("EmployerTypeId", NullValueHandling = NullValueHandling.Ignore)]
        public long? EmployerTypeId { get; set; }

        [JsonProperty("WageClass", NullValueHandling = NullValueHandling.Ignore)]
        public string WageClass { get; set; }

        [JsonProperty("PostalCode", NullValueHandling = NullValueHandling.Ignore)]
        public string PostalCode { get; set; }

        [JsonProperty("WorkHours", NullValueHandling = NullValueHandling.Ignore)]
        public long? WorkHours { get; set; }

        [JsonProperty("Salary", NullValueHandling = NullValueHandling.Ignore)]
        public double? Salary { get; set; }

        [JsonProperty("IsStudent", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsStudent { get; set; }

        [JsonProperty("IsYouth", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsYouth { get; set; }

        [JsonProperty("IsApprentice", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsApprentice { get; set; }

        [JsonProperty("IsDisability", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsDisability { get; set; }

        [JsonProperty("IsAboriginal", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsAboriginal { get; set; }

        [JsonProperty("IsNewcomer", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsNewcomer { get; set; }

        [JsonProperty("IsVeteran", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsVeteran { get; set; }

        [JsonProperty("IsVismin", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsVismin { get; set; }

        [JsonProperty("IsMatureWorker", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsMatureWorker { get; set; }

        [JsonProperty("IsFederalJob", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsFederalJob { get; set; }

        [JsonProperty("SkillCategories", NullValueHandling = NullValueHandling.Ignore)]
        public List<SkillCategory> SkillCategories { get; set; }

        [JsonProperty("LastUpdated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("PositionsAvailable", NullValueHandling = NullValueHandling.Ignore)]
        public long? PositionsAvailable { get; set; }

        [JsonProperty("ApplyEmailAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyEmailAddress { get; set; }

        [JsonProperty("ApplyPhoneNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPhoneNumber { get; set; }

        [JsonProperty("ApplyPhoneNumberExt", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPhoneNumberExt { get; set; }

        [JsonProperty("ApplyPhoneNumberTimeFrom", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPhoneNumberTimeFrom { get; set; }

        [JsonProperty("ApplyPhoneNumberTimeTo", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPhoneNumberTimeTo { get; set; }

        [JsonProperty("ApplyFaxNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyFaxNumber { get; set; }

        [JsonProperty("ApplyWebsite", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyWebsite { get; set; }

        [JsonProperty("NocGroup", NullValueHandling = NullValueHandling.Ignore)]
        public string NocGroup { get; set; }

        [JsonProperty("Noc", NullValueHandling = NullValueHandling.Ignore)]
        public string Noc
        {
            get
            {
                const int nocLength = 4;
                if (string.IsNullOrWhiteSpace(_noc))
                {
                    return string.Empty;
                }
                if (_noc.Length >= nocLength)
                {
                    return _noc;
                }
                var zeroes = new string('0', nocLength);
                // pad the NOC with zeroes so it is always nocLength characters
                var s = $"{zeroes}{_noc}";
                return s.Substring(s.Length - nocLength, nocLength);
            }
            set { _noc = value; }
        }

        [JsonProperty("IsVariousLocation", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsVariousLocation { get; set; }

        [JsonProperty("SalaryConditions", NullValueHandling = NullValueHandling.Ignore)]
        public JobType SalaryConditions { get; set; }

        [JsonProperty("SalaryDescription", NullValueHandling = NullValueHandling.Ignore)]
        public string SalaryDescription { get; set; }

        [JsonProperty("SalarySummary", NullValueHandling = NullValueHandling.Ignore)]
        public string SalarySummary { get; set; }

        [JsonProperty("ProgramName", NullValueHandling = NullValueHandling.Ignore)]
        public string ProgramName { get; set; }

        [JsonProperty("ProgramDescription", NullValueHandling = NullValueHandling.Ignore)]
        public string ProgramDescription { get; set; }

        [JsonProperty("ApplyPersonStreet", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPersonStreet { get; set; }

        [JsonProperty("ApplyPersonRoom", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPersonRoom { get; set; }

        [JsonProperty("ApplyPersonCity", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPersonCity { get; set; }

        [JsonProperty("ApplyPersonProvince", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPersonProvince { get; set; }

        [JsonProperty("ApplyPersonPostalCode", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPersonPostalCode { get; set; }

        [JsonProperty("ApplyPersonTimeFrom", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPersonTimeFrom { get; set; }

        [JsonProperty("ApplyPersonTimeTo", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyPersonTimeTo { get; set; }

        [JsonProperty("ApplyMailStreet", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyMailStreet { get; set; }

        [JsonProperty("ApplyMailRoom", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyMailRoom { get; set; }

        [JsonProperty("ApplyMailCity", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyMailCity { get; set; }

        [JsonProperty("ApplyMailProvince", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyMailProvince { get; set; }

        [JsonProperty("ApplyMailPostalCode", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplyMailPostalCode { get; set; }

        [JsonProperty("Views", NullValueHandling = NullValueHandling.Ignore)]
        public int Views { get; set; }

        [JsonProperty("ExternalSource", NullValueHandling = NullValueHandling.Ignore)]
        public ExternalJobSource ExternalSource { get; set; }

        [JsonProperty("StartDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? StartDate { get; set; }

        [JsonProperty("Reason", NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; set; } // used by Recommended Jobs

        [JsonProperty("Score", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Score { get; set; }

        [JsonProperty("IsNew", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsNew { get; set; }

        [JsonProperty("WorkplaceType", NullValueHandling = NullValueHandling.Ignore)]
        public WorkplaceType WorkplaceType { get; set; }
    }
}