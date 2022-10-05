using System;

namespace WorkBC.Data.Model.Enterprise
{
    public partial class CareerProfile
    {
        public int CareerProfileId { get; set; }
        public int NocId { get; set; }
        public string MinimumEducation { get; set; }
        public string CareerSummary { get; set; }
        public string MainDuties { get; set; }
        public string SpecialDuties { get; set; }
        public string WorkEnvironment { get; set; }
        public string JobRequirements { get; set; }
        public string Workforce { get; set; }
        public double? RegionalEmploymentVithis { get; set; }
        public double? RegionalEmploymentViall { get; set; }
        public double? RegionalEmploymentMswthis { get; set; }
        public double? RegionalEmploymentMswall { get; set; }
        public double? RegionalEmploymentTokthis { get; set; }
        public double? RegionalEmploymentTokall { get; set; }
        public double? RegionalEmploymentKoothis { get; set; }
        public double? RegionalEmploymentKooall { get; set; }
        public double? RegionalEmploymentCarthis { get; set; }
        public double? RegionalEmploymentCarall { get; set; }
        public double? RegionalEmploymentNcnthis { get; set; }
        public double? RegionalEmploymentNcnall { get; set; }
        public string EmploymentOutlook { get; set; }
        public string CareerOutlookTitle { get; set; }
        public string CareerOutlookSummary { get; set; }
        public string RegionOutlookSummaryMsw { get; set; }
        public string RegionOutlookSummaryVi { get; set; }
        public string RegionOutlookSummaryTok { get; set; }
        public string RegionOutlookSummaryKoo { get; set; }
        public string RegionOutlookSummaryCar { get; set; }
        public string RegionOutlookSummaryNcn { get; set; }
        public string RegionOutlookSummaryNe { get; set; }
        public string CareerPath { get; set; }
        public string CareerTrekDescription { get; set; }
        public string CareerTrekId { get; set; }
        public string CareerTrekImageUrl { get; set; }
        public string CareerTrekImageAltText { get; set; }
        public string CareerTrekSource { get; set; }
        public byte[] CareerImage { get; set; }
        public bool? IsFeatured { get; set; }
        public string FeaturedDescription { get; set; }
        public byte[] CareerImageHeadShot { get; set; }
        public string DayInTheLifeOf { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? HighOpportunityNocgroupNocId { get; set; }
        public int? EmploymentSizeRating { get; set; }
        public int? ProjectedUnemploymentRating { get; set; }
        public int? ProjectedJobOpeningsRating { get; set; }
        public int? EducationRequired { get; set; }
        public bool? IsFeaturedCategory { get; set; }
        public string FeaturedCategoryDescription { get; set; }
        public string EarningsExtended { get; set; }
        public bool IsTopOccupation { get; set; }
        public string CareerProfileDescription { get; set; }
        public int? TopOccupationSkillLevelRank { get; set; }
        public bool IsTop10Lng { get; set; }
        public bool IsTop50Occupation { get; set; }
        public bool IsDesignatedTrade { get; set; }
        public string TradesTrainingResourcesNote { get; set; }
        public byte? StatusId { get; set; }
        public string CareerFullImageAltTag { get; set; }
        public int? SiteId { get; set; }
        public string SkillsAndAttributes { get; set; }
        public string EngLangRequire { get; set; }
        public short? CareerLicensingId { get; set; }
        public string BecomingQualifiedHeader { get; set; }
        public string TotalApproximateFees { get; set; }
        public string EstimatedTime { get; set; }
        public string BecomingQualifiedContent { get; set; }
        public string RelatedCareersIntro { get; set; }
        public int? RelatedCareersSites { get; set; }
        public string CareerIntro { get; set; }
        public bool IsTopHealthOccupation { get; set; }
        public string AddResourcesNotation { get; set; }
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
        public string PageTitle { get; set; }
        public string PageDescription { get; set; }
        public string PageKeywords { get; set; }
        public string HighDemandTitle { get; set; }
        public string AnnualSalarySource { get; set; }
        public string WorkRelatedSkillsIntro { get; set; }

        public virtual Noc Noc { get; set; }
    }
}