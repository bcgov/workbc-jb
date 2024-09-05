using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.Extensions.Configuration;
using WorkBC.Data.Model.JobBoard;
using WorkBC.ElasticSearch.Indexing.XmlParsingHelpers;
using WorkBC.Shared.Constants;
using WorkBC.Shared.Extensions;
using WorkBC.Shared.Services;

namespace WorkBC.ElasticSearch.Indexing.Services
{
    public class XmlParsingServiceFederal : XmlParsingServiceBase
    {
        private const int WorkplaceInfoSkillCategoryId = 100000;
        private const int HealthBenefitsCategoryId = 102001;
        private const int FinancialBenefitsCategoryId = 102002;
        private const int LongTermBenefitsCategoryId = 102003;
        private const int OtherBenefitsCategoryId = 102004;
        private const string VirtualJobBasedIn_EN = "Virtual job based in";
        private const string VirtualJobBasedIn_FR = "Emploi virtuel basé à";
        private readonly IGeocodingService _geocodingService;
        //NOC-387-Special case scenario fix for NOC 2021 codes 00011 to 00015
        private List<int> specialNocs = new List<int>() { 00011, 00012, 00013, 00014, 00015 };



        public XmlParsingServiceFederal(IConfiguration configuration) : base(configuration)
        {
            _geocodingService = new GeocodingService(JobBoardContext, configuration);
        }

        public XmlParsingServiceFederal(List<Data.Model.JobBoard.Location> duplicateCities,
            Dictionary<string, string> uniqueCities, List<NocCode> nocCodes, List<NocCode2021> nocCodes2021, IGeocodingService geocodingService) : base(duplicateCities, uniqueCities, nocCodes, nocCodes2021)
        {
            this._geocodingService = geocodingService;
        }

        public ElasticSearchJob ConvertToElasticJob(string federalXml, bool isFrench = false)
        {
            var job = new ElasticSearchJob();

            try
            {
                //read XML to XmlDocument
                var jobXml = new XmlDocument();

                //Load job xml
                jobXml.LoadXml(federalXml);

                //Get the root element
                XmlElement root = jobXml.DocumentElement;

                //Number of jobs in this XML
                //It should be 1
                var numberOfJobsFound =
                    Convert.ToInt32(root.SelectSingleNode("/SolrResponse/Header/numFound").InnerText);

                if (numberOfJobsFound != 1)
                {
                    //Job "Not available" in xml
                    Console.Write("[NA]");
                    return null;
                }

                //Read XML Node
                XmlNode xmlJobNode = root.SelectSingleNode("/SolrResponse/Documents/Document");

                //Used to convert values to decimal
                var culture = new CultureInfo("en-US");

                // get noc code
                var noc = Convert.ToInt16(xmlJobNode["noc2016"].InnerText);

                // make sure the code is valid
                if (NocCodes.All(c => c.Id != noc))
                {
                    noc = 0;
                }

                //Special case for NOC-387: The parser should convert any incoming NOC 00011-00015 into NOC 00018
                //before attempting to locate it in the database.


                //get noc 2021 code
                var noc2021 = 0;
                if (xmlJobNode["noc2021"] != null)
                {
                    noc2021 = Convert.ToInt32(xmlJobNode["noc2021"].InnerText);

                    //Special case for NOC-387: The parser should convert any incoming NOC 00011-00015 into NOC 00018
                    //before attempting to locate it in the database.

                    if (specialNocs.Contains(noc2021))
                    {
                        noc2021 = 00018;
                    }
                    // make sure the code 2021 is valid
                    if (NocCodes2021.All(c => c.Id != noc2021))
                    {
                        noc2021 = 0;
                    }
                }

                job = new ElasticSearchJob
                {
                    JobId = xmlJobNode["jobs_id"].InnerText,
                    Title = xmlJobNode["title"] != null ? xmlJobNode["title"].InnerText.Trim() : string.Empty,
                    DatePosted = Convert.ToDateTime(xmlJobNode["date_posted"].InnerText),
                    ActualDatePosted = Convert.ToDateTime(xmlJobNode["date_posted"].InnerText),
                    EmployerName = xmlJobNode["employer_name_string"] != null
                        ? xmlJobNode["employer_name_string"].InnerText.Trim()
                        : string.Empty,
                    EmployerTypeId = Convert.ToInt32(xmlJobNode["employer_type_id"].InnerText),
                    Lang = xmlJobNode["lang"].InnerText,
                    WorkLangCd = new JobLanguage(),// xmlJobNode["work_lang_cd"].InnerText,
                    PostalCode = xmlJobNode["postal_code"] != null
                        ? xmlJobNode["postal_code"].InnerText
                        : string.Empty,
                    Salary = CalculateSalary(xmlJobNode["salary_hourly"],
                        xmlJobNode["salary_weekly"],
                        xmlJobNode["salary_yearly"],
                        xmlJobNode.SelectSingleNode("salary/string")),
                    WorkHours = xmlJobNode["work_hours"] != null
                        ? decimal.Parse(xmlJobNode["work_hours"].InnerText, culture)
                        : 0,
                    WageClass = xmlJobNode["wage_class"].InnerText,
                    HoursOfWork = new HoursOfWork(),
                    PeriodOfEmployment = new PeriodOfEmployment(),
                    EmploymentTerms = new EmploymentTerms(),
                    IsAboriginal = Convert.ToBoolean(xmlJobNode["job_aboriginal_flag"]?.InnerText ?? "false"),
                    IsApprentice = Convert.ToBoolean(xmlJobNode["job_apprentice_flag"]?.InnerText ?? "false"),
                    IsStudent = Convert.ToBoolean(xmlJobNode["job_student_flag"]?.InnerText ?? "false"),
                    IsNewcomer = Convert.ToBoolean(xmlJobNode["job_newcomer_flag"]?.InnerText ?? "false"),
                    IsVeteran = Convert.ToBoolean(xmlJobNode["job_veteran_flag"]?.InnerText ?? "false"),
                    IsVismin = Convert.ToBoolean(xmlJobNode["job_vismin_flag"]?.InnerText ?? "false"),
                    IsYouth = Convert.ToBoolean(xmlJobNode["job_youth_flag"]?.InnerText ?? "false"),
                    IsMatureWorker = Convert.ToBoolean(xmlJobNode["job_senior_flag"]?.InnerText ?? "false"),
                    IsDisability = Convert.ToBoolean(xmlJobNode["job_disability_flag"]?.InnerText ?? "false"),
                    Location = new Location[] { },
                    SkillCategories = new List<SkillCategory>(),
                    SalaryConditions = new SalaryConditions(),
                    IsFederalJob = true,
                    ExpireDate = Convert.ToDateTime(xmlJobNode["display_until"].InnerText),
                    LastUpdated = Convert.ToDateTime(xmlJobNode["file_update_date"].InnerText),
                    PositionsAvailable = Convert.ToInt32(xmlJobNode["num_positions"].InnerText),
                    Noc = noc == 0 ? (int?)null : noc,
                    Noc2021 = noc2021 == 0 ? (int?)null : noc2021,
                    IsVariousLocation = Convert.ToBoolean(xmlJobNode["various_location_flag"].InnerText),
                    ProgramName = xmlJobNode["program_name"] != null
                        ? xmlJobNode["program_name"].InnerText
                        : string.Empty,
                    ProgramDescription = xmlJobNode["program_desc"] != null
                        ? xmlJobNode["program_desc"].InnerText
                        : string.Empty,
                    NaicsId = xmlJobNode["naics_id"] != null ? Convert.ToInt32(xmlJobNode["naics_id"].InnerText) : 0
                };

                #region Hours of work

                job.HoursOfWork.Description = new List<string>
                {
                    xmlJobNode["work_period_cd"].InnerText
                };

                switch (job.HoursOfWork.Description[0].ToLower())
                {
                    case "l":
                        job.HoursOfWork.Description[0] = "Part-time leading to full-time";
                        break;
                    case "f":
                        job.HoursOfWork.Description[0] = "Full-time";
                        break;
                    case "p":
                        job.HoursOfWork.Description[0] = "Part-time";
                        break;
                }

                #endregion

                #region Job Language

                job.WorkLangCd.Description = new List<string>
                {
                    xmlJobNode["work_lang_cd"].InnerText
                };

                switch (job.WorkLangCd.Description[0].ToLower())
                {
                    case "e":
                        job.WorkLangCd.Description[0] = "English";
                        break;
                    case "f":
                        job.WorkLangCd.Description[0] = "French";
                        break;
                    case "u":
                        job.WorkLangCd.Description[0] = "English or French";
                        break;
                    case "b":
                        job.WorkLangCd.Description[0] = "Bilingual";
                        break;
                    case "o":
                        job.WorkLangCd.Description[0] = "Other";
                        break;
                }
                #endregion

                #region Period of Employment

                job.PeriodOfEmployment.Description = new List<string>
                {
                    xmlJobNode["work_term_cd"].InnerText
                };

                switch (job.PeriodOfEmployment.Description[0].ToLower())
                {
                    case "t":
                        job.PeriodOfEmployment.Description[0] = "Temporary";
                        break;
                    case "p":
                        job.PeriodOfEmployment.Description[0] = "Permanent";
                        break;
                    case "c":
                        job.PeriodOfEmployment.Description[0] = "Casual";
                        break;
                    case "s":
                        job.PeriodOfEmployment.Description[0] = "Seasonal";
                        break;
                    default:
                        //clear if we could not find any match
                        job.PeriodOfEmployment.Description[0] = string.Empty;
                        break;
                }

                #endregion

                #region Employment terms

                job.EmploymentTerms.Description = new List<string>();

                if (xmlJobNode["employment_terms"] != null)
                {
                    //structure in XML looks like <string>Day</string><string>Night</string>
                    foreach (XmlNode s in xmlJobNode.SelectSingleNode("employment_terms").SelectNodes("string"))
                    {
                        // capitalized the first letter only
                        string term = s.InnerText;
                        if (!string.IsNullOrWhiteSpace(term))
                        {
                            job.EmploymentTerms.Description.Add(term.Capitalize());
                        }
                    }
                }

                #endregion

                #region Skills

                job.WorkplaceType = new WorkplaceType
                {
                    Id = (int)WorkplaceTypeId.OnSite,
                    Description = isFrench
                        ? WorkPlaceTypeFrench.OnSite
                        : WorkPlaceTypeEnglish.OnSite
                };

                XmlNodeList xmlSkillCategories = xmlJobNode.SelectNodes("skill_categories/skill_category");
                foreach (XmlNode skillCategory in xmlSkillCategories)
                {
                    var id = Convert.ToInt32(skillCategory.Attributes["id"].Value);
                    string name = skillCategory.FirstChild.InnerText;
                    XmlNodeList xmlSkillOptions;

                    if (id == WorkplaceInfoSkillCategoryId)
                    {
                        XmlNode workplaceInfo = skillCategory.SelectSingleNode("options/option_name");
                        var workPlaceTypeId = Convert.ToInt32(workplaceInfo.Attributes["id"].Value);

                        switch (workPlaceTypeId)
                        {
                            case (int)WorkplaceTypeId.Hybrid:
                                job.WorkplaceType = new WorkplaceType
                                {
                                    Id = (int)WorkplaceTypeId.Hybrid,
                                    Description = isFrench
                                        ? WorkPlaceTypeFrench.Hybrid
                                        : WorkPlaceTypeEnglish.Hybrid
                                };
                                break;
                            case (int)WorkplaceTypeId.Travelling:
                                job.WorkplaceType = new WorkplaceType
                                {
                                    Id = (int)WorkplaceTypeId.Travelling,
                                    Description = isFrench
                                        ? WorkPlaceTypeFrench.Travelling
                                        : WorkPlaceTypeEnglish.Travelling
                                };
                                break;
                            case (int)WorkplaceTypeId.Virtual:
                                job.WorkplaceType = new WorkplaceType
                                {
                                    Id = (int)WorkplaceTypeId.Virtual,
                                    Description = isFrench
                                        ? WorkPlaceTypeFrench.Virtual
                                        : WorkPlaceTypeEnglish.Virtual
                                };
                                break;
                        }

                        continue;
                    }

                    if (id == FinancialBenefitsCategoryId || id == LongTermBenefitsCategoryId)
                    {
                        xmlSkillOptions = skillCategory.SelectNodes("options/option_name");
                        foreach (XmlNode option in xmlSkillOptions)
                        {
                            // capitalize the first letter and lower case all others
                            var benefit = option.InnerText.Capitalize();

                            if (benefit.Contains("rrsp"))
                            {
                                benefit = "RRSP benefits";
                            }
                            
                            if (benefit.Contains("resp"))
                            {
                                benefit = "RESP benefits";
                            }
                            
                            if (benefit.Contains("Life insurance"))
                            {
                                benefit = "Life insurance benefits";
                            }
                            
                            if (benefit.Contains("Pension plan"))
                            {
                                benefit = "Pension plan benefits";
                            }

                            job.SalaryConditions.Description.Add(benefit);
                        }

                        continue;
                    }
                    
                    if (id == HealthBenefitsCategoryId)
                    {
                        xmlSkillOptions = skillCategory.SelectNodes("options/option_name");
                        foreach (XmlNode option in xmlSkillOptions)
                        {
                            var benefit = option.InnerText.Capitalize();
                            
                            /*
                             Examples of Health Benefits include:
                              - Dental plan
                              - Disability benefits
                              - Health care plan
                              - Vision care benefits
                            */

                            job.SalaryConditions.Description.Add(benefit);
                        }

                        continue;
                    }

                    
                    if (id == OtherBenefitsCategoryId)
                    {
                        job.SalaryConditions.Description.Add("Other benefits");

                        continue;
                    }

                    switch (name.ToLower())
                    {
                        case "education":
                            //All Education categories should have the ID 195 (Feds)
                            id = 195;
                            break;
                        case "études":
                            //All Education categories should have the ID 195 (Feds)
                            id = 195;
                            break;
                    }

                    var sc = new SkillCategory
                    {
                        Category = new Category
                        {
                            Id = id,
                            Name = name
                        },
                        Skills = new List<string>()
                    };

                    xmlSkillOptions = skillCategory.SelectNodes("options/option_name");
                    foreach (XmlNode option in xmlSkillOptions)
                    {
                        sc.Skills.Add(option.InnerText);
                    }

                    sc.SkillCount = sc.Skills.Count;

                    job.SkillCategories.Add(sc);
                }

                #endregion

                #region Job location

                if (job.WorkplaceType.Id == (int)WorkplaceTypeId.Virtual)
                {
                    string employerPostalCode = xmlJobNode["employer_postal_code"] != null
                        ? xmlJobNode["employer_postal_code"].InnerText
                        : string.Empty;

                    string postalCodeKey = employerPostalCode + ", CANADA";
                    GeocodedLocationCache cacheLocation = _geocodingService.GetLocation(postalCodeKey).Result;

                    if (cacheLocation == null && employerPostalCode.Length == 6)
                    {
                        string employerPostalCode2 = employerPostalCode.Substring(0, 3);
                        postalCodeKey = employerPostalCode2 + ", CANADA";
                        cacheLocation = _geocodingService.GetLocation(postalCodeKey).Result;
                    }

                    if (cacheLocation == null)
                    {
                        job.City = isFrench
                            ? new[] { $"{VirtualJobBasedIn_FR} {employerPostalCode}" }
                            : new[] { $"{VirtualJobBasedIn_EN} {employerPostalCode}" };
                    }
                    else
                    {
                        if (isFrench)
                        {
                            job.City = cacheLocation.FrenchCity != null
                                ? new[]
                                {
                                    $"{VirtualJobBasedIn_FR} {cacheLocation.FrenchCity}, {cacheLocation.Province}"
                                }
                                : new[] { $"{VirtualJobBasedIn_FR} {cacheLocation.Province}" };
                        }
                        else
                        {
                            if (cacheLocation.City == null && cacheLocation.Province == null && ! string.IsNullOrWhiteSpace(employerPostalCode))
                            {
                                switch (employerPostalCode[..1])
                                {
                                    case "A":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Terre-Neuve-et-Labrador" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Newfoundland and Labrador" };
                                        break;
                                    case "B":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Nouvelle-Écosse" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Nova Scotia" };
                                        break;
                                    case "C":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Île-du-Prince-Édouard" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Prince Edward Island" };
                                        break;
                                    case "E":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Nouveau-Brunswick" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} New Brunswick" };
                                        break;
                                    case "G":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Est du Québec" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Eastern Quebec" };
                                        break;
                                    case "H":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Grand Montréal" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Metropolitan Montréal" };
                                        break;
                                    case "J":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Ouest du Québec" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Western Quebec" };
                                        break;
                                    case "K":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Est de l'Ontario" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Eastern Ontario" };
                                        break;
                                    case "L":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Centre de l'Ontario" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Central Ontario" };
                                        break;
                                    case "M":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Grand Toronto" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Metropolitan Toronto" };
                                        break;
                                    case "N":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Sud-ouest de l'Ontario" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Southwestern Ontario" };
                                        break;
                                    case "P":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Nord de l'Ontario" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Northern Ontario" };
                                        break;
                                    case "R":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Manitoba" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Manitoba" };
                                        break;
                                    case "S":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Saskatchewan" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Saskatchewan" };
                                        break;
                                    case "T":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Alberta" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Alberta" };
                                        break;
                                    case "V":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Colombie-Britannique" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} British Columbia" };
                                        break;
                                    case "X":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Territoires du Nord-Ouest et Nunavut" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Northwest Territories and Nunavut" };
                                        break;
                                    case "Y":
                                        job.City = isFrench ? new[] { $"{VirtualJobBasedIn_FR} Yukon" }
                                                            : new[] { $"{VirtualJobBasedIn_EN} Yukon" };
                                        break;
                                    default:
                                        job.City = isFrench ? new[] { $"Emploi virtuel, base inconnue" }
                                                            : new[] { $"Virtual job, base unknown" };
                                        break;
                                }
                            }
                            else
                            {
                                job.City = cacheLocation.City != null
                                    ? new[] { $"{VirtualJobBasedIn_EN} {cacheLocation.City}, {cacheLocation.Province}" }
                                    : new[] { $"{VirtualJobBasedIn_EN} {cacheLocation.Province}" };
                            }
                        }
                    }

                    job.Region = new string[] { };
                }
                else
                {
                    var locations = new FederalXmlLocations(xmlJobNode);
                    job.Location = locations.Locations;
                    job.LocationGeo = locations.LocationGeos.Distinct().ToArray();
                    job.Province = locations.Province;

                    var cities = new List<string>();
                    var regions = new List<string>();

                    for (var i = 0; i < locations.Count; i++)
                    {
                        var cityName = locations.CityNames[i];

                        // call CleanUpCityName to apply XmlManualOverrides
                        cityName = CleanUpCityName(cityName);

                        cities.Add(GetJobCity(cityName, locations.CityIds[i]));
                        regions.Add(GetJobRegion(
                            cityName,
                            locations.CityIds[i],
                            UniqueCities,
                            DuplicateCities
                        ));
                    }

                    job.Region = regions.Distinct().ToArray();
                    job.City = cities.Distinct().ToArray();
                }

                #endregion

                #region Salary conditions

                XmlNodeList salaryConditions = xmlJobNode.SelectNodes("salary_conditions/string");
                foreach (XmlNode condition in salaryConditions)
                {
                    string benefit = condition.InnerText.Capitalize();
                    job.SalaryConditions.Description.Add(benefit);
                }

                #endregion

                #region Apply details - email

                if (xmlJobNode.SelectSingleNode("app_methods/app_email") != null)
                {
                    job.ApplyEmailAddress = xmlJobNode.SelectSingleNode("app_methods/app_email").InnerText;
                }
                else
                {
                    job.ApplyEmailAddress = string.Empty;
                }

                #endregion

                #region Apply details - phone number

                if (xmlJobNode.SelectSingleNode("app_methods/app_phone/app_phone_number") != null)
                {
                    job.ApplyPhoneNumber = xmlJobNode.SelectSingleNode("app_methods/app_phone/app_phone_number")
                        .InnerText;
                }
                else
                {
                    job.ApplyPhoneNumber = string.Empty;
                }

                if (xmlJobNode.SelectSingleNode("app_methods/app_phone/app_phone_ext") != null)
                {
                    job.ApplyPhoneNumberExt =
                        xmlJobNode.SelectSingleNode("app_methods/app_phone/app_phone_ext").InnerText;
                }

                if (xmlJobNode.SelectSingleNode("app_methods/app_phone/app_phone_end_bus_hours") != null &&
                    xmlJobNode.SelectSingleNode("app_methods/app_phone/app_phone_start_bus_hours") != null)
                {
                    string startHours = xmlJobNode
                        .SelectSingleNode("app_methods/app_phone/app_phone_start_bus_hours").InnerText;
                    string endHours = xmlJobNode.SelectSingleNode("app_methods/app_phone/app_phone_end_bus_hours")
                        .InnerText;

                    if (!string.IsNullOrEmpty(startHours))
                    {
                        decimal startHoursDecimal = Convert.ToDecimal(startHours) / 60;
                        decimal endHoursDecimal = Convert.ToDecimal(endHours) / 60;

                        job.ApplyPhoneNumberTimeFrom = ConvertToTime(startHoursDecimal);
                        job.ApplyPhoneNumberTimeTo = ConvertToTime(endHoursDecimal);
                    }
                }

                if (xmlJobNode.SelectSingleNode("app_methods/app_fax") != null)
                {
                    job.ApplyFaxNumber = xmlJobNode.SelectSingleNode("app_methods/app_fax").InnerText;
                }

                #endregion

                #region Apply details - website

                if (xmlJobNode.SelectSingleNode("app_methods/app_online") != null)
                {
                    job.ApplyWebsite = xmlJobNode.SelectSingleNode("app_methods/app_online").InnerText;
                }
                else
                {
                    job.ApplyWebsite = string.Empty;
                }

                #endregion

                #region Education level

                string eduLevel = xmlJobNode["edulevel_cd"].InnerText;
                var levelOfStudy = Convert.ToInt32(xmlJobNode["los_id"].InnerText);

                //level of study for education level
                switch (levelOfStudy)
                {
                    case 2:
                        job.EduLevel = "Secondary school or job-specific training";
                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        job.EduLevel = "College or apprenticeship";
                        break;
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        job.EduLevel = "University";
                        break;
                    default:
                        job.EduLevel = "No education";
                        break;
                }

                #endregion

                //#region Noc

                //job.NocGroup = GetNocGroup(job.Noc2021, isFrench);
                //job.NocJobTitle = job.Title;

                //#endregion

                #region Noc2021

                job.NocGroup = GetNocGroup2021(job.Noc2021, isFrench);
                job.NocJobTitle = job.Title;

                #endregion

                #region Salary Description / Salary Summary

                if (xmlJobNode.SelectSingleNode("salary/string") != null && xmlJobNode["hours"] != null)
                {
                    job.SalaryDescription = xmlJobNode.SelectSingleNode("salary/string").InnerText +
                                            (isFrench ? " pour " : " for ") + xmlJobNode["hours"].InnerText;
                }

                if (xmlJobNode.SelectSingleNode("salary/string") != null)
                {
                    job.SalarySummary = xmlJobNode.SelectSingleNode("salary/string").InnerText;

                    // remove the cents from annual jobs
                    if (job.SalarySummary.Contains("annually"))
                    {
                        job.SalarySummary = job.SalarySummary.Replace(".00", "");
                    }
                }
                else if ((job.Salary ?? 0m) > 0m)
                {
                    decimal yearlySalary = xmlJobNode["salary_yearly"] != null
                        ? decimal.Parse(xmlJobNode["salary_yearly"].InnerText, culture)
                        : 0;

                    decimal hourlySalary = xmlJobNode["salary_hourly"] != null
                        ? decimal.Parse(xmlJobNode["salary_hourly"].InnerText, culture)
                        : 0;

                    if (yearlySalary > 0)
                    {
                        job.SalarySummary = $"${yearlySalary:#,##0} annually";
                    }
                    else if (hourlySalary > 0)
                    {
                        job.SalarySummary = $"{hourlySalary:C} hourly";
                    }
                    else
                    {
                        decimal weeklySalary = xmlJobNode["salary_weekly"] != null
                            ? decimal.Parse(xmlJobNode["salary_weekly"].InnerText, culture)
                            : 0;

                        if (weeklySalary > 0)
                        {
                            job.SalarySummary = $"{weeklySalary:C} weekly";
                        }
                        else
                        {
                            job.SalarySummary = "N/A";
                        }
                    }
                }

                // In the event that the jobs have invalid salary information (e.g. below minimum wage),
                // default to showing the salary string. Provincial legislation prohibits displaying
                // jobs without a salary
                if (job.Salary == null &&
                    (xmlJobNode["salary_yearly"] != null || xmlJobNode["salary_hourly"] != null))
                {
                    job.SalarySummary = xmlJobNode.SelectSingleNode("salary/string").InnerText;
                    job.SalaryDescription = xmlJobNode.SelectSingleNode("salary/string").InnerText;
                }

                #region Apply person information

                job.ApplyPersonRoom = xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_room") != null
                    ? xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_room").InnerText
                    : string.Empty;

                job.ApplyPersonStreet =
                    xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_street") != null
                        ? xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_street").InnerText
                        : string.Empty;

                job.ApplyPersonCity = xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_city") != null
                    ? xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_city").InnerText
                    : string.Empty;

                job.ApplyPersonPostalCode =
                    xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_pstlcd") != null
                        ? xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_pstlcd").InnerText
                        : string.Empty;

                job.ApplyPersonProvince =
                    xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_province") != null
                        ? xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_province").InnerText
                        : string.Empty;

                string startHoursPerson =
                    xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_start_bus_hours") != null
                        ? xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_start_bus_hours").InnerText
                        : string.Empty;

                string endHoursPerson =
                    xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_end_bus_hours") != null
                        ? xmlJobNode.SelectSingleNode("app_methods/app_person/app_person_end_bus_hours").InnerText
                        : string.Empty;

                if (!string.IsNullOrEmpty(startHoursPerson))
                {
                    decimal startHoursPersonDecimal = Convert.ToDecimal(startHoursPerson) / 60;
                    decimal endHoursPersonDecimal = Convert.ToDecimal(endHoursPerson) / 60;

                    job.ApplyPersonTimeFrom = ConvertToTime(startHoursPersonDecimal);
                    job.ApplyPersonTimeTo = ConvertToTime(endHoursPersonDecimal);
                }

                #endregion

                #region Apply mail information

                job.ApplyMailRoom = xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_room") != null
                    ? xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_room").InnerText
                    : string.Empty;

                job.ApplyMailStreet = xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_street") != null
                    ? xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_street").InnerText
                    : string.Empty;

                job.ApplyMailCity = xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_city") != null
                    ? xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_city").InnerText
                    : string.Empty;

                job.ApplyMailPostalCode =
                    xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_pstlcd") != null
                        ? xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_pstlcd").InnerText
                        : string.Empty;

                job.ApplyMailProvince =
                    xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_province") != null
                        ? xmlJobNode.SelectSingleNode("app_methods/app_mail/app_mail_province").InnerText
                        : string.Empty;

                #endregion

                #region Start date

                if (xmlJobNode["start_date"] != null)
                {
                    if (xmlJobNode["start_date"].InnerText != string.Empty)
                    {
                        // add 8 hours because we are in BC (Startdate is only used as a date, not as a time)
                        job.StartDate = Convert.ToDateTime(xmlJobNode["start_date"].InnerText).AddHours(8);
                    }
                }

                #endregion

                #endregion

                SetSalarySort(job);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ConvertToElasticJob(), reason: " + ex.Message);
                Console.WriteLine("XML: " + federalXml);
                Console.WriteLine(ex);
            }

            return job;
        }

        /// <summary>
        ///     Convert all salaries to annual salaries
        ///     Hourly - hourly rate is multiplied by 2080 (approximate number of work hours in a year)
        ///     Weekly - weekly rate is multiplied by 52 (number of weeks in a year)
        /// </summary>
        private decimal? CalculateSalary(XmlElement hourlyEl, XmlElement weeklyEl, XmlElement yearlyEl, XmlNode salaryStringEl)
        {
            decimal hourlyRate = 0;
            decimal weeklyRate = 0;
            decimal yearlyRate = 0;
            string salaryString = "";

            if (hourlyEl != null)
            {
                decimal.TryParse(hourlyEl.InnerText, out hourlyRate);
            }

            if (weeklyEl != null)
            {
                decimal.TryParse(weeklyEl.InnerText, out weeklyRate);
            }

            if (yearlyEl != null)
            {
                decimal.TryParse(yearlyEl.InnerText, out yearlyRate);
            }

            if (salaryStringEl != null && !string.IsNullOrEmpty(salaryStringEl.InnerText))
            {
                salaryString = salaryStringEl.InnerText;
            }

            // always use 40 hours so hourly, annual, monthly, weekly and biweekly filters are consistent
            var weeklyWorkHours = 40m;

            // sanitize for really big numbers
            var maxHourlyRate = 2500m;
            var maxWeeklySalary = 100000m;
            var maxYearlySalary = 5000000m;

            // Minimum is wage as at June 2024. This doesn't have to be exact.  The purpose of
            // this is to filter out some bad data coming from the national job bank, not to ensure,
            // that employers pay minimum wage.
            const decimal minimumWage = 17.40m;

            // if hourly is going to appear on the job listing, then use hourly wage
            if (salaryString.ToLower().Contains("hour"))
            {
                // allow 90% of minimum wage (see note above)
                if (hourlyRate >= 0.9m * minimumWage && hourlyRate < maxHourlyRate)
                {
                    return hourlyRate * weeklyWorkHours * 52;
                }
            }

            if (yearlyRate >= minimumWage * weeklyWorkHours * 52 && yearlyRate < maxYearlySalary)
            {
                return yearlyRate;
            }

            if (hourlyRate >= minimumWage && hourlyRate < maxHourlyRate)
            {
                return hourlyRate * weeklyWorkHours * 52;
            }

            if (weeklyRate >= minimumWage * weeklyWorkHours && weeklyRate < maxWeeklySalary)
            {
                return weeklyRate * 52;
            }

            return null;
        }
    }
}