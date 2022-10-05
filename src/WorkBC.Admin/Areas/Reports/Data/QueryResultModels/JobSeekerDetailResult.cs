using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;

namespace WorkBC.Admin.Areas.Reports.Data.QueryResultModels
{
    public class JobSeekerDetailResult
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short AccountStatus { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public DateTime? DateRegistered { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsApprentice { get; set; }
        public bool IsIndigenousPerson { get; set; }
        public bool IsMatureWorker { get; set; }
        public bool IsNewImmigrant { get; set; }
        public bool IsPersonWithDisability { get; set; }
        public bool IsStudent { get; set; }
        public bool IsVeteran { get; set; }
        public bool IsVisibleMinority { get; set; }
        public bool IsYouth { get; set; }
        public bool HasJobAlerts { get; set; }
        public bool HasSavedCareerProfiles { get; set; }
        public bool HasSavedIndustryProfiles { get; set; }
        public bool HasSavedJobs { get; set; }

        public HtmlString EmploymentGroupsHtml
        {
            get
            {
                IEnumerable<string> employmentGroups = EmploymentGroupList()
                    .Select(s => s.Replace(" ", "&nbsp;"));
                return new HtmlString(string.Join("<br>", employmentGroups));
            }
        }

        public string EmploymentGroupsText => string.Join("\r\n", EmploymentGroupList());

        public string Location
        {
            get
            {
                var sb = new List<string>();
                if (City != null)
                {
                    sb.Add(City);

                    if (Region != null)
                    {
                        sb.Add(Region);
                    }
                }
                else if (Province != null)
                {
                    sb.Add(Province);
                }

                if (Country != null)
                {
                    sb.Add(Country);
                }

                return string.Join(", ", sb);
            }
        }

        private List<string> EmploymentGroupList()
        {
            var sb = new List<string>();

            if (IsApprentice)
            {
                sb.Add("Apprentice");
            }

            if (IsIndigenousPerson)
            {
                sb.Add("Indigenous person");
            }

            if (IsMatureWorker)
            {
                sb.Add("Mature");
            }

            if (IsNewImmigrant)
            {
                sb.Add("Newcomer to BC");
            }

            if (IsPersonWithDisability)
            {
                sb.Add("Person with a disability");
            }

            if (IsStudent)
            {
                sb.Add("Student");
            }

            if (IsVeteran)
            {
                sb.Add("Veteran");
            }

            if (IsVisibleMinority)
            {
                sb.Add("Visible minority");
            }

            if (IsYouth)
            {
                sb.Add("Youth");
            }

            return sb;
        }

        public HtmlString FormatEmail()
        {
            if (Email.Length <= 25)
            {
                return new HtmlString(Email);
            }

            string email = Email.Replace("@", "@&#8203;");
            email = email.Replace(".", "&#8203;.");

            return new HtmlString(email);
        }
    }
}