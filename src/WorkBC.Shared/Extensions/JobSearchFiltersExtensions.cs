using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkBC.ElasticSearch.Models.Filters;
using WorkBC.Shared.Constants;

namespace WorkBC.Shared.Extensions
{
    public static class JobSearchFiltersExtensions
    {
        public static string BookmarkableUrl(this JobSearchFilters filters)
        {
            string matrixParams = filters.KeywordsParam()
                                  + filters.LocationParams()
                                  + filters.JobTypeParams()
                                  + filters.SalaryParams()
                                  + filters.IndustryParams()
                                  + filters.EducationParams()
                                  + filters.DateParams()
                                  + filters.MoreParams();

            return matrixParams == string.Empty
                ? ";sortby=1;"
                : $";{matrixParams}sortby=1;";
        }

        private static string KeywordsParam(this JobSearchFilters filters)
        {
            if (string.IsNullOrEmpty(filters.Keyword))
            {
                return string.Empty;
            }

            switch (filters.SearchInField.ToLower())
            {
                case "employer":
                    return $"employer={MyUrlEncode(filters.Keyword)};";
                case "jobid":
                    return $"job={MyUrlEncode(filters.Keyword)};";
                case "title":
                    return $"title={MyUrlEncode(filters.Keyword)};";
                default:
                    return $"search={MyUrlEncode(filters.Keyword)};";
            }
        }

        private static string LocationParams(this JobSearchFilters filters)
        {
            if (filters.SearchLocations == null || filters.SearchLocations.Count == 0)
            {
                return string.Empty;
            }

            var cityList = new List<string>();
            var postalList = new List<string>();
            var regionList = new List<string>();

            foreach (LocationField location in filters.SearchLocations)
            {
                if (!string.IsNullOrEmpty(location.City))
                {
                    cityList.Add(MyUrlEncode(location.City));
                }
                else if (!string.IsNullOrEmpty(location.Postal))
                {
                    postalList.Add(MyUrlEncode(location.Postal.Replace(" ", string.Empty).ToUpper()));
                }
                else if (!string.IsNullOrEmpty(location.Region))
                {
                    string region = location.Region
                        .Replace(" ", "")
                        .Replace("-", "")
                        .Replace("&", "")
                        .Replace("/", "");
                    regionList.Add(MyUrlEncode(region));
                }
            }

            var locationParams = string.Empty;

            if (cityList.Any())
            {
                locationParams += $"city={string.Join(",", cityList)};";
            }

            if (postalList.Any())
            {
                locationParams += $"postal={string.Join(",", postalList)};";
            }

            if (regionList.Any())
            {
                locationParams += $"region={string.Join(",", regionList)};";
            }

            if (filters.SearchLocations.Count == 1 && (cityList.Count == 1 || postalList.Count == 1))
            {
                int radius = filters.SearchLocationDistance;

                if (radius != -1)
                {
                    locationParams += $"radius={radius};";
                }
            }

            return locationParams;
        }

        private static string JobTypeParams(this JobSearchFilters filters)
        {
            var jobTypeParams = string.Empty;
            var hoursOfWorkList = new List<int>();

            // 1, 'Full-time'
            if (filters.SearchJobTypeFullTime)
            {
                hoursOfWorkList.Add(1);
            }

            // 2, 'Part-time'
            if (filters.SearchJobTypePartTime)
            {
                hoursOfWorkList.Add(2);
            }

            // 3, 'Part-time leading to full-time'
            if (filters.SearchJobTypeLeadingToFullTime)
            {
                hoursOfWorkList.Add(3);
            }

            if (hoursOfWorkList.Any())
            {
                jobTypeParams += $"hoursofwork={string.Join(",", hoursOfWorkList)};";
            }

            var periodOfEmploymentList = new List<int>();

            // 1, 'Permanent'
            if (filters.SearchJobTypePermanent)
            {
                periodOfEmploymentList.Add(1);
            }

            // 2, 'Temporary'
            if (filters.SearchJobTypeTemporary)
            {
                periodOfEmploymentList.Add(2);
            }

            // 3, 'Casual'
            if (filters.SearchJobTypeCasual)
            {
                periodOfEmploymentList.Add(3);
            }

            // 4, 'Seasonal'
            if (filters.SearchJobTypeSeasonal)
            {
                periodOfEmploymentList.Add(4);
            }

            if (periodOfEmploymentList.Any())
            {
                jobTypeParams += $"periodofemployment={string.Join(",", periodOfEmploymentList)};";
            }

            var employmentTermsList = new List<int>();

            // 1, 'Day'
            if (filters.SearchJobTypeDay)
            {
                employmentTermsList.Add(1);
            }

            // 2, 'Early morning'
            if (filters.SearchJobTypeEarly)
            {
                employmentTermsList.Add(2);
            }

            // 3, 'Evening'
            if (filters.SearchJobTypeEvening)
            {
                employmentTermsList.Add(3);
            }

            // 4, 'Flexible hours'
            if (filters.SearchJobTypeFlexible)
            {
                employmentTermsList.Add(4);
            }

            // 5, 'Morning'
            if (filters.SearchJobTypeMorning)
            {
                employmentTermsList.Add(5);
            }

            // 6, 'Night'
            if (filters.SearchJobTypeNight)
            {
                employmentTermsList.Add(6);
            }

            // 7, 'On call'
            if (filters.SearchJobTypeOnCall)
            {
                employmentTermsList.Add(7);
            }

            // 8, 'Overtime'
            if (filters.SearchJobTypeOvertime)
            {
                employmentTermsList.Add(8);
            }

            // 9, 'Shift'
            if (filters.SearchJobTypeShift)
            {
                employmentTermsList.Add(9);
            }

            // 10, 'To be determined'
            if (filters.SearchJobTypeTbd)
            {
                employmentTermsList.Add(10);
            }

            // 12, 'Weekend'
            if (filters.SearchJobTypeWeekend)
            {
                employmentTermsList.Add(12);
            }

            if (employmentTermsList.Any())
            {
                jobTypeParams += $"employmentterms={string.Join(",", employmentTermsList)};";
            }

            var workplaceTypeList = new List<int>();

            // 0, "On-site only",
            if (filters.SearchJobTypeOnSite)
            {
                workplaceTypeList.Add((int)WorkplaceTypeId.OnSite);
            }

            // 100000, "On-site or remote work",
            if (filters.SearchJobTypeHybrid)
            {
                workplaceTypeList.Add((int)WorkplaceTypeId.Hybrid);
            }

            // 100001 "Work location varies",
            if (filters.SearchJobTypeTravelling)
            {
                workplaceTypeList.Add((int)WorkplaceTypeId.Travelling);
            }

            // 15141, "Virtual job"
            if (filters.SearchJobTypeVirtual)
            {
                workplaceTypeList.Add((int)WorkplaceTypeId.Virtual);
            }

            if (workplaceTypeList.Any())
            {
                jobTypeParams += $"workplacetype={string.Join(",", workplaceTypeList)};";
            }

            return jobTypeParams;
        }

