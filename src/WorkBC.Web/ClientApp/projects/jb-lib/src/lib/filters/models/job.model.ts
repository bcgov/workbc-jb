import {
  RecommendationFilterVm,
  JobSeekerFlags
} from '../../models/recommendation-filter.model';

export class RecommendedJobResult<T> {
  result: T[];
  count: number;
  pageNumber: number;
  pageSize: number;
  city: string;
  jobSeekerFlags: JobSeekerFlags;
}

export class RecommendedJobFilter {
  filterSavedJobTitles = false;
  filterSavedJobNocs = false;
  filterSavedJobEmployers = false;
  filterJobSeekerCity = false;
  filterIsApprentice = false;
  filterIsIndigenous = false;
  filterIsMatureWorkers = false;
  filterIsNewcomers = false;
  filterIsPeopleWithDisabilities = false;
  filterIsStudents = false;
  filterIsVeterans = false;
  filterIsVisibleMinority = false;
  filterIsYouth = false;

  constructor(
    public page = 1,
    public pageSize = 20,
    public sortOrder = 11,
    vm?: RecommendationFilterVm,
    recommendedJobFilter?
  ) {
    if (vm) {
      this.updateReasons(vm);
    }
    if (recommendedJobFilter) {
      const f = recommendedJobFilter;
      this.filterSavedJobTitles = f.filterSavedJobTitles;
      this.filterSavedJobNocs = f.filterSavedJobNocs;
      this.filterSavedJobEmployers = f.filterSavedJobEmployers;
      this.filterJobSeekerCity = f.filterJobSeekerCity;
      this.filterIsApprentice = f.filterIsApprentice;
      this.filterIsIndigenous = f.filterIsIndigenous;
      this.filterIsMatureWorkers = f.filterIsMatureWorkers;
      this.filterIsNewcomers = f.filterIsNewcomers;
      this.filterIsPeopleWithDisabilities = f.filterIsPeopleWithDisabilities;
      this.filterIsStudents = f.filterIsStudents;
      this.filterIsVeterans = f.filterIsVeterans;
      this.filterIsVisibleMinority = f.filterIsVisibleMinority;
      this.filterIsYouth = f.filterIsYouth;
    }
  }

  updateReasons(vm: RecommendationFilterVm): void {
    this.filterSavedJobTitles = vm.hasSameJobTitle;
    this.filterSavedJobNocs = vm.hasSameNoc;
    this.filterSavedJobEmployers = vm.hasSameEmployer;
    this.filterJobSeekerCity = vm.inTheSameCity;
    this.filterIsApprentice = vm.isApprentice;
    this.filterIsIndigenous = vm.isIndigenous;
    this.filterIsMatureWorkers = vm.isMatureWorker;
    this.filterIsNewcomers = vm.isNewcomer;
    this.filterIsPeopleWithDisabilities = vm.hasDisability;
    this.filterIsStudents = vm.isStudent;
    this.filterIsVeterans = vm.isVeteran;
    this.filterIsVisibleMinority = vm.isMinority;
    this.filterIsYouth = vm.isYouth;
  }

  get filterCount(): number {
    const properties = Object.getOwnPropertyNames(this);
    let count = 0;
    for (const name of properties) {
      if (name.startsWith('filter') &&
        Object.prototype.hasOwnProperty.call(this, name) &&
        this[name] === true) {
        count++;
      }
    }
    return count;
  }
}

export class Results<T> extends RecommendedJobResult<T> {
  //skills: string[];
  textHeaders: TextHeader;
}

export interface SavedJob {
  city: string;
  datePosted: Date;
  expireDate: Date;
  employerName: string;
  jobId: number;
  salary: number;
  salarySummary: string;
  title: string;
  hoursOfWork: string[];
  periodOfEmployment: string[];
  lastUpdated: Date;
  positionsAvailable: number;
  views: number;
  source: number;
  externalSourceName: string;
  externalSourceUrl: string;
  id: number;
  note: string;
  isActive: boolean;
  isNew: boolean;
}

export class HoursOfWorkCls {
  Description: string[];

  constructor(hoursOfWork: string[]) {
    this.Description = hoursOfWork;
  }
}

export class PeriodOfEmploymentCls {
  Description: string[];

  constructor(periodOfEmployment: string[]) {
    this.Description = periodOfEmployment;
  }
}

export class ExternalSourceCls {
  Source: Array<ExternalSource>;

  constructor(url: string, source: string) {
    this.Source = [{ Url: url, Source: source }];
  }
}

export class JobBase {
  JobId: string;
  Title: string;
  DatePosted: string;
  ExpireDate: string;
  EmployerName: string;
  City: string;
  HoursOfWork: HoursOfWorkCls;
  PeriodOfEmployment: PeriodOfEmploymentCls;
  IsFederalJob: boolean;
  LastUpdated: string;
  SalarySummary: string;
  Views: number;
  ExternalSource: ExternalSourceCls;
  IsNew: boolean;
}

export class RecommendedJob extends JobBase {
  Noc: string;
  Noc2021: string;
  Reason: string;
  Score: number;
}

export class Job extends RecommendedJob {
  constructor(
    public savedJob: SavedJob,
    job?: JobBase | RecommendedJob,
    jobDetail?: Job
  ) {
    super();
    if (savedJob) {
      this.City = savedJob.city;
      this.DatePosted = savedJob.datePosted.toString();
      this.ExpireDate = savedJob.expireDate.toString();
      this.EmployerName = savedJob.employerName;
      this.JobId = savedJob.jobId.toString();
      this.SalarySummary = savedJob.salarySummary;
      this.Title = savedJob.title;
      this.HoursOfWork = new HoursOfWorkCls(savedJob.hoursOfWork);
      this.PeriodOfEmployment = new PeriodOfEmploymentCls(
        savedJob.periodOfEmployment
      );
      this.LastUpdated = savedJob.lastUpdated.toString();
      this.PositionsAvailable = savedJob.positionsAvailable;
      this.Views = savedJob.views;
      this.Note = savedJob.note ? savedJob.note : '';
      this.IsActive = savedJob.isActive;
      this.IsFederalJob = savedJob.source === 1;
      this.IsNew = savedJob.isNew;

      if (savedJob.salary) {
        this.Salary = savedJob.salary.toString();
      }

      if (savedJob.externalSourceUrl && savedJob.externalSourceName) {
        this.ExternalSource = new ExternalSourceCls(
          savedJob.externalSourceUrl,
          savedJob.externalSourceName
        );
      }
    }

    if (job) {
      this.JobId = job.JobId;
      this.Title = job.Title;
      this.DatePosted = job.DatePosted;
      this.ExpireDate = job.ExpireDate;
      this.EmployerName = job.EmployerName;
      this.City = job.City;
      this.HoursOfWork = job.HoursOfWork;
      this.PeriodOfEmployment = job.PeriodOfEmployment;
      this.IsFederalJob = job.IsFederalJob;
      this.LastUpdated = job.LastUpdated;
      this.SalarySummary = job.SalarySummary;
      this.Views = job.Views;
      this.ExternalSource = job.ExternalSource;
      this.IsNew = job.IsNew;

      if ('Reason' in job) {
        this.Noc = job.Noc;
        this.Noc2021 = job.Noc2021;
        this.Reason = job.Reason;
        this.Score = job.Score;
      }
    }

    // if a jobDetail was passed in, then clone the entire object
    // This is a workaround for not being able to overload constructors
    // in TypeScript (i.e. this class has no empty constructor)
    if (jobDetail) {
      const j = jobDetail;
      for (const property in j) {
        this[property] = j[property];
      }
    }
  }

  Note: string;
  IsActive: boolean;
  Description: string;
  EmployerTypeId: string;
  Function: string;
  Industry: string;
  IsAboriginal: string;
  IsApprentice: string;
  IsDisability: string;
  IsNewcomer: string;
  IsStudent: string;
  IsVeteran: string;
  IsVismin: string;
  IsYouth: string;
  IsVariousLocation: string;
  Lang: string;
  WorkLangCd: JobLanguageCls;
  Occupation: string;
  PostalCode: string;
  Province: string;
  Salary: string;
  SalaryDescription: string;
  WageClass: string;
  WorkHours: string;
  WorkplaceType: WorkplaceTypeCls;
  EmploymentTerms: EmploymentTermsCls;
  Location: LocationCls;
  SkillCategories: SkillCategoriesCls;

  get Labels(): string[] {
    const result: string[] = [];

    //Only add "NEW" for jobs posted in the past three days
    if (this.IsNew) {
      //Add label
      result.push(!this.Lang || this.Lang === 'en' ? 'New' : 'Nouveau');
    }

    return result;
  }

  //ExpiresIn: returns an integer that represents the number
  // of days between now and the job expiry date;
  get ExpiresIn(): number {
    const oneDay = 86400000;
    let result = -1;
    if (this.IsFederalJob) {
      const dateNow = new Date();
      dateNow.setHours(0, 0, 0, 0);
      const expireDate = new Date(this.ExpireDate);
      expireDate.setHours(23, 59, 59, 0);
      const diffDays = Math.round((expireDate.getTime() - dateNow.getTime()) / oneDay);
      result = diffDays;
    }
    return result;
  }

  PositionsAvailable: number;
  ApplyEmailAddress: string;
  ApplyPhoneNumber: string;
  ApplyPhoneNumberExt: string;
  ApplyFaxNumber: string;
  ApplyPhoneNumberTimeFrom: string;
  ApplyPhoneNumberTimeTo: string;
  ApplyWebsite: string;
  ApplyAddress: string;
  NocGroup: string;
  SalaryConditions: SalaryConditionCls;
  ProgramName: string;
  ProgramDescription: string;
  ApplyPersonStreet: string;
  ApplyPersonRoom: string;
  ApplyPersonCity: string;
  ApplyPersonProvince: string;
  ApplyPersonPostalCode: string;
  ApplyPersonTimeFrom: string;
  ApplyPersonTimeTo: string;
  ApplyMailStreet: string;
  ApplyMailRoom: string;
  ApplyMailCity: string;
  ApplyMailProvince: string;
  ApplyMailPostalCode: string;
  ExternalSource: ExternalSourceCls;
  StartDate: string;
}

export class ExternalSource {
  Url: string;
  Source: string;
}

export class EmploymentTermsCls {
  Description: string[];
}

export class SalaryConditionCls {
  Description: string[];
}

export class LocationCls {
  Lat: string;
  Lon: string;
}

export class SkillCategoriesCls {
  Category: CategoryCls;
  SkillCount: number;
  Skills: Array<string>;
}

export class CategoryCls {
  Id: number;
  Name: string;
}

export class GoogleMapData {
  longitude: number;
  latitude: number;
  jobId: string;
  numberOfJobs: number;
}

export class MapDataCls {
  uniqueJobCount: number;
  results: Array<GoogleMapData>;
}

export class LocationInformation {
  jobTitle: string;
  expire: string;
  datePosted: string;
  salary: string;
  company: string;
  city: string;
  hoursOfWork: string;
  periodOfEmployment: string;
  jobId: number;
  isFederalJob: boolean;
  jobSource: string;
  externalUrl: string;
}

export class WorkplaceTypeCls {
  Id: number;
  Description: string;
}

export class JobLanguageCls {
  Description: string[];
}
export class TextHeader {
  jobPosting: string;
  salary: string;
  education: string;
  jobType: string;
  language: string;
  english: string;
  positionsAvailable: string;
  nocGroup: string;
  expiresIn: string;
  days: string;
  expires: string;
  posted: string;
  lastUpdated: string;
  views: string;
  jobNumber: string;
  save: string;
  print: string;
  share: string;
  jobRequirements: string;
  jobLocations: string;
  experience: string;
  credentials: string;
  additionalSkills: string;
  workSetting: string;
  specificSkills: string;
  securitySafety: string;
  workSiteEnvironment: string;
  workLocationInformation: string;
  personalSuitability: string;
  applyNow: string;
  online: string;
  byEmail: string;
  byFax: string;
  byPhone: string;
  benefits: string;
  byMail: string;
  startDate: string;
  asSoonAsPossible: string;
  inPerson: string;
  workSchedule: string;
  workplaceType: string;
}

export interface NocCode {
  nocCode: string;
  name: string;
}
