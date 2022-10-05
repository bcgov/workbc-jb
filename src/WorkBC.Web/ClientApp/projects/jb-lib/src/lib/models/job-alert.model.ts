import { CheckboxInfo } from '../filters/services/checkboxinfo.service';
import { PostalCodeService } from '../filters/services/postalcode.service';
import { LocationService } from '../filters/services/location.service';
import { JobService } from '../services/job.service';
import { SalaryRange } from '../filters/services/salaryrange.service';
import { MainFilterModel, ElasticSearchJobSearchFilter } from '../filters/models/filter.model';
import { getLocationInfo } from '../shared/constants';

export class JobAlertModel {
  matchedJobs = -1;
  showMore = false;
  overwriteExisting = false;
  keyword = '';
  searchField = 'all';

  constructor(
    public title = '',
    public alertFrequency = 1,
    public urlParameters = '',
    public jobSearchFilters = JSON.stringify(
      new MainFilterModel().convertToElasticSearchJobSearchFilters()
    ),
    public id?: number,
    private jobService: JobService = null
  ) {
    if (this.jobService) {
      this.jobService
        .getTotalMatchedJobs(jobSearchFilters)
        .subscribe(result => (this.matchedJobs = result));
    }

    this.setKeyword();
  }

  get totalMatchedJobs(): string {
    const s = this.matchedJobs > 1 ? 's' : '';
    return `${this.matchedJobs.toLocaleString('en')} job${s} available`;
  }

  get jobSearchFiltersObj(): ElasticSearchJobSearchFilter {
    return JSON.parse(this.jobSearchFilters);
  }

  get keywordsPart(): string {
    let result = '';
    if (this.keyword && this.keyword.trim().length > 0) {
      result =
        (this.searchField === 'all' ? 'search' : this.searchField) +
        '=' +
        this.keyword.trim();
    }
    return result ? ';' + result : result;
  }

  get filterTags(): string[] {
    return this.filters.filter(
      x =>
        !x.startsWith('Search:') &&
        !x.startsWith('Job title:') &&
        !x.startsWith('Employer:') &&
        !x.startsWith('Job number:')
    );
  }

  get emailAlertFrequency(): string {
    return this.getAlertFrequency(this.alertFrequency);
  }

  private setKeyword() {
    if (this.urlParameters) {
      const params = this.urlParameters.split(';').filter(x => !!x);
      for (const param of params) {
        const paramName = param.split('=')[0];
        const paramValue = decodeURI(param.split('=')[1]);
        switch (paramName) {
          case 'search':
          case 'title':
          case 'employer':
            this.keyword = paramValue;
            this.searchField = paramName === 'search' ? 'all' : paramName;
            break;
        }
      }
    }
  }

  private getSalaryIntervalText(option: string): string {
    let result = '';
    switch (option) {
      case '0':
        result = 'per hour';
        break;
      case '1':
        result = 'per week';
        break;
      case '2':
        result = 'biweekly';
        break;
      case '3':
        result = 'per month';
        break;
      case '4':
        result = 'per year';
        break;
    }
    return result;
  }

  private setFilter(result: string[], radius: string, cityOrPostal: string) {
    if (!radius || +radius.split('=')[1] === -1)
      result.push('Location: ' + cityOrPostal);
    else {
      const locationInfo = getLocationInfo(radius.split('=')[1], cityOrPostal);
      result.push(locationInfo);
    }
  }

  private getDateFormat(date: string): string {
    let result = date;
    if (date && date.length === 8) {
      const year = date.substr(0, 4);
      const month = date.substr(4, 2);
      const day = date.substr(6);
      result = `${year}/${month}/${day}`;
    }
    return result;
  }

  //filters: string[] = [];
  private _filters: string[];
  get filters(): string[] {
    if (!this._filters) {
      this._filters = this.getFilters();
    }
    return this._filters;
  }
  set filters(value: string[]) {
    this._filters = value;
  }

  private getFilters(): string[] {
    const result: string[] = [];
    if (this.urlParameters) {
      let startDate = '';
      let endDate = '';
      let salaryrangeMin: number;
      let salaryrangeMax: number;

      const params = this.urlParameters.split(';').filter(x => !!x);

      const radius = params.find(x => x.startsWith('radius'));

      const salaryInterval = params.find(x => x.startsWith('salaryinterval'));
      const salaryIntervalValue = salaryInterval
        ? salaryInterval.split('=')[1]
        : '4';
      const salaryIntervalText = this.getSalaryIntervalText(salaryIntervalValue);

      for (const param of params) {
        const paramName = param.split('=')[0];
        const paramValue = decodeURIComponent(param.split('=')[1]);
        const paramValues = paramValue ? paramValue.split(',') : [];

        switch (paramName) {
          case 'postal':
            for (const postal of paramValues) {
              this.setFilter(
                result,
                radius,
                PostalCodeService.formatPostalCode(postal)
              );
            }
            break;
          case 'city':
            for (const city of paramValues) {
              this.setFilter(result, radius, city);
            }
            break;
          case 'region':
            for (const region of paramValues) {
              result.push(
                'Location: ' + LocationService.getLocationTitle(region)
              );
            }
            break;
          case 'title':
            result.push('Job title: ' + paramValue);
            break;
          case 'job':
            result.push('Job number: ' + paramValue);
            break;
          case 'employer':
            result.push('Employer: ' + paramValue);
            break;
          case 'search':
            result.push('Keywords: ' + paramValue);
            break;
          case 'noc':
            result.push(
              'More Filters: ' +
                (paramValue.startsWith('NOC ')
                  ? paramValue
                  : 'NOC ' + paramValue)
            );
            break;
          case 'startdate':
            startDate = paramValue;
            break;
          case 'enddate':
            endDate = paramValue;
            result.push(
              'Date Range: ' +
                this.getDateFormat(startDate) +
                ' - ' +
                this.getDateFormat(endDate)
            );
            break;
          case 'datetype':
            if (paramValue === '1') result.push('Date Posted: Today');
            else if (paramValue === '2')
              result.push('Date Posted: Past three days');
            break;
          case 'jobsource':
            switch (paramValue) {
              case '1':
                result.push('More Filters: WorkBC');
                break;
              case '2':
                result.push('More Filters: External (other job boards)');
                break;
              case '3':
                result.push('More Filters: Federal government');
                break;
              case '4':
                result.push('More Filters: Municipal government');
                break;
              case '5':
                result.push('More Filters: BC Public Service');
                break;
            }
            break;
          case 'salaryrange':
            for (const value of paramValues) {
              let salaryRanges;

              switch (salaryIntervalValue) {
                case '0':
                  salaryRanges = SalaryRange.hourly;
                  break;
                case '1':
                  salaryRanges = SalaryRange.weekly;
                  break;
                case '2':
                  salaryRanges = SalaryRange.biweekly;
                  break;
                case '3':
                  salaryRanges = SalaryRange.monthly;
                  break;
                default:
                  salaryRanges = SalaryRange.annually;
                  break;
              }

              switch (value) {
                case '1':
                  result.push('Salary:\t ' + salaryRanges.range1);
                  break;
                case '2':
                  result.push('Salary:\t\t ' + salaryRanges.range2);
                  break;
                case '3':
                  result.push('Salary:\t\t\t ' + salaryRanges.range3);
                  break;
                case '4':
                  result.push('Salary:\t\t\t\t ' + salaryRanges.range4);
                  break;
                case '5':
                  result.push('Salary:\t\t\t\t\t ' + salaryRanges.range5);
                  break;
                case '7':
                  result.push('Salary:\t\t\t\t\t\t\t Unknown salaries');
                  break;
              }
            }
            break;
          case 'salaryrangemin':
            salaryrangeMin = +paramValue;
            break;
          case 'salaryrangemax': {
            salaryrangeMax = +paramValue;

            const decimals = +salaryIntervalValue === 0 ? 2 : 0;
            const formatter = new Intl.NumberFormat('en-US', {
              style: 'currency',
              currency: 'USD',
              minimumFractionDigits: decimals,
              maximumFractionDigits: decimals
            });

            result.push(
              `Salary:\t\t\t\t\t\t ${formatter.format(salaryrangeMin)}${
              salaryrangeMax !== 0
                ? ` to ${formatter.format(salaryrangeMax)}`
                : '+'
              } ${salaryIntervalText}`
            );
            break;
          }
          case 'placementagency':
            switch (paramValue) {
              case '1':
                result.push('More Filters: Exclude placement agency jobs');
                break;
            }
            break;
          case 'language':
            switch (paramValue) {
              case '1':
                result.push(
                  'More Filters: English and French'
                );
                break;
            }
            break;
          case 'education':
            this.addFilters(CheckboxInfo.education, paramValue, result);
            break;
          case 'employmentgroups':
            this.addFilters(CheckboxInfo.moreFilters, paramValue, result);
            break;
          case 'industry':
            this.addFilters(CheckboxInfo.industry, paramValue, result);
            break;
          case 'benefits':
            this.addFilters(CheckboxInfo.salary, paramValue, result);
            break;
          case 'hoursofwork':
          case 'periodofemployment':
          case 'employmentterms':
          case 'workplacetype':
            this.addFilters(
              CheckboxInfo.jobType,
              paramValue,
              result,
              paramName
            );
            break;
        }
      }
    }
    return result;
  }

  private addFilters(
    groupDef = [],
    csv: string,
    activeFilters: string[],
    groupKey?: string
  ) {
    const values: string[] = csv.split(',');
    for (let i = 0; i < groupDef.length; i++) {
      const group = groupDef[i];
      if (!groupKey || group.key === groupKey) {
        for (let j = 0; j < group.filters.length; j++) {
          const item = group.filters[j];
          for (let k = 0; k < values.length; k++) {
            if (item.id === values[k]) {
              const label = group.prefix + ': ' + item.label;
              activeFilters.push(label);
            }
          }
        }
      }
    }
  }

  private getAlertFrequency(alertFrequency: number): string {
    let result = '';
    switch (alertFrequency) {
      case 1:
        result = 'Daily';
        break;
      case 2:
        result = 'Weekly';
        break;
      case 3:
        result = 'Biweekly';
        break;
      case 4:
        result = 'Monthly';
        break;
      default:
        result = 'Never';
    }
    return result;
  }
}
