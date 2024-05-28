import { PostalCodeService } from '../services/postalcode.service';
import { CheckboxInfo } from '../services/checkboxinfo.service';
import { LocationService } from '../services/location.service';
import { SalaryRange, SalaryRanges } from '../services/salaryrange.service';
import { getLocationInfo } from '../../shared/constants';

export class KeyWordFilterModel {
  invalidCity: boolean;
  invalidPostal: boolean;

  isPostal(): boolean {
    return PostalCodeService.isPostalCode(this.cityOrPostal);
  }

  formatPostal(): string {
    return PostalCodeService.formatPostalCode(this.cityOrPostal);
  }

  getLocationTag(): string {
    if (this.isPostal()) {
      return getLocationInfo(this.radius, this.formatPostal());
    } else {
      return 'Location: ' + this.cityOrPostal;
    }
  }

  constructor(
    public keyword = '',
    public cityOrPostal = '',
    public searchField = 'all',
    public city = '',
    public postal = '',
    public radius = -1
  ) { }
}

export class JbDate {
  constructor(public year: number, public month: number, public day: number) { }

  static parse(date): JbDate {
    return new JbDate(
      date.Year || date.year,
      date.Month || date.month,
      date.Day || date.day);
  }
}

export class LocationField {
  constructor(
    public city = '',
    public postal = '',
    public region = '',
    public radius = -1
  ) { }

  formatPostal(): string {
    return PostalCodeService.formatPostalCode(this.postal);
  }

  getLocationTag(): string {
    return PostalCodeService.getLocationTag(
      this.region,
      this.city,
      this.postal,
      this.radius
    );
  }
}

export class SearchLocation {
  constructor(
    public City = '',
    public Postal = '',
    public Region = ''
  ) { }
}

export class LocationFilterModel {
  //radiusLabel = '';

  private static readonly iniExactLocationLabel = 'Exact Location Only';
  exactLocationLabel = LocationFilterModel.iniExactLocationLabel;

  private static readonly iniRadius = -1;
  private _radius = LocationFilterModel.iniRadius;
  get radius() {
    return this._radius;
  }
  set radius(value: number) {
    if (this._radius !== +value) this._radius = +value;

    if (
      this.locationFields &&
      this.locationFields.length === 1 &&
      this.locationFields[0].radius !== this._radius
    ) {
      this.locationFields[0].radius = this._radius;
    }
  }

  private _locationFields = new Array<LocationField>();
  get locationFields() {
    return this._locationFields;
  }
  set locationFields(value: LocationField[]) {
    this._locationFields = value;

    this.init();

    for (const locationField of this._locationFields) {
      if (locationField.region) {
        const locationId = LocationService.getLocationId(locationField.region);
        this[locationId] = true;
        //this[locationId + 'Hover'] = true;
      }
    }

    //set the radius label text based on the selected city or postal code
    if (this._locationFields && this._locationFields.length === 1) {
      if (this._radius !== +this._locationFields[0].radius) {
        this._radius = +this._locationFields[0].radius;
      }

      if (this._locationFields[0].city) {
        this.radiusLabel = this._locationFields[0].city;
        this.exactLocationLabel = 'Exact city name only';
        //this.radius = -1;
      } else if (this._locationFields[0].postal) {
        this.radiusLabel = this._locationFields[0].formatPostal();
        this.exactLocationLabel = 'Exact postal code only';
        //this.radius = 15;
      }
    }
  }

  constructor(
    public nameOrPostalCode = '',
    public tooltipText = '',
    public radiusLabel = '',

    public Cariboo = false,
    public Kootenay = false,
    public MainlandSouthwest = false,
    public Northeast = false,
    public NorthCoastNechako = false,
    public ThompsonOkanagan = false,
    public VancouverIslandCoast = false,
    // Hover states
    public CaribooHover = false,
    public KootenayHover = false,
    public MainlandSouthwestHover = false,
    public NortheastHover = false,
    public NorthCoastNechakoHover = false,
    public ThompsonOkanaganHover = false,
    public VancouverIslandCoastHover = false
  ) { }

  private init() {
    this.exactLocationLabel = LocationFilterModel.iniExactLocationLabel;
    this._radius = LocationFilterModel.iniRadius;
    this.nameOrPostalCode = '';
    this.tooltipText = '';
    this.radiusLabel = '';
    this.Cariboo = false;
    this.Kootenay = false;
    this.MainlandSouthwest = false;
    this.Northeast = false;
    this.NorthCoastNechako = false;
    this.ThompsonOkanagan = false;
    this.VancouverIslandCoast = false;
    this.CaribooHover = false;
    this.KootenayHover = false;
    this.MainlandSouthwestHover = false;
    this.NortheastHover = false;
    this.NorthCoastNechakoHover = false;
    this.ThompsonOkanaganHover = false;
    this.VancouverIslandCoastHover = false;
  }
}

export class DateFilterModel {
  startDate: JbDate;
  endDate: JbDate;

  constructor(public rangeSelected = 0, public isDisabled = true) {}
}

export class PaginationModel {
  currentPage = 1;
  totalResults = 0;
  resultsPerPage = 20;
}

export enum FilterType {
  Education,
  Industry,
  JobType
}

export class MainFilterModel {
  oldCityOrPostal = '';
  sortOrder = 11; //relevance
  scrollElement: HTMLElement;

  activeFilters: Array<string> = [];
  cities: Array<City> = [];

  locationFilter = new LocationFilterModel();
  //locationFields = new Array<LocationField>();
  get locationFields() {
    return this.locationFilter.locationFields;
  }
  set locationFields(value: LocationField[]) {
    this.locationFilter.locationFields = value;
  }

  keywordFilters = new KeyWordFilterModel();
  dateFilters = new DateFilterModel();
  pagination = new PaginationModel();
  salaryFilters = new SalaryFilterModel();
  educationFilters = new EducationFilterModel();
  industryFilters = new IndustryFilterModel();
  jobTypeFilters = new JobTypeFilterModel();
  moreFilters = new MoreFiltersFilterModel();

  get filtersWithoutKeyword() {
    return this.activeFilters.filter(
      x =>
        !x.startsWith('Keywords: ') &&
        !x.startsWith('Job title: ') &&
        !x.startsWith('Employer: ') &&
        !x.startsWith('Job number: ')
    );
  }

  get Keyword() {
    const regExp = new RegExp('-', 'g');
    return this.keywordFilters.keyword ?
      this.keywordFilters.keyword.replace(regExp, ' ') :
      this.keywordFilters.keyword;
  }

  getFilterTags(): string[] {
    return this.activeFilters;
  }

  // the text of the tag for removing the individual filter
  oldLocationLabel(radius = -1): string {
    if (!this.oldCityOrPostal.trim().length) {
      return '';
    }
    return PostalCodeService.isPostalCode(this.oldCityOrPostal)
      ? PostalCodeService.getLocationTag('', '', this.oldCityOrPostal, radius)
      : PostalCodeService.getLocationTag('', this.oldCityOrPostal, '', radius);
  }

  removeActiveFilter(label: string, startsWith = false): void {
    if (startsWith) {
      const itemsToRemove = [];
      for (let i = 0; i < this.activeFilters.length; i++) {
        if (this.activeFilters[i].indexOf(label) === 0) {
          itemsToRemove.push(this.activeFilters[i]);
        }
      }
      for (let i = 0; i < itemsToRemove.length; i++) {
        this.removeActiveFilter(itemsToRemove[i]);
      }
    } else {
      const position = this.activeFilters.indexOf(label);
      if (position !== -1) {
        this.activeFilters.splice(position, 1);
      }
    }
  }

  addActiveFilter(label: string, replaceIfStartsWith = ''): void {
    if (replaceIfStartsWith.length) {
      for (let i = 0; i < this.activeFilters.length; i++) {
        if (this.activeFilters[i].indexOf(replaceIfStartsWith) === 0) {
          this.activeFilters[i] = label;
          i = this.activeFilters.length;
        }
      }
    }

    if (this.activeFilters.indexOf(label) === -1) {
      this.activeFilters.push(label);
    }
  }

  public static getNocCode(nocCode: string): string {
    // parse the noc code to get just the numberic part
    let result = nocCode.toUpperCase();
    const split = result.startsWith('NOC ')
      ? result.split(' ')
      : ('NOC ' + result).split(' ');
    result = split.length > 1 && !isNaN(split[1] as never) ? split[1] : '00000';
    return result;
  }

  convertToElasticSearchJobSearchFilters(): ElasticSearchJobSearchFilter {
    return {
      Page: this.pagination.currentPage,
      PageSize: this.pagination.resultsPerPage,
      SalaryBracket1: this.salaryFilters.amountRange1, //Under 40,000
      SalaryBracket2: this.salaryFilters.amountRange2, //40,000 - 59,999
      SalaryBracket3: this.salaryFilters.amountRange3, //60,000 - 79,999
      SalaryBracket4: this.salaryFilters.amountRange4, //80,000 - 99,999
      SalaryBracket5: this.salaryFilters.amountRange5, //100,000+
      SalaryBracket6: this.salaryFilters.amountRange6, //custom
      SalaryMin: this.salaryFilters.minAmount, //custom min
      SalaryMax: this.salaryFilters.maxAmount, //custom max
      SearchSalaryUnknown: this.salaryFilters.searchSalaryUnknown,
      Keyword: this.Keyword,
      StartDate: this.dateFilters.startDate,
      EndDate: this.dateFilters.endDate,
      SearchInField: this.keywordFilters.searchField,
      SearchJobTypeFullTime: this.jobTypeFilters.fullTime,
      SearchJobTypePartTime: this.jobTypeFilters.partTime,
      SearchJobTypeLeadingToFullTime: this.jobTypeFilters
        .partTimeLeadingToFullTime,
      SearchJobTypePermanent: this.jobTypeFilters.permanent,
      SearchJobTypeTemporary: this.jobTypeFilters.temporary,
      SearchJobTypeCasual: this.jobTypeFilters.casual,
      SearchJobTypeSeasonal: this.jobTypeFilters.seasonal,
      SearchJobTypeDay: this.jobTypeFilters.day,
      SearchJobTypeEarly: this.jobTypeFilters.early,
      SearchJobTypeEvening: this.jobTypeFilters.evening,
      SearchJobTypeFlexible: this.jobTypeFilters.flexible,
      SearchJobTypeMorning: this.jobTypeFilters.morning,
      SearchJobTypeNight: this.jobTypeFilters.night,
      SearchJobTypeOnCall: this.jobTypeFilters.onCall,
      SearchJobTypeOvertime: this.jobTypeFilters.overtime,
      SearchJobTypeShift: this.jobTypeFilters.shift,
      SearchJobTypeTbd: this.jobTypeFilters.toBeDetermined,
      SearchJobTypeWeekend: this.jobTypeFilters.weekend,
      SearchJobTypeOnSite: this.jobTypeFilters.onSite,
      SearchJobTypeHybrid: this.jobTypeFilters.hybrid,
      SearchJobTypeTravelling: this.jobTypeFilters.travelling,
      SearchJobTypeVirtual: this.jobTypeFilters.virtual,
      SearchDateSelection: this.dateFilters.rangeSelected,
      SearchJobEducationLevel: this.educationFilters.activeFilters,
      SalaryType: this.salaryFilters.salaryEarningInterval,
      SearchLocationDistance:
        this.locationFields && this.locationFields.length === 1
          ? this.locationFields[0].radius
          : -1,
      SearchLocations: this.convertLocationFields(this.locationFields),
      SearchSalaryConditions: this.salaryFilters.salaryConditions,
      SortOrder: this.sortOrder,
      SearchIndustry: this.industryFilters.activeFilters,
      SearchIsApprentice: this.moreFilters.isApprentice,
      SearchIsVeterans: this.moreFilters.isVeterans,
      SearchIsIndigenous: this.moreFilters.isIndigenous,
      SearchIsMatureWorkers: this.moreFilters.isMatureWorkers,
      SearchIsNewcomers: this.moreFilters.isNewcomers,
      SearchIsPeopleWithDisabilities: this.moreFilters.isPeopleWithDisabilities,
      SearchIsStudents: this.moreFilters.isStudents,
      SearchIsVisibleMinority: this.moreFilters.isVisibleMinority,
      SearchIsYouth: this.moreFilters.isYouth,
      SearchIsPostingsInEnglish: this.moreFilters.isPostingsInEnglish,
      SearchIsPostingsInEnglishAndFrench: this.moreFilters
        .isPostingsInEnglishAndFrench,
      NocCode: this.moreFilters.nocCode,
      SearchNocField: MainFilterModel.getNocCode(this.moreFilters.nocCode),
      SearchJobSource: this.moreFilters.jobSource,
      SearchExcludePlacementAgencyJobs: this.moreFilters
        .excludePlacementAgencyJobs
    };
  }

  private setMoreFilter(filter: ElasticSearchJobSearchFilter) {
    this.moreFilters.nocCode = filter.NocCode
      ? filter.NocCode
      : filter.SearchNocField;

    if (this.moreFilters.isApprentice !== filter.SearchIsApprentice)
      this.moreFilters.isApprentice = filter.SearchIsApprentice;

    if (this.moreFilters.isVeterans !== filter.SearchIsVeterans)
      this.moreFilters.isVeterans = filter.SearchIsVeterans;

    if (this.moreFilters.isIndigenous !== filter.SearchIsIndigenous)
      this.moreFilters.isIndigenous = filter.SearchIsIndigenous;

    if (this.moreFilters.isMatureWorkers !== filter.SearchIsMatureWorkers)
      this.moreFilters.isMatureWorkers = filter.SearchIsMatureWorkers;

    if (this.moreFilters.isNewcomers !== filter.SearchIsNewcomers)
      this.moreFilters.isNewcomers = filter.SearchIsNewcomers;

    if (
      this.moreFilters.isPeopleWithDisabilities !==
      filter.SearchIsPeopleWithDisabilities
    )
      this.moreFilters.isPeopleWithDisabilities =
        filter.SearchIsPeopleWithDisabilities;

    if (this.moreFilters.isStudents !== filter.SearchIsStudents)
      this.moreFilters.isStudents = filter.SearchIsStudents;

    if (this.moreFilters.isVisibleMinority !== filter.SearchIsVisibleMinority)
      this.moreFilters.isVisibleMinority = filter.SearchIsVisibleMinority;

    if (this.moreFilters.isYouth !== filter.SearchIsYouth)
      this.moreFilters.isYouth = filter.SearchIsYouth;

    if (
      this.moreFilters.isPostingsInEnglish !== filter.SearchIsPostingsInEnglish
    )
      this.moreFilters.isPostingsInEnglish = filter.SearchIsPostingsInEnglish;

    if (
      this.moreFilters.isPostingsInEnglishAndFrench !==
      filter.SearchIsPostingsInEnglishAndFrench
    )
      this.moreFilters.isPostingsInEnglishAndFrench =
        filter.SearchIsPostingsInEnglishAndFrench;

    if (this.moreFilters.jobSource !== filter.SearchJobSource)
      this.moreFilters.jobSource = filter.SearchJobSource;

    if (
      this.moreFilters.excludePlacementAgencyJobs !==
      filter.SearchExcludePlacementAgencyJobs
    )
      this.moreFilters.excludePlacementAgencyJobs =
        filter.SearchExcludePlacementAgencyJobs;
  }

  private setJobTypeFilter(filter: ElasticSearchJobSearchFilter) {
    if (this.jobTypeFilters.fullTime !== filter.SearchJobTypeFullTime)
      this.jobTypeFilters.fullTime = filter.SearchJobTypeFullTime;

    if (this.jobTypeFilters.partTime !== filter.SearchJobTypePartTime)
      this.jobTypeFilters.partTime = filter.SearchJobTypePartTime;

    if (
      this.jobTypeFilters.partTimeLeadingToFullTime !==
      filter.SearchJobTypeLeadingToFullTime
    )
      this.jobTypeFilters.partTimeLeadingToFullTime =
        filter.SearchJobTypeLeadingToFullTime;

    if (this.jobTypeFilters.permanent !== filter.SearchJobTypePermanent)
      this.jobTypeFilters.permanent = filter.SearchJobTypePermanent;

    if (this.jobTypeFilters.temporary !== filter.SearchJobTypeTemporary)
      this.jobTypeFilters.temporary = filter.SearchJobTypeTemporary;

    if (this.jobTypeFilters.casual !== filter.SearchJobTypeCasual)
      this.jobTypeFilters.casual = filter.SearchJobTypeCasual;

    if (this.jobTypeFilters.seasonal !== filter.SearchJobTypeSeasonal)
      this.jobTypeFilters.seasonal = filter.SearchJobTypeSeasonal;

    if (this.jobTypeFilters.day !== filter.SearchJobTypeDay)
      this.jobTypeFilters.day = filter.SearchJobTypeDay;

    if (this.jobTypeFilters.early !== filter.SearchJobTypeEarly)
      this.jobTypeFilters.early = filter.SearchJobTypeEarly;

    if (this.jobTypeFilters.evening !== filter.SearchJobTypeEvening)
      this.jobTypeFilters.evening = filter.SearchJobTypeEvening;

    if (this.jobTypeFilters.flexible !== filter.SearchJobTypeFlexible)
      this.jobTypeFilters.flexible = filter.SearchJobTypeFlexible;

    if (this.jobTypeFilters.morning !== filter.SearchJobTypeMorning)
      this.jobTypeFilters.morning = filter.SearchJobTypeMorning;

    if (this.jobTypeFilters.night !== filter.SearchJobTypeNight)
      this.jobTypeFilters.night = filter.SearchJobTypeNight;

    if (this.jobTypeFilters.onCall !== filter.SearchJobTypeOnCall)
      this.jobTypeFilters.onCall = filter.SearchJobTypeOnCall;

    if (this.jobTypeFilters.overtime !== filter.SearchJobTypeOvertime)
      this.jobTypeFilters.overtime = filter.SearchJobTypeOvertime;

    if (this.jobTypeFilters.shift !== filter.SearchJobTypeShift)
      this.jobTypeFilters.shift = filter.SearchJobTypeShift;

    if (this.jobTypeFilters.toBeDetermined !== filter.SearchJobTypeTbd)
      this.jobTypeFilters.toBeDetermined = filter.SearchJobTypeTbd;

    if (this.jobTypeFilters.weekend !== filter.SearchJobTypeWeekend)
      this.jobTypeFilters.weekend = filter.SearchJobTypeWeekend;

    if (this.jobTypeFilters.onSite !== filter.SearchJobTypeOnSite)
      this.jobTypeFilters.onSite = filter.SearchJobTypeOnSite;

    if (this.jobTypeFilters.hybrid !== filter.SearchJobTypeHybrid)
      this.jobTypeFilters.hybrid = filter.SearchJobTypeHybrid;

    if (this.jobTypeFilters.travelling !== filter.SearchJobTypeTravelling)
      this.jobTypeFilters.travelling = filter.SearchJobTypeTravelling;

    if (this.jobTypeFilters.virtual !== filter.SearchJobTypeVirtual)
      this.jobTypeFilters.virtual = filter.SearchJobTypeTravelling;
  }

  private setDateFilter(filter: ElasticSearchJobSearchFilter) {
    if (this.dateFilters.rangeSelected !== filter.SearchDateSelection) {
      this.dateFilters.rangeSelected = filter.SearchDateSelection;
    }

    if (+this.dateFilters.rangeSelected === 3) {
      this.dateFilters.startDate = JbDate.parse(filter.StartDate);
      this.dateFilters.endDate = JbDate.parse(filter.EndDate);
      this.dateFilters.isDisabled = false;
    } else {
      this.dateFilters.isDisabled = true;
    }
  }

  private setKeywordFilter(filter: ElasticSearchJobSearchFilter) {
    if (this.keywordFilters.keyword !== filter.Keyword)
      this.keywordFilters.keyword = filter.Keyword;

    if (this.keywordFilters.searchField !== filter.SearchInField)
      this.keywordFilters.searchField = filter.SearchInField;
  }

  private setEducationFilter(filter: ElasticSearchJobSearchFilter) {
    this.educationFilters.activeFilters = filter.SearchJobEducationLevel;

    const filters = CheckboxInfo.education[0].filters;

    this.educationFilters.university = filter.SearchJobEducationLevel.indexOf(
      filters[0].label
    ) !== -1;

    this.educationFilters.secondarSchoolOrJobSpecificTraining = filter.SearchJobEducationLevel.indexOf(
      filters[1].label
    ) !== -1;

    this.educationFilters.collegeOrApprenticeship = filter.SearchJobEducationLevel.indexOf(
      filters[2].label
    ) !== -1;

    this.educationFilters.noEducationRequired = filter.SearchJobEducationLevel.indexOf(
      filters[3].label
    ) !== -1;
  }

  private hasIndustryFilter(activeFilters: number[], id: number) {
    return activeFilters.filter(x => +x === id).length === 1;
  }

  private setIndustryFilter(filter: ElasticSearchJobSearchFilter) {
    this.industryFilters.activeFilters = filter.SearchIndustry;

    this.industryFilters.accommodationAndFoodServices = this.hasIndustryFilter(
      filter.SearchIndustry,
      37
    );
    this.industryFilters.administrativeAndSupport = this.hasIndustryFilter(
      filter.SearchIndustry,
      40
    );
    this.industryFilters.agricultureAndFishing = this.hasIndustryFilter(
      filter.SearchIndustry,
      1
    );
    this.industryFilters.artsEntertainmentRecreation = this.hasIndustryFilter(
      filter.SearchIndustry,
      36
    );
    this.industryFilters.construction = this.hasIndustryFilter(
      filter.SearchIndustry,
      23
    );
    this.industryFilters.educationalServices = this.hasIndustryFilter(
      filter.SearchIndustry,
      34
    );
    this.industryFilters.employmentServices = this.hasIndustryFilter(
      filter.SearchIndustry,
      42
    );
    this.industryFilters.financeInsurance = this.hasIndustryFilter(
      filter.SearchIndustry,
      29
    );
    this.industryFilters.healthCareAndSocialAssistance = this.hasIndustryFilter(
      filter.SearchIndustry,
      35
    );
    this.industryFilters.informationAndCulturalIndustries = this.hasIndustryFilter(
      filter.SearchIndustry,
      28
    );
    this.industryFilters.managementOfCompaniesAndEnterprises = this.hasIndustryFilter(
      filter.SearchIndustry,
      32
    );
    this.industryFilters.manufacturing = this.hasIndustryFilter(
      filter.SearchIndustry,
      24
    );
    this.industryFilters.miningAndOilAndGasExtraction = this.hasIndustryFilter(
      filter.SearchIndustry,
      21
    );
    this.industryFilters.personalAndLaundry = this.hasIndustryFilter(
      filter.SearchIndustry,
      44
    );
    this.industryFilters.privateHouseholds = this.hasIndustryFilter(
      filter.SearchIndustry,
      46
    );
    this.industryFilters.professionalScientificAndTechnicalServices = this.hasIndustryFilter(
      filter.SearchIndustry,
      31
    );
    this.industryFilters.publicAdministration = this.hasIndustryFilter(
      filter.SearchIndustry,
      39
    );
    this.industryFilters.realEstateAndRental = this.hasIndustryFilter(
      filter.SearchIndustry,
      30
    );
    this.industryFilters.religiousGrantMaking = this.hasIndustryFilter(
      filter.SearchIndustry,
      45
    );
    this.industryFilters.repairAndMaintenance = this.hasIndustryFilter(
      filter.SearchIndustry,
      43
    );
    this.industryFilters.retailTrade = this.hasIndustryFilter(
      filter.SearchIndustry,
      26
    );
    this.industryFilters.transportationAndWarehousing = this.hasIndustryFilter(
      filter.SearchIndustry,
      27
    );
    this.industryFilters.utilities = this.hasIndustryFilter(
      filter.SearchIndustry,
      22
    );
    this.industryFilters.wasteManagement = this.hasIndustryFilter(
      filter.SearchIndustry,
      41
    );
    this.industryFilters.wholesaleTrade = this.hasIndustryFilter(
      filter.SearchIndustry,
      25
    );
  }

  private setSalaryFilter(filter: ElasticSearchJobSearchFilter): void {
    if (this.salaryFilters.amountRange1 !== filter.SalaryBracket1)
      this.salaryFilters.amountRange1 = filter.SalaryBracket1;

    if (this.salaryFilters.amountRange2 !== filter.SalaryBracket2)
      this.salaryFilters.amountRange2 = filter.SalaryBracket2;

    if (this.salaryFilters.amountRange3 !== filter.SalaryBracket3)
      this.salaryFilters.amountRange3 = filter.SalaryBracket3;

    if (this.salaryFilters.amountRange4 !== filter.SalaryBracket4)
      this.salaryFilters.amountRange4 = filter.SalaryBracket4;

    if (this.salaryFilters.amountRange5 !== filter.SalaryBracket5)
      this.salaryFilters.amountRange5 = filter.SalaryBracket5;

    if (this.salaryFilters.amountRange6 !== filter.SalaryBracket6)
      this.salaryFilters.amountRange6 = filter.SalaryBracket6;

    if (this.salaryFilters.minAmount !== filter.SalaryMin)
      this.salaryFilters.minAmount = filter.SalaryMin;

    if (this.salaryFilters.maxAmount !== filter.SalaryMax)
      this.salaryFilters.maxAmount = filter.SalaryMax;

    if (this.salaryFilters.searchSalaryUnknown !== filter.SearchSalaryUnknown)
      this.salaryFilters.searchSalaryUnknown = filter.SearchSalaryUnknown;

    if (this.salaryFilters.salaryEarningInterval !== filter.SalaryType)
      this.salaryFilters.salaryEarningInterval = filter.SalaryType;

    this.salaryFilters.salaryConditions = filter.SearchSalaryConditions ?? [];
  }

  private setLocationFilter(filter: ElasticSearchJobSearchFilter) {
    const locationFields: LocationField[] = [];
    for (const locationField of filter.SearchLocations) {
      locationFields.push(
        new LocationField(
          locationField.City,
          locationField.Postal,
          locationField.Region,
          filter.SearchLocationDistance
        )
      );
    }
    this.locationFields = locationFields;
  }

  private convertLocationFields(locationFields: LocationField[]): SearchLocation[] {
    const searchLocations: SearchLocation[] = [];
    for (const locationField of locationFields) {
      searchLocations.push(
        new SearchLocation(
          locationField.city,
          locationField.postal,
          locationField.region
        )
      );
    }
    return searchLocations;
  }

  setMainFilterModel(filter: ElasticSearchJobSearchFilter): void {
    if (filter) {
      this.setLocationFilter(filter);
      this.setIndustryFilter(filter);
      this.setEducationFilter(filter);
      this.setSalaryFilter(filter);
      this.setKeywordFilter(filter);
      this.setDateFilter(filter);
      this.setJobTypeFilter(filter);
      this.setMoreFilter(filter);
    }
  }
}

export interface ElasticSearchJobSearchFilter {
  Page: number;
  PageSize: number;
  SalaryBracket1: boolean;
  SalaryBracket2: boolean;
  SalaryBracket3: boolean;
  SalaryBracket4: boolean;
  SalaryBracket5: boolean;
  SalaryBracket6: boolean;
  SalaryMin: string;
  SalaryMax: string;
  SearchSalaryUnknown: boolean;
  Keyword: string;
  StartDate: JbDate;
  EndDate: JbDate;
  SearchInField: string;
  SearchJobTypeFullTime: boolean;
  SearchJobTypePartTime: boolean;
  SearchJobTypeLeadingToFullTime: boolean;
  SearchJobTypePermanent: boolean;
  SearchJobTypeTemporary: boolean;
  SearchJobTypeCasual: boolean;
  SearchJobTypeSeasonal: boolean;
  SearchJobTypeDay: boolean;
  SearchJobTypeEarly: boolean;
  SearchJobTypeEvening: boolean;
  SearchJobTypeFlexible: boolean;
  SearchJobTypeMorning: boolean;
  SearchJobTypeNight: boolean;
  SearchJobTypeOnCall: boolean;
  SearchJobTypeOvertime: boolean;
  SearchJobTypeShift: boolean;
  SearchJobTypeTbd: boolean;
  SearchJobTypeWeekend: boolean;
  SearchJobTypeOnSite: boolean;
  SearchJobTypeHybrid: boolean;
  SearchJobTypeTravelling: boolean;
  SearchJobTypeVirtual: boolean;
  SearchDateSelection: number;
  SearchJobEducationLevel: string[];
  SalaryType: number;
  SearchLocationDistance: number;
  SearchLocations: SearchLocation[];
  SearchSalaryConditions: string[];
  SortOrder: number;
  SearchIndustry: number[];
  SearchIsApprentice: boolean;
  SearchIsVeterans: boolean;
  SearchIsIndigenous: boolean;
  SearchIsMatureWorkers: boolean;
  SearchIsNewcomers: boolean;
  SearchIsPeopleWithDisabilities: boolean;
  SearchIsStudents: boolean;
  SearchIsVisibleMinority: boolean;
  SearchIsYouth: boolean;
  SearchIsPostingsInEnglish: boolean;
  SearchIsPostingsInEnglishAndFrench: boolean;
  NocCode: string;
  SearchNocField: string;
  SearchJobSource: string;
  SearchExcludePlacementAgencyJobs: boolean;
}

export class SalaryFilterModel {
  salaryEarningInterval = 4;
  asPerCollectiveAgreement = false;
  bonus = false;
  disabilityBenefits = false;
  gratuities = false;
  medicalBenefits = false;
  mileagePaid = false;
  pieceWork = false;
  respBenefits = false;
  commission = false;
  dentalBenefits = false;
  groupInsuranceBenefits = false;
  lifeInsuranceBenefits = false;
  pensionPlanBenefits = false;
  rrspBenefits = false;
  otherBenefits = false;
  visionCareBenefits = false;

  //_salaryConditions: Array<string> = [];
  get salaryConditions(): string[] {
    const result: string[] = [];

    const filters = CheckboxInfo.salary[0].filters;
    if (this.asPerCollectiveAgreement) result.push(filters.find(x => x.id === '1').label);
    if (this.bonus) result.push(filters.find(x => x.id === '2').label);
    if (this.commission) result.push(filters.find(x => x.id === '3').label);
    if (this.dentalBenefits) result.push(filters.find(x => x.id === '4').label);
    if (this.disabilityBenefits) result.push(filters.find(x => x.id === '5').label);
    if (this.gratuities) result.push(filters.find(x => x.id === '6').label);
    if (this.groupInsuranceBenefits) result.push(filters.find(x => x.id === '7').label);
    if (this.lifeInsuranceBenefits) result.push(filters.find(x => x.id === '8').label);
    if (this.medicalBenefits) result.push(filters.find(x => x.id === '9').label);
    if (this.mileagePaid) result.push(filters.find(x => x.id === '10').label);
    if (this.pensionPlanBenefits) result.push(filters.find(x => x.id === '11').label);
    if (this.pieceWork) result.push(filters.find(x => x.id === '12').label);
    if (this.respBenefits) result.push(filters.find(x => x.id === '13').label);
    if (this.rrspBenefits) result.push(filters.find(x => x.id === '14').label);
    if (this.visionCareBenefits) result.push(filters.find(x => x.id === '15').label);
    if (this.otherBenefits) result.push(filters.find(x => x.id === '16').label);

    return result;
    //return this._salaryConditions;
  }
  set salaryConditions(value: string[]) {
    //this._salaryConditions = value;
    const filters = CheckboxInfo.salary[0].filters;
    this.asPerCollectiveAgreement = value.indexOf(filters.find(x => x.id === '1').label) !== -1;
    this.bonus = value.indexOf(filters.find(x => x.id === '2').label) !== -1;
    this.commission = value.indexOf(filters.find(x => x.id === '3').label) !== -1;
    this.dentalBenefits = value.indexOf(filters.find(x => x.id === '4').label) !== -1;
    this.disabilityBenefits = value.indexOf(filters.find(x => x.id === '5').label) !== -1;
    this.gratuities = value.indexOf(filters.find(x => x.id === '6').label) !== -1;
    this.groupInsuranceBenefits = value.indexOf(filters.find(x => x.id === '7').label) !== -1;
    this.lifeInsuranceBenefits = value.indexOf(filters.find(x => x.id === '8').label) !== -1;
    this.medicalBenefits = value.indexOf(filters.find(x => x.id === '9').label) !== -1;
    this.mileagePaid = value.indexOf(filters.find(x => x.id === '10').label) !== -1;
    this.pensionPlanBenefits = value.indexOf(filters.find(x => x.id === '11').label) !== -1;
    this.pieceWork = value.indexOf(filters.find(x => x.id === '12').label) !== -1;
    this.respBenefits = value.indexOf(filters.find(x => x.id === '13').label) !== -1;
    this.rrspBenefits = value.indexOf(filters.find(x => x.id === '14').label) !== -1;
    this.visionCareBenefits = value.indexOf(filters.find(x => x.id === '15').label) !== -1;
    this.otherBenefits = value.indexOf(filters.find(x => x.id === '16').label) !== -1;
  }

  amountRange1: boolean;
  amountRange2: boolean;
  amountRange3: boolean;
  amountRange4: boolean;
  amountRange5: boolean;
  amountRange6: boolean;

  get amountRange1Text(): string {
    return this.salaryRanges.range1;
  }
  get amountRange2Text(): string {
    return this.salaryRanges.range2;
  }
  get amountRange3Text(): string {
    return this.salaryRanges.range3;
  }
  get amountRange4Text(): string {
    return this.salaryRanges.range4;
  }
  get amountRange5Text(): string {
    return this.salaryRanges.range5;
  }
  get salaryIntervalText(): string {
    return this.salaryRanges.intervalText;
  }
  get amountRange6Text(): string {
    return 'Custom range ' + this.salaryIntervalText;
  }

  private get salaryRanges(): SalaryRanges {
    let result: SalaryRanges;
    switch (+this.salaryEarningInterval) {
      case 0:
        result = SalaryRange.hourly;
        break;
      case 1:
        result = SalaryRange.weekly;
        break;
      case 2:
        result = SalaryRange.biweekly;
        break;
      case 3:
        result = SalaryRange.monthly;
        break;
      case 4:
        result = SalaryRange.annually;
        break;
    }
    return result;
  }

  minAmount = '';
  maxAmount = '';
  searchSalaryUnknown: boolean;
}

export class EducationFilterModel {
  university: boolean;
  collegeOrApprenticeship: boolean;
  secondarSchoolOrJobSpecificTraining: boolean;
  noEducationRequired: boolean;

  activeFilters: Array<string> = [];
}

export class IndustryFilterModel {
  accommodationAndFoodServices: boolean;
  administrativeAndSupport: boolean;
  agricultureAndFishing: boolean;
  artsEntertainmentRecreation: boolean;
  construction: boolean;
  educationalServices: boolean;
  employmentServices: boolean;
  financeInsurance: boolean;
  healthCareAndSocialAssistance: boolean;
  informationAndCulturalIndustries: boolean;
  managementOfCompaniesAndEnterprises: boolean;
  manufacturing: boolean;
  miningAndOilAndGasExtraction: boolean;
  personalAndLaundry: boolean;
  privateHouseholds: boolean;
  professionalScientificAndTechnicalServices: boolean;
  publicAdministration: boolean;
  realEstateAndRental: boolean;
  religiousGrantMaking: boolean;
  repairAndMaintenance: boolean;
  retailTrade: boolean;
  transportationAndWarehousing: boolean;
  utilities: boolean;
  wasteManagement: boolean;
  wholesaleTrade: boolean;

  activeFilters: Array<number> = [];
}

export class JobTypeFilterModel {
  fullTime = false;
  partTime = false;
  partTimeLeadingToFullTime = false;
  temporary = false;
  permanent = false;
  casual = false;
  seasonal = false;
  early = false;
  day = false;
  night = false;
  morning = false;
  evening = false;
  flexible = false;
  onCall = false;
  overtime = false;
  shift = false;
  toBeDetermined = false;
  weekend = false;
  onSite = false;
  hybrid = false;
  travelling = false;
  virtual = false;
}

export class City {
  key: string; // location ID
  value: string; //location Name
}

export class MoreFiltersFilterModel {
  isApprentice = false;
  isIndigenous = false;
  isMatureWorkers = false;
  isNewcomers = false;
  isPeopleWithDisabilities = false;
  isStudents = false;
  isVeterans = false;
  isVisibleMinority = false;
  isYouth = false;
  isPostingsInEnglish = true;
  isPostingsInEnglishAndFrench = false;
  nocCode = '';
  jobSource = '0';
  excludePlacementAgencyJobs = false;
}
