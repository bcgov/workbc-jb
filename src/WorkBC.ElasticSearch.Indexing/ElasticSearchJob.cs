using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WorkBC.ElasticSearch.Indexing
{
    public class ElasticSearchJob
    {
        //Shared
        public string JobId { get; set; }
        public string Title { get; set; }
        public DateTime DatePosted { get; set; }

        [JsonIgnore]
        public DateTime ActualDatePosted { get; set; }

        public string EduLevel { get; set; }
        public string EmployerName { get; set; }
        public string Lang { get; set; }
        public Location[] Location { get; set; }
        public string[] City { get; set; } 
        public string Province { get; set; }
        public string[] Region { get; set; }
        public HoursOfWork HoursOfWork { get; set; }
        public PeriodOfEmployment PeriodOfEmployment { get; set; }
        public EmploymentTerms EmploymentTerms { get; set; }
        public string[] LocationGeo { get; set; }

        //Federal
        public int EmployerTypeId { get; set; }
        public string WageClass { get; set; }
        public string PostalCode { get; set; }
        public decimal WorkHours { get; set; }
        public decimal? Salary { get; set; }
        public bool IsStudent { get; set; }
        public bool IsYouth { get; set; }
        public bool IsApprentice { get; set; }
        public bool IsDisability { get; set; }
        public bool IsAboriginal { get; set; }
        public bool IsNewcomer { get; set; }
        public bool IsVeteran { get; set; }
        public bool IsVismin { get; set; }
        public bool IsFederalJob { get; set; }
        public bool IsMatureWorker { get; set; }
        public List<SkillCategory> SkillCategories { get; set; }
        public SalaryConditions SalaryConditions { get; set; }
        public WorkplaceType WorkplaceType { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? Noc { get; set; }
        public string NocGroup { get; set; }
        public string NocJobTitle { get; set; }
        public bool IsVariousLocation { get; set; }
        public string SalaryDescription { get; set; }
        public string SalarySummary { get; set; }
        public string ProgramName { get; set; }
        public string ProgramDescription { get; set; }

        public int? NaicsId { get; set; }

        public DateTime? StartDate { get; set; }

        public SalarySort SalarySort { get; set; }

        //Wanted
        public string Industry { get; set; }
        public string JobDescription { get; set; }
        public string Occupation { get; set; }
        public string Function { get; set; }
        public ExternalJobSource ExternalSource { get; set; }


        //Extra fields for job-details page
        public DateTime? LastUpdated { get; set; }
        public int PositionsAvailable { get; set; }
        public string ApplyEmailAddress { get; set; }
        public string ApplyFaxNumber { get; set; }
        public string ApplyPhoneNumber { get; set; }
        public string ApplyPhoneNumberExt { get; set; }
        public string ApplyPhoneNumberTimeFrom { get; set; }
        public string ApplyPhoneNumberTimeTo { get; set; }
        public string ApplyWebsite { get; set; }

        public string ApplyPersonStreet { get; set; }
        public string ApplyPersonRoom { get; set; }
        public string ApplyPersonCity { get; set; }
        public string ApplyPersonProvince { get; set; }
        public string ApplyPersonPostalCode { get; set; }
        public string ApplyPersonTimeFrom { get; set; }
        public string ApplyPersonTimeTo { get; set; }

        public string ApplyMailStreet { get; set; }
        public string ApplyMailRoom { get; set; }
        public string ApplyMailCity { get; set; }
        public string ApplyMailProvince { get; set; }
        public string ApplyMailPostalCode { get; set; }
    }

    //Federal only
    public class SkillCategory
    {
        public Category Category { get; set; }
        public int SkillCount { get; set; }
        public List<string> Skills { get; set; }
    }

    //Federal only
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SalaryConditions
    {
        public SalaryConditions()
        {
            Description = new List<string>();
        }

        public List<string> Description { get; set; }
    }

    //Federal only
    public class WorkplaceType
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    //Shared
    public class Location
    {
        public string Lat { get; set; }
        public string Lon { get; set; }

        public static Location LocationOrNull(string lat, string lon)
        {
            decimal dLat;
            decimal dLon;

            if (!(decimal.TryParse(lat, out dLat) && decimal.TryParse(lon, out dLon)))
            {
                return null;
            }

            // do a rough check to make sure the location is in BC
            if (dLat is > 48.2m and < 60m && dLon is < -114m and > -139m)
            {
                return new Location
                {
                    Lat = lat,
                    Lon = lon
                };
            }

            return null;
        }
    }

    public class HoursOfWork
    {
        public List<string> Description { get; set; }
    }
    public class PeriodOfEmployment
    {
        public List<string> Description { get; set; }
    }
    public class EmploymentTerms
    {
        public List<string> Description { get; set; }
    }

    public class ExternalJobSource
    {
        public List<ExternalSource> Source { get; set; }
    }

    public class SalarySort
    {
        public decimal Ascending { get; set; }
        public decimal Descending { get; set; }
    }

    public class ExternalSource
    {
        public string Url { get; set; }
        public string Source { get; set; }
    }
}