        private static string SalaryParams(this JobSearchFilters filters)
        {
            var salaryParams = string.Empty;

            if (filters.SearchSalaryConditions != null)
            {
                var salaryConditions = new Dictionary<string, int>
                {
                    { "as per collective agreement", 1 },
                    { "bonus", 2 },
                    { "commission", 3 },
                    { "dental benefits", 4 },
                    { "disability benefits", 5 },
                    { "gratuities", 6 },
                    { "group insurance benefits", 7 },
                    { "life insurance benefits", 8 },
                    { "medical benefits", 9 },
                    { "mileage paid", 10 },
                    { "pension plan benefits", 11 },
                    { "piece work", 12 },
                    { "resp benefits", 13 },
                    { "rrsp benefits", 14 },
                    { "vision care benefits", 15 },
                    { "other benefits", 16 }
                };

                var salaryConditionList = new List<int>();

                foreach (string condition in filters.SearchSalaryConditions)
                {
                    if (salaryConditions.ContainsKey(condition.ToLower()))
                    {
                        salaryConditionList.Add(salaryConditions[condition.ToLower()]);
                    }
                }

                if (salaryConditionList.Any())
                {
                    salaryConditionList = salaryConditionList.OrderBy(i => i).ToList();
                    salaryParams += $"benefits={string.Join(",", salaryConditionList)};";
                }
            }

            var salaryBracketList = new List<int>();

            if (filters.SalaryBracket1)
            {
                salaryBracketList.Add(1);
            }

            if (filters.SalaryBracket2)
            {
                salaryBracketList.Add(2);
            }

            if (filters.SalaryBracket3)
            {
                salaryBracketList.Add(3);
            }

            if (filters.SalaryBracket4)
            {
                salaryBracketList.Add(4);
            }

            if (filters.SalaryBracket5)
            {
                salaryBracketList.Add(5);
            }

            if (filters.SalaryBracket6)
            {
                salaryBracketList.Add(6);
            }

            if (filters.SearchSalaryUnknown)
            {
                salaryBracketList.Add(7);
            }

            if (salaryBracketList.Any())
            {
                salaryParams += $"salaryrange={string.Join(",", salaryBracketList)};";
            }

            if (filters.SalaryBracket6)
            {
                if (filters.SalaryMin != null)
                {
                    int.TryParse(filters.SalaryMin, out int min);
                    salaryParams += $"salaryrangemin={min};";

                    if (filters.SalaryMax != null)
                    {
                        string salaryMax = filters.SalaryMax;
                        if (salaryMax.Trim() == "" || salaryMax.Trim() == "unlimited")
                        {
                            salaryMax = "0";
                        }

                        int.TryParse(salaryMax, out int max);
                        salaryParams += $"salaryrangemax={max};";
                    }
                }
            }

            if (filters.SalaryType != 4)
            {
                salaryParams += $"salaryinterval={filters.SalaryType};";
            }

            return salaryParams;
        }

        private static string IndustryParams(this JobSearchFilters filters)
        {
            var industryParams = string.Empty;

            if (filters.SearchIndustry != null && filters.SearchIndustry.Any())
            {
                industryParams += $"industry={string.Join(",", filters.SearchIndustry)};";
            }

            return industryParams;
        }

        private static string EducationParams(this JobSearchFilters filters)
        {
            var educationParams = string.Empty;

            if (filters.SearchJobEducationLevel != null)
            {
                var educationLevelList = new List<int>();

                var educationLevels = new Dictionary<string, int>
                {
                    { "university", 1 },
                    { "college or apprenticeship", 3 },
                    { "secondary school or job-specific training", 2 },
                    { "no education", 4 }
                };

                foreach (string level in filters.SearchJobEducationLevel)
                {
                    if (educationLevels.ContainsKey(level.ToLower()))
                    {
                        educationLevelList.Add(educationLevels[level.ToLower()]);
                    }
                }

                if (educationLevelList.Any())
                {
                    educationParams += $"education={string.Join(",", educationLevelList)};";
                }
            }

            return educationParams;
        }

        private static string DateParams(this JobSearchFilters filters)
        {
            switch (filters.SearchDateSelection)
            {
                case "1": // Today
                    return "datetype=1;";
                case "2": // Past 3 Days
                    return "datetype=2;";
                case "3": // Date range
                {
                    DateField startDate = filters.StartDate;
                    string start = $"{startDate.Year}{startDate.Month:00}{startDate.Day:00}";
                    DateField endDate = filters.EndDate;
                    string end = $"{endDate.Year}{endDate.Month:00}{endDate.Day:00}";
                    return $"datetype=3;startdate={start};enddate={end};";
                }
                default:
                    return string.Empty;
            }
        }

        private static string MoreParams(this JobSearchFilters filters)
        {
            var moreParams = string.Empty;

            // Employment groups
            var employmentGroupsList = new List<int>();

            // 1, 'Apprentices'
            if (filters.SearchIsApprentice)
            {
                employmentGroupsList.Add(1);
            }

            // 2, 'Indigenous people'
            if (filters.SearchIsIndigenous)
            {
                employmentGroupsList.Add(2);
            }

            // 3, 'Mature workers'
            if (filters.SearchIsMatureWorkers)
            {
                employmentGroupsList.Add(3);
            }

            // 4, 'Newcomers'
            if (filters.SearchIsNewcomers)
            {
                employmentGroupsList.Add(4);
            }

            // 5, 'People with disabilities'
            if (filters.SearchIsPeopleWithDisabilities)
            {
                employmentGroupsList.Add(5);
            }

            // 6, 'Students'
            if (filters.SearchIsStudents)
            {
                employmentGroupsList.Add(6);
            }

            // 7, 'Veterans'
            if (filters.SearchIsVeterans)
            {
                employmentGroupsList.Add(7);
            }

            // 8, 'Visible minorities'
            if (filters.SearchIsVisibleMinority)
            {
                employmentGroupsList.Add(8);
            }

            // 9, 'Youth'          
            if (filters.SearchIsYouth)
            {
                employmentGroupsList.Add(9);
            }

            if (employmentGroupsList.Any())
            {
                moreParams += $"employmentgroups={string.Join(",", employmentGroupsList)};";
            }

            //// 2016 NOC code
            //if (!string.IsNullOrEmpty(filters.SearchNocField))
            //{
            //    moreParams += $"noc={filters.SearchNocField};";
            //}

            // 2021 NOC code
            if (!string.IsNullOrEmpty(filters.SearchNocField))
            {
                moreParams += $"noc={filters.SearchNocField};";
            }

            // Job posting language = English and French
            if (filters.SearchIsPostingsInEnglishAndFrench)
            {
                moreParams += "language=1;";
            }

            // Job posting language
            if (filters.SearchExcludePlacementAgencyJobs)
            {
                moreParams += "placementagency=1;";
            }

            // Job source = WorkBC
            if (filters.SearchJobSource == "1")
            {
                moreParams += "jobsource=1;";
            }

            // Job source =  External (other job boards)
            if (filters.SearchJobSource == "2")
            {
                moreParams += "jobsource=2;";
            }
            
            // Job source =  Federal Jobs (other job boards)
            if (filters.SearchJobSource == "3")
            {
                moreParams += "jobsource=3;";
            }
            
            // Job source =  Municipal Jobs (other job boards)
            if (filters.SearchJobSource == "4")
            {
                moreParams += "jobsource=4;";
            }
            
            // Job source =  Provincial Jobs (other job boards)
            if (filters.SearchJobSource == "5")
            {
                moreParams += "jobsource=5;";
            }

            return moreParams;
        }

        private static string MyUrlEncode(string str)
        {
            return HttpUtility.UrlEncode((str ?? "").Trim())
                .Replace("+", "%20")
                .Replace("%2c", ",")
                .Replace("%40", "@")
                .Replace("%23", "%20") // # to space
                .Replace("%24", "%20") // % to space
                .Replace("%26", "&")
                .Replace("%2b", "+")
                .Replace("%3d", "=");
        }
    }
}