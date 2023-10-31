import { Injectable } from '@angular/core';
import { Location } from '@angular/common';
import { Params } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import {
  KeyWordFilterModel,
  MainFilterModel,
  DateFilterModel,
  SalaryFilterModel,
  EducationFilterModel,
  IndustryFilterModel,
  JobTypeFilterModel,
  MoreFiltersFilterModel,
  LocationField,
  JbDate
} from '../models/filter.model';
import { BaseService } from './base.service';
import { CheckboxInfo } from './checkboxinfo.service';
import { PostalCodeService } from './postalcode.service';
import { LocationService } from './location.service';
import { SystemSettingsService } from '../../services/system-settings.service';
import { CheckboxCategory } from '../../models/checkbox-category';

@Injectable({
  providedIn: 'root'
})
export class FilterService extends BaseService {
  private filterSubject: BehaviorSubject<MainFilterModel>;
  mainFilterModels$: Observable<MainFilterModel>;

  constructor(
    public location: Location,
    private settings: SystemSettingsService
  ) {
    super();

    //Setup of filter structures - init all filters
    const mainFilter = new MainFilterModel();

    this.filterSubject = new BehaviorSubject<MainFilterModel>(mainFilter);
    this.mainFilterModels$ = this.filterSubject.asObservable();
  }

  get currentFilter(): MainFilterModel {
    return this.filterSubject.value;
  }

  private setActiveFilterAndUrl(
    mainFilters: MainFilterModel,
    redirect: string,
    key: string,
    value: string,
    label: string,
    replaceIfStartsWith = '',
    allowMultiple = true,
    startDateStr = '',
    endDateStr = ''
  ): string {
    mainFilters.addActiveFilter(label, replaceIfStartsWith);
    redirect = this.addParam(key, value, redirect, allowMultiple);

    if (value === '3' && key === 'datetype') {
      if (startDateStr.length === 10) {
        redirect = this.addParam(
          'startdate',
          startDateStr.replace(/\//g, ''),
          redirect,
          allowMultiple
        );
      }

      if (endDateStr.length === 10) {
        redirect = this.addParam(
          'enddate',
          endDateStr.replace(/\//g, ''),
          redirect,
          allowMultiple
        );
      }
    }

    return redirect;
  }

  private clearActiveFilters(mainFilterModel: MainFilterModel) {
    mainFilterModel.activeFilters = mainFilterModel.filtersWithoutKeyword;
  }

  setFilterDate(redirect: string) {
    //retrieve all current filters and only update the date filters
    const mainFilters = this.currentFilter;

    // clear the 'datetype' param from the url
    redirect = this.clearParam('datetype', redirect);
    redirect = this.clearParam('startdate', redirect);
    redirect = this.clearParam('enddate', redirect);

    const rangeSelected = mainFilters.dateFilters.rangeSelected.toString();
    switch (rangeSelected) {
      case '0':
        mainFilters.removeActiveFilter('Date Posted:', true);
        mainFilters.removeActiveFilter('Date Range:', true);
        break;
      case '1':
      case '2':
        redirect = this.setActiveFilterAndUrl(
          mainFilters,
          redirect,
          'datetype',
          rangeSelected,
          rangeSelected === '1'
            ? 'Date Posted: Today'
            : 'Date Posted: Past three days',
          'Date '
        );
        break;
      case '3':
        if (
          mainFilters.dateFilters.startDate &&
          mainFilters.dateFilters.endDate
        ) {
          const startDateStr =
            mainFilters.dateFilters.startDate.year +
            '/' +
            this.padLeft(
              mainFilters.dateFilters.startDate.month.toString(),
              '0',
              2
            ) +
            '/' +
            this.padLeft(
              mainFilters.dateFilters.startDate.day.toString(),
              '0',
              2
            );
          const endDateStr =
            mainFilters.dateFilters.endDate.year +
            '/' +
            this.padLeft(
              mainFilters.dateFilters.endDate.month.toString(),
              '0',
              2
            ) +
            '/' +
            this.padLeft(
              mainFilters.dateFilters.endDate.day.toString(),
              '0',
              2
            );

          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'datetype',
            '3',
            'Date Range: ' + startDateStr + ' - ' + endDateStr,
            'Date ',
            true,
            startDateStr,
            endDateStr
          );
        }
        break;
    }
    return redirect;
  }

  padLeft(text: string, padChar: string, size: number): string {
    return (String(padChar).repeat(size) + text).substr(size * -1, size);
  }

  setFilterKeyword(redirect: string, filter?: KeyWordFilterModel) {
    //retrieve all current filters and only update the keyword filters
    const mainFilters = this.currentFilter;
    if (filter) {
      mainFilters.keywordFilters = filter;
    }

    const keywordFilters = mainFilters.keywordFilters;

    if (keywordFilters.keyword) {
      const searchField = keywordFilters.searchField.toLocaleLowerCase();

      if (searchField !== 'all') {
        redirect = this.clearParam('search', redirect);
        mainFilters.removeActiveFilter('Keywords:', true);
      }

      if (searchField !== 'title') {
        redirect = this.clearParam('title', redirect);
        mainFilters.removeActiveFilter('Job title:', true);
      }

      if (searchField !== 'employer') {
        redirect = this.clearParam('employer', redirect);
        mainFilters.removeActiveFilter('Employer:', true);
      }

      if (searchField !== 'jobid') {
        redirect = this.clearParam('job', redirect);
        mainFilters.removeActiveFilter('Job number:', true);
      }

      switch (searchField) {
        case 'title':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'title',
            keywordFilters.keyword,
            'Job title: ' + keywordFilters.keyword,
            'Job title: ',
            false
          );
          break;
        case 'jobid':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'job',
            keywordFilters.keyword,
            'Job number: ' + keywordFilters.keyword,
            'Job number: ',
            false
          );
          break;
        case 'employer':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'employer',
            keywordFilters.keyword,
            'Employer: ' + keywordFilters.keyword,
            'Employer: ',
            false
          );
          break;
        case 'all':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'search',
            keywordFilters.keyword,
            'Keywords: ' + keywordFilters.keyword,
            'Keywords: ',
            false
          );
          break;
      }
    } else {
      this.clearActiveFilters(mainFilters);
    }

    //City or Postal in main Keyword Filters

    // if the value is the city or postal field was edited, then remove all the location filters
    if (keywordFilters.cityOrPostal !== mainFilters.oldCityOrPostal) {
      // if the text field was just manually cleared then remove the first location and it's label
      // (but don't replace the text with the new first item because the user actually wanted it to be cleared)
      if (
        !keywordFilters.cityOrPostal.trim().length &&
        mainFilters.oldCityOrPostal.trim().length &&
        mainFilters.locationFields.length > 0
      ) {
        const oldLabel = mainFilters.oldLocationLabel(
          mainFilters.locationFields[0].radius
        );

        const locationFields = [...mainFilters.locationFields];
        locationFields.splice(0, 1);
        mainFilters.locationFields = locationFields;

        mainFilters.removeActiveFilter(oldLabel);
      }

      // set new filters
      if (keywordFilters.cityOrPostal && keywordFilters.cityOrPostal.trim().length) {
        //There are locations applied to the filter, we need to remove the radius, and not add any radius
        const lf = new LocationField();
        lf.city = keywordFilters.city;
        lf.region = '';
        lf.postal = keywordFilters.postal;
        lf.radius = keywordFilters.radius;

        if (
          mainFilters.oldCityOrPostal.length &&
          mainFilters.locationFields.length > 1 &&
          (keywordFilters.city.length || keywordFilters.postal.length)
        ) {
          const position = mainFilters.activeFilters.indexOf(
            mainFilters.oldLocationLabel()
          );
          if (position !== -1) {
            mainFilters.activeFilters[position] = lf.getLocationTag();
          } else {
            mainFilters.addActiveFilter(lf.getLocationTag());
          }

          mainFilters.locationFields[0] = lf;
        } else {
          // clear location filters from the model
          mainFilters.locationFields = new Array<LocationField>();

          //remove any previous filters
          const itemsToRemove = mainFilters.activeFilters.filter(
            item => item.indexOf('Location:') > -1
          );

          //loop through items to remove, and find them in the array and remove
          for (let i = 0; i < itemsToRemove.length; i++) {
            mainFilters.removeActiveFilter(itemsToRemove[i]);
          }

          if (keywordFilters.city.length || keywordFilters.postal.length) {
            const locationFields = [...mainFilters.locationFields];
            locationFields.push(lf);
            mainFilters.locationFields = locationFields;

            mainFilters.addActiveFilter(
              keywordFilters.getLocationTag()
            );
          } else {
            mainFilters.oldCityOrPostal = '';
            mainFilters.keywordFilters.cityOrPostal = '';
          }
        }
      }

      mainFilters.oldCityOrPostal = keywordFilters.cityOrPostal;

      //update the location filter url params
      redirect = this.setLocationUrl(redirect);
    }

    if (filter) {
      this.update(mainFilters);
    }

    return redirect;
  }

  setPagination(redirect: string) {
    //retrieve all current filters and only update the pagination filters

    // clear the 'page' param from the url
    redirect = this.clearParam('page', redirect);
    if (this.currentFilter.pagination.currentPage !== 1) {
      redirect = this.addParam(
        'page',
        this.currentFilter.pagination.currentPage,
        redirect
      );
    }

    return redirect;
  }

  setSortingOrder(order) {
    //retrieve all current filters and only update the pagination filters
    const mainFilters = this.currentFilter;
    mainFilters.sortOrder = order;

    this.update(mainFilters);
  }

  clearParam(param: string, url: string): string {
    return this.removeParam(param, null, url);
  }

  removeParam(param: string, value: string, url: string): string {
    //querystring part
    let queryString = '';

    //split by ; (get the querystring section of the URL)
    const urlParts = url.split(';');

    //join the rest of the parameters
    let partsJoined = '';
    for (let i = 1; i < urlParts.length; i++) {
      partsJoined += urlParts[i] + ';';
    }
    partsJoined = partsJoined.substring(0, partsJoined.length - 1);

    //split by different keys
    const vars = partsJoined.split(';');

    for (let i = 0; i < vars.length; i++) {
      //get all the values for each key
      const pair = vars[i].split('=');

      //if it is the key we want to remove
      if (param === pair[0]) {
        //split the values to see if there are more than one value for this key (checkboxes)
        let values: string[];
        if (value !== null) {
          values = pair[1].split(',');
        } else {
          values = [];
        }

        if (values.length > 1) {
          //append to querystring
          queryString += pair[0] + '=';

          //loop through values and only remove the applicable value
          for (let k = 0; k < values.length; k++) {
            if (values[k] != value) {
              //append the value if its not the value that we need to remove
              queryString += decodeURIComponent(values[k]) + ',';
            }
          }

          //remove last comma
          queryString = queryString.substring(0, queryString.length - 1);

          //add ; to indicate the end of the grouping
          queryString += ';';
        }
      } else {
        //different key - append to querystring
        if (pair[0] != '') {
          queryString += pair[0] + '=' + pair[1] + ';';
        }
      }
    }

    //clean-up
    if (queryString.endsWith(',')) {
      queryString = queryString.substring(0, queryString.length - 1);
    }

    //return new URL
    return urlParts[0] + (queryString ? ';' + queryString : '');
  }

  addParam(key, value, url, allowMultiple = true): string {
    //placeholder for new querystring
    let tmp = '';

    //if key already exists, add a comma with the additional value
    if (url.indexOf(key + '=') > -1) {
      //find the key and completely remove it from the url
      const items = url.split(';');
      for (let i = 0; i < items.length; i++) {
        const group = items[i].split('=');
        if (group.length === 2) {
          //new query string key, add it
          if (group[0] != key) {
            tmp += ';' + group[0] + '=' + group[1];
          } else {
            //key already exists
            //Check if the value that we are trying to add already exist for this key
            if (group[1].indexOf(value) === -1) {
              //does not exist, add to the url
              if (allowMultiple) {
                //append value to already existing values
                tmp += ';' + group[0] + '=' + group[1] + ',' + value;
              } else {
                //only add latest value - multiple values not allowed
                tmp += ';' + group[0] + '=' + value;
              }
            } else {
              //value already exist, do not add it
              tmp += ';' + group[0] + '=' + group[1];
            }
          }
        } else if (i === 0) {
          tmp = group[i];
        }
      }

      //return URL part
      return tmp;
    } else {
      return (
        url +
        (url.substring(url.length - 1) == ';' ? '' : ';') +
        key +
        '=' +
        value +
        ';'
      );
    }
  }

  /**
   * Processes a removal tag for a checkbox option. Sets the model property to
   * false when a checbox filter is removed, and removes the value of the checkbox
   * from an optional list property.  Also removes the selection from the bookmarkable
   * URL parameters.
   */
  removeParams(groupDef: Array<CheckboxCategory>, tag: string, updatedUrl: string, isSalary = false): string {
    const mainFilters = this.currentFilter;
    for (let i = 0; i < groupDef.length; i++) {
      const group = groupDef[i];
      for (let j = 0; j < group.filters.length; j++) {
        const item = group.filters[j];
        if (tag === group.prefix + ': ' + item.label) {
          mainFilters[group.obj][item.key] = false;
          if (group.listName) {
            const index = mainFilters[group.obj][group.listName].findIndex(
              el => el == item[group.listValueField]
            );
            if (index > -1) {
              mainFilters[group.obj][group.listName].splice(index, 1);
            }
            if (index > -1 || isSalary) {
              return this.removeParam(group.key, item.id, updatedUrl);
            }
          } else {
            return this.removeParam(group.key, item.id, updatedUrl);
          }
        }
      }
    }
    return updatedUrl;
  }

  /**
   * Sets the filter model state for a set of checkboxes based on a
   * specified parameter from the url
   */
  applyUrlState(groupDef: Array<CheckboxCategory>, groupKey: string, csv: string) {
    const values: string[] = csv.split(',');
    const mainFilters = this.currentFilter;
    for (let i = 0; i < groupDef.length; i++) {
      if (groupDef[i].key === groupKey) {
        const group = groupDef[i];
        for (let j = 0; j < group.filters.length; j++) {
          for (let k = 0; k < values.length; k++) {
            if (group.filters[j].id === values[k]) {
              const item = group.filters[j];
              mainFilters[group.obj][item.key] = true;
            }
          }
        }
      }
    }
  }

  /**
   * Sets the removal tags, url paramaters, and an optional list property. Uses
   * a set of checkbox filters (defined in a javascript object) and the current
   * fitler model state.
   */
  addFilters(groupDef: Array<CheckboxCategory>, filter: unknown, redirect: string): string {
    const mainFilters = this.currentFilter;
    for (let i = 0; i < groupDef.length; i++) {
      const group = groupDef[i];
      for (let j = 0; j < group.filters.length; j++) {
        const item = group.filters[j];
        const label = group.prefix + ': ' + item.label;
        if (filter[item.key]) {
          // only add the activeFilter if it doesn't already exist
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            group.key,
            item.id,
            label,
            ''
          );
          if (group.listName) {
            mainFilters[group.obj][group.listName].push(
              item[group.listValueField]
            );
          }
        } else {
          // remove the activeFilter
          mainFilters.removeActiveFilter(label);
        }
      }
    }
    return redirect;
  }

  removeFilter(filter: string, replaceState = false): void {
    //retrieve all current filters and remove specific filter
    const mainFilters = this.currentFilter;

    //reference to URL
    let updatedUrl = this.iniRedirect;

    const pos = mainFilters.activeFilters.indexOf(filter);
    if (pos > -1) {
      mainFilters.removeActiveFilter(filter);

      if (filter.indexOf('Date ') === 0) {
        mainFilters.dateFilters.rangeSelected = 0;
        mainFilters.dateFilters.endDate = null;
        mainFilters.dateFilters.startDate = null;
        mainFilters.dateFilters.isDisabled = true;

        //remove from URL
        updatedUrl = this.clearParam('datetype', updatedUrl);
        updatedUrl = this.clearParam('startdate', updatedUrl);
        updatedUrl = this.clearParam('enddate', updatedUrl);
      }

      const prefix1 = 'Salary:\t ';
      const prefix2 = 'Salary:\t\t ';
      const prefix3 = 'Salary:\t\t\t ';
      const prefix4 = 'Salary:\t\t\t\t ';
      const prefix5 = 'Salary:\t\t\t\t\t ';
      const prefix6 = 'Salary:\t\t\t\t\t\t ';
      const prefix7 = 'Salary:\t\t\t\t\t\t\t ';

      //removing Salary filter
      if (filter.startsWith('Salary:') || filter.startsWith('Benefits:')) {
        switch (filter.split(' ')[0] + ' ') {
          case 'Benefits: ':
            updatedUrl = this.removeParams(
              CheckboxInfo.salary,
              filter,
              updatedUrl,
              true
            );
            mainFilters.removeActiveFilter(filter);
            break;
          case prefix1:
            mainFilters.salaryFilters.amountRange1 = false;
            updatedUrl = this.removeParam('salaryrange', '1', updatedUrl);
            break;
          case prefix2:
            mainFilters.salaryFilters.amountRange2 = false;
            updatedUrl = this.removeParam('salaryrange', '2', updatedUrl);
            break;
          case prefix3:
            mainFilters.salaryFilters.amountRange3 = false;
            updatedUrl = this.removeParam('salaryrange', '3', updatedUrl);
            break;
          case prefix4:
            mainFilters.salaryFilters.amountRange4 = false;
            updatedUrl = this.removeParam('salaryrange', '4', updatedUrl);
            break;
          case prefix5:
            mainFilters.salaryFilters.amountRange5 = false;
            updatedUrl = this.removeParam('salaryrange', '5', updatedUrl);
            break;
          case prefix6:
            mainFilters.salaryFilters.amountRange6 = false;
            mainFilters.salaryFilters.minAmount = '';
            mainFilters.salaryFilters.maxAmount = '';
            updatedUrl = this.removeParam('salaryrange', '6', updatedUrl);
            updatedUrl = this.clearParam('salaryrangemin', updatedUrl);
            updatedUrl = this.clearParam('salaryrangemax', updatedUrl);
            break;
          case prefix7:
            mainFilters.salaryFilters.searchSalaryUnknown = false;
            updatedUrl = this.removeParam('salaryrange', '7', updatedUrl);
            break;
        }
      }

      //removing Education filter
      if (filter.startsWith('Education:')) {
        updatedUrl = this.removeParams(
          CheckboxInfo.education,
          filter,
          updatedUrl
        );
      }

      //removing Industry filter
      if (filter.startsWith('Industry:')) {
        updatedUrl = this.removeParams(
          CheckboxInfo.industry,
          filter,
          updatedUrl
        );
      }

      //removing Job Type filter
      if (filter.startsWith('Job Type:')) {
        updatedUrl = this.removeParams(
          CheckboxInfo.jobType,
          filter,
          updatedUrl
        );
      }

      //removing More filter
      if (filter.startsWith('More Filters:')) {
        switch (filter) {
          case 'More Filters: English and French':
            mainFilters.moreFilters.isPostingsInEnglishAndFrench = false;
            mainFilters.moreFilters.isPostingsInEnglish = true;
            updatedUrl = this.clearParam('language', updatedUrl);
            break;
          case 'More Filters: Exclude placement agency jobs':
            mainFilters.moreFilters.excludePlacementAgencyJobs = false;
            updatedUrl = this.clearParam('placementagency', updatedUrl);
            break;
          case 'More Filters: WorkBC':
          case 'More Filters: External (other job boards)':
          case 'More Filters: Federal government':
          case 'More Filters: Municipal government':
          case 'More Filters: BC Public Service':
            mainFilters.moreFilters.jobSource = '0';
            updatedUrl = this.clearParam('jobsource', updatedUrl);
            break;
          default:
            updatedUrl = this.removeParams(
              CheckboxInfo.moreFilters,
              filter,
              updatedUrl
            );
            break;
        }

        if (filter.toLowerCase().indexOf('more filters: noc') > -1) {
          mainFilters.moreFilters.nocCode = '';
          updatedUrl = this.clearParam('noc', updatedUrl);
        }
      }

      //removing Location filter
      if (filter.indexOf('Location:') > -1) {
        if (mainFilters.locationFields != null) {
          for (let i = 0; i < mainFilters.locationFields.length; i++) {
            const loc = mainFilters.locationFields[i];

            if (filter === loc.getLocationTag()) {
              //get position of location to remove and stop the for-loop
              const locationFields = [...mainFilters.locationFields];
              locationFields.splice(i, 1);
              mainFilters.locationFields = locationFields;
              break;
            }
          }

          if (
            mainFilters.locationFields.length &&
            mainFilters.locationFields[0].region === ''
          ) {
            if (mainFilters.locationFields[0].postal.length) {
              mainFilters.oldCityOrPostal = mainFilters.locationFields[0].formatPostal();
              mainFilters.keywordFilters.cityOrPostal = mainFilters.locationFields[0].formatPostal();
            } else {
              mainFilters.oldCityOrPostal = mainFilters.locationFields[0].city;
              mainFilters.keywordFilters.cityOrPostal =
                mainFilters.locationFields[0].city;
            }
          } else {
            mainFilters.oldCityOrPostal = '';
            mainFilters.keywordFilters.cityOrPostal = '';
          }
        }
      }

      if (mainFilters.keywordFilters.keyword) {
        if (filter.startsWith('Job title: ')) {
          mainFilters.keywordFilters.keyword = '';
          updatedUrl = this.clearParam('title', updatedUrl);
        }

        if (filter.startsWith('Job number: ')) {
          mainFilters.keywordFilters.keyword = '';
          updatedUrl = this.clearParam('job', updatedUrl);
        }

        if (filter.startsWith('Employer: ')) {
          mainFilters.keywordFilters.keyword = '';
          updatedUrl = this.clearParam('employer', updatedUrl);
        }

        if (filter.startsWith('Keywords: ')) {
          mainFilters.keywordFilters.keyword = '';
          updatedUrl = this.clearParam('search', updatedUrl);
        }
      }

      //Additional logic (Add 15km radius if there is only one location and its a postal code)
      if (
        mainFilters.locationFields != null &&
        mainFilters.locationFields.length == 1
      ) {
        if (
          mainFilters.locationFields[0].postal &&
          mainFilters.locationFields[0].postal.length
        ) {
          const tagPos = mainFilters.activeFilters.indexOf(
            mainFilters.locationFields[0].getLocationTag()
          );
          //add radius of 15km back to the list
          const defaultRadius = this.settings.shared.settings.defaultSearchRadius || 15;
          mainFilters.locationFields[0].radius = defaultRadius;
          mainFilters.activeFilters[
            tagPos
          ] = mainFilters.locationFields[0].getLocationTag();
        }
      }

      this.update(mainFilters);

      //Go to the updated URL
      updatedUrl = this.setLocationUrl(updatedUrl);
      this.goLocation(updatedUrl, replaceState);
    }
  }

  goLocation(path: string, replaceState = false): void {
    const oldUrl = window.location.href;
    !replaceState ? this.location.go(path) : this.location.replaceState(path);
    // track a pageview in snowlplow analytics
    if (oldUrl !== window.location.href && (window as any).snowplow) {
      (window as any).snowplow('trackPageView');
    }
  }

  setScrollElement(element) {
    //set the scroll html element
    const mainFilters = this.currentFilter;
    mainFilters.scrollElement = element;

    this.update(mainFilters);
  }

  removeAllTags() {
    //retrieve all current filters and remove all tags
    const mainFilters = this.currentFilter;

    //reset all

    //reset actual filters
    mainFilters.activeFilters = [];

    //reset keyword filters
    mainFilters.keywordFilters = new KeyWordFilterModel();
    mainFilters.oldCityOrPostal = '';
    mainFilters.cities = [];

    //reset date filters
    mainFilters.dateFilters = new DateFilterModel();

    //reset salary filters
    mainFilters.salaryFilters = new SalaryFilterModel();

    //reset education filters
    mainFilters.educationFilters = new EducationFilterModel();

    //reset industry filters
    mainFilters.industryFilters = new IndustryFilterModel();

    //reset job type filters
    mainFilters.jobTypeFilters = new JobTypeFilterModel();

    //reset more filters
    mainFilters.moreFilters = new MoreFiltersFilterModel();

    //reset location filters
    mainFilters.locationFields = new Array<LocationField>();

    //reset sorting order
    mainFilters.sortOrder = 11; //relevance

    this.update(mainFilters);
  }

  getScrollElement(): HTMLElement {
    return this.currentFilter.scrollElement;
  }

  setSalaryFilter(redirect: string): string {
    const prefix1 = 'Salary:\t ';
    const prefix2 = 'Salary:\t\t ';
    const prefix3 = 'Salary:\t\t\t ';
    const prefix4 = 'Salary:\t\t\t\t ';
    const prefix5 = 'Salary:\t\t\t\t\t ';
    const prefix6 = 'Salary:\t\t\t\t\t\t ';
    const prefix7 = 'Salary:\t\t\t\t\t\t\t ';

    //retrieve all current filters and only update the pagination filters
    const mainFilters = this.currentFilter;

    // clear the params from the url
    redirect = this.clearParam('benefits', redirect);
    redirect = this.clearParam('salaryrange', redirect);
    redirect = this.clearParam('salaryrangemin', redirect);
    redirect = this.clearParam('salaryrangemax', redirect);
    redirect = this.clearParam('salaryinterval', redirect);

    redirect = this.addFilters(
      CheckboxInfo.salary,
      mainFilters.salaryFilters,
      redirect
    );

    if (mainFilters.salaryFilters.amountRange1) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'salaryrange',
        '1',
        prefix1 + mainFilters.salaryFilters.amountRange1Text,
        prefix1
      );
    } else {
      mainFilters.removeActiveFilter(prefix1, true);
    }

    if (mainFilters.salaryFilters.amountRange2) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'salaryrange',
        '2',
        prefix2 + mainFilters.salaryFilters.amountRange2Text,
        prefix2
      );
    } else {
      mainFilters.removeActiveFilter(prefix2, true);
    }

    if (mainFilters.salaryFilters.amountRange3) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'salaryrange',
        '3',
        prefix3 + mainFilters.salaryFilters.amountRange3Text,
        prefix3
      );
    } else {
      mainFilters.removeActiveFilter(prefix3, true);
    }

    if (mainFilters.salaryFilters.amountRange4) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'salaryrange',
        '4',
        prefix4 + mainFilters.salaryFilters.amountRange4Text,
        prefix4
      );
    } else {
      mainFilters.removeActiveFilter(prefix4, true);
    }

    if (mainFilters.salaryFilters.amountRange5) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'salaryrange',
        '5',
        prefix5 + mainFilters.salaryFilters.amountRange5Text,
        prefix5
      );
    } else {
      mainFilters.removeActiveFilter(prefix5, true);
    }

    if (
      mainFilters.salaryFilters.amountRange6 &&
      mainFilters.salaryFilters.minAmount != ''
    ) {
      let decimals = 0;
      if (mainFilters.salaryFilters.salaryEarningInterval == 0) {
        decimals = 2;
      }

      const formatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
      });

      const minAmount: number = parseFloat(mainFilters.salaryFilters.minAmount);
      let maxAmount = 0;
      if (
        mainFilters.salaryFilters.maxAmount.toLowerCase() != 'unlimited' &&
        mainFilters.salaryFilters.maxAmount.toLowerCase() != ''
      ) {
        maxAmount = parseFloat(mainFilters.salaryFilters.maxAmount);
      }
      const intervalText = mainFilters.salaryFilters.salaryIntervalText;

      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'salaryrange',
        '6',
        `${prefix6}${formatter.format(minAmount)}${
          maxAmount !== 0 ? ` to ${formatter.format(maxAmount)}` : '+'
        } ${intervalText}`,
        prefix6
      );
      redirect = this.addParam('salaryrangemin', minAmount, redirect);
      redirect = this.addParam('salaryrangemax', maxAmount, redirect);
    } else {
      mainFilters.removeActiveFilter(prefix6, true);
    }

    if (mainFilters.salaryFilters.searchSalaryUnknown) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'salaryrange',
        '7',
        prefix7 + 'Unknown salaries',
        prefix7
      );
    } else {
      mainFilters.removeActiveFilter(prefix7, true);
    }

    if (mainFilters.salaryFilters.salaryEarningInterval !== 4) {
      redirect = this.addParam(
        'salaryinterval',
        mainFilters.salaryFilters.salaryEarningInterval,
        redirect
      );
    }

    return redirect;
  }

  setEducationFilter(redirect: string) {
    const filter = this.currentFilter.educationFilters;
    filter.activeFilters = [];

    // clear the 'education' param from the url
    redirect = this.clearParam('education', redirect);

    redirect = this.addFilters(CheckboxInfo.education, filter, redirect);

    return redirect;
  }

  setIndustryFilter(redirect: string) {
    const filter = this.currentFilter.industryFilters;
    filter.activeFilters = [];

    // clear the 'industry' param from the url
    redirect = this.clearParam('industry', redirect);

    redirect = this.addFilters(CheckboxInfo.industry, filter, redirect);

    return redirect;
  }

  setJobTypeFilter(redirect: string) {
    const filter = this.currentFilter.jobTypeFilters;

    // clear params from the url
    for (const param of [
      'hoursofwork',
      'periodofemployment',
      'employmentterms',
      'workplacetype'
    ]) {
      redirect = this.clearParam(param, redirect);
    }

    redirect = this.addFilters(CheckboxInfo.jobType, filter, redirect);

    return redirect;
  }

  setMoreFilter(redirect: string) {
    const mainFilters = this.currentFilter;
    const filter = mainFilters.moreFilters;

    // clear the 'employmentgroups' param from the url
    redirect = this.clearParam('employmentgroups', redirect);

    redirect = this.addFilters(CheckboxInfo.moreFilters, filter, redirect);

    if (filter.nocCode) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'noc',
        MainFilterModel.getNocCode(filter.nocCode),
        'More Filters: ' + this.getNocCode(filter.nocCode),
        'More Filters: NOC',
        false
      );
    } else {
      mainFilters.removeActiveFilter('More Filters: NOC', true);
      redirect = this.clearParam('noc', redirect);
    }

    if (filter.isPostingsInEnglishAndFrench) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'language',
        '1',
        'More Filters: English and French',
        ''
      );
    } else {
      mainFilters.removeActiveFilter(
        'More Filters: English and French'
      );
      redirect = this.clearParam('language', redirect);
    }

    if (filter.excludePlacementAgencyJobs) {
      redirect = this.setActiveFilterAndUrl(
        mainFilters,
        redirect,
        'placementagency',
        '1',
        'More Filters: Exclude placement agency jobs',
        ''
      );
    } else {
      mainFilters.removeActiveFilter(
        'More Filters: Exclude placement agency jobs'
      );
      redirect = this.clearParam('placementagency', redirect);
    }

    if (filter.jobSource != null) {
      if (filter.jobSource !== '1') {
        mainFilters.removeActiveFilter('More Filters: WorkBC');
      }
      if (filter.jobSource !== '2') {
        mainFilters.removeActiveFilter(
          'More Filters: External (other job boards)'
        );
      }
      if (filter.jobSource !== '3') {
        mainFilters.removeActiveFilter('More Filters: Federal government');
      }
      if (filter.jobSource !== '4') {
        mainFilters.removeActiveFilter('More Filters: Municipal government');
      }
      if (filter.jobSource !== '5') {
        mainFilters.removeActiveFilter('More Filters: BC Public Service');
      }

      switch (filter.jobSource) {
        case '0':
          //show all
          redirect = this.clearParam('jobsource', redirect);
          break;
        case '1':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'jobsource',
            '1',
            'More Filters: WorkBC',
            '',
            false
          );
          break;
        case '2':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'jobsource',
            '2',
            'More Filters: External (other job boards)',
            '',
            false
          );
          break;
        case '3':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'jobsource',
            '3',
            'More Filters: Federal government',
            '',
            false
          );
          break;
        case '4':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'jobsource',
            '4',
            'More Filters: Municipal government',
            '',
            false
          );
          break;
        case '5':
          redirect = this.setActiveFilterAndUrl(
            mainFilters,
            redirect,
            'jobsource',
            '5',
            'More Filters: BC Public Service',
            ''
          );
          break;
      }
    }

    return redirect;
  }

  setLocationFilter(redirect: string) {
    //retrieve all current filters and only update the pagination filters
    const mainFilters = this.currentFilter;
    const locationFilters = mainFilters.locationFields;

    //get all existing location tags
    let locationTags = mainFilters.activeFilters.filter(item =>
      item.startsWith('Location:')
    );

    // use Exact Location if there is more than 1 filter
    if (locationFilters.length > 1) {
      locationFilters.forEach(function (lf) {
        lf.radius = -1;
      });
    }

    // do updates and inserts
    locationFilters.forEach(function(lf) {
      if (locationTags.indexOf(lf.getLocationTag()) > -1) {
        // tag has not changed (do nothing)
      } else {
        // find the tag
        const currentTag = locationTags.filter(
          item =>
            (lf.city !== '' && item.endsWith(' ' + lf.city)) ||
            (lf.postal !== '' && item.endsWith(lf.formatPostal())) ||
            (lf.region !== '' && item === 'Location: ' + lf.region)
        );

        if (currentTag !== undefined && currentTag.length) {
          // tag aleady exists but radius has changed
          for (let i = 0; i < mainFilters.activeFilters.length; i++) {
            if (mainFilters.activeFilters[i].startsWith('Location')) {
              const item = mainFilters.activeFilters[i];
              if (
                item.endsWith(' ' + lf.city) ||
                item.endsWith(lf.formatPostal()) ||
                item === 'Location: ' + lf.region
              ) {
                mainFilters.activeFilters[i] = lf.getLocationTag();
              }
            }
          }
        } else {
          mainFilters.addActiveFilter(lf.getLocationTag());
        }
      }
    });

    // remove extra locations
    locationTags = mainFilters.activeFilters.filter(item =>
      item.startsWith('Location:')
    );

    locationTags.forEach(function(lt) {
      const lf = locationFilters.filter(item => item.getLocationTag() === lt);
      if (lf === undefined || lf.length === 0) {
        mainFilters.removeActiveFilter(lt);
      }
    });

    // remove duplicates
    mainFilters.activeFilters = Array.from(new Set(mainFilters.activeFilters));

    // update the cityOrPostal field in the main search area
    if (mainFilters.locationFields && mainFilters.locationFields.length) {
      const firstFilter = mainFilters.locationFields[0];
      if (firstFilter.postal) {
        mainFilters.keywordFilters.cityOrPostal = firstFilter.postal;
        mainFilters.oldCityOrPostal = firstFilter.postal;
      }

      if (firstFilter.city) {
        mainFilters.keywordFilters.cityOrPostal = firstFilter.city;
        mainFilters.oldCityOrPostal = firstFilter.city;
      }
    }

    // update location url
    return this.setLocationUrl(redirect);
  }

  setLocationUrl(redirect: string) {
    const mainFilters = this.currentFilter;

    // clear urls
    redirect = this.clearParam('city', redirect);
    redirect = this.clearParam('postal', redirect);
    redirect = this.clearParam('region', redirect);
    redirect = this.clearParam('radius', redirect);

    for (let i = 0; i < mainFilters.locationFields.length; i++) {
      const lf = mainFilters.locationFields[i];

      if (lf.city) {
        redirect = this.addParam('city', lf.city, redirect, true);
      }

      if (lf.postal) {
        redirect = this.addParam(
          'postal',
          PostalCodeService.unformatPostalCode(lf.postal),
          redirect,
          true
        );
      }

      if (lf.region) {
        const regionId = LocationService.getLocationId(lf.region);
        redirect = this.addParam('region', regionId, redirect, true);
      }

      if (i === 0 && lf.radius !== -1 && !lf.region) {
        redirect = this.addParam('radius', lf.radius, redirect, false);
      }
    }

    return redirect;
  }

  removeMainLocation(location) {
    //retrieve all current filters and only update the pagination filters
    const mainFilters = this.currentFilter;

    //remove location filter
    mainFilters.removeActiveFilter(location);

    //find filter item and remove it
    const locationItem = mainFilters.activeFilters.filter(
      item => item.indexOf(location.getLocationTag()) > -1
    );

    if (locationItem.length == 1) {
      mainFilters.removeActiveFilter(locationItem[0]);
    }

    //clear city/postal field
    mainFilters.keywordFilters.cityOrPostal = '';
    mainFilters.oldCityOrPostal = '';

    //Update observ
    this.update(mainFilters);
  }

  setFilters(): void {
    let redirect = this.iniRedirect;

    redirect = this.setFilterKeyword(redirect);
    redirect = this.setLocationFilter(redirect);
    redirect = this.setJobTypeFilter(redirect);
    redirect = this.setSalaryFilter(redirect);
    redirect = this.setIndustryFilter(redirect);
    redirect = this.setEducationFilter(redirect);
    redirect = this.setFilterDate(redirect);
    redirect = this.setMoreFilter(redirect);
    redirect = this.setPagination(redirect);

    this.goLocation(redirect, true);

    this.update(this.currentFilter);
  }

  update(mainFilterModel: MainFilterModel) {
    this.filterSubject.next(mainFilterModel);
  }

  setBookmarkableUrl(params: Params, inJobAlert = false, path = ''): void {
    //always clear the state before we set the bookmarks else the state will overwrite it
    this.removeAllTags();

    if (params) {
      if (
        !(Object.keys(params).length === 0 && params.constructor === Object)
      ) {
        //loop through key/values
        for (const key in params) {
          //read value
          const value = params[key];

          //set specific filter
          this.setSpecificFilter(key, value);
        }

        //update filters
        this.setFilters();
      }
    } else {
      //remove all params in url
      this.goLocation(
        !inJobAlert ? '/job-search' : path,
        inJobAlert
      );
    }
  }

  setSpecificFilter(key, value): void {
    //retrieve all current filters
    const mainFilters = this.currentFilter;
    key = key.toLowerCase();

    switch (key) {
      case 'datetype':
        mainFilters.dateFilters.rangeSelected = value;
        mainFilters.dateFilters.isDisabled = value != 3;
        break;

      case 'startdate':
        if (!isNaN(value) && value.length === 8) {
          mainFilters.dateFilters.startDate = new JbDate(
            +value.substring(0, 4),
            +value.substring(4, 6),
            +value.substring(6, 8)
          );
        }
        break;

      case 'enddate':
        if (!isNaN(value) && value.length === 8) {
          mainFilters.dateFilters.endDate = new JbDate(
            +value.substring(0, 4),
            +value.substring(4, 6),
            +value.substring(6, 8)
          );
        }
        break;

      case 'employmentgroups':
        this.applyUrlState(CheckboxInfo.moreFilters, key, value);
        break;

      case 'noc':
        if (!isNaN(value)) {
          value = ('0000' + value).slice(-4);
          mainFilters.moreFilters.nocCode = value;
        }
        break;

      case 'jobsource':
        mainFilters.moreFilters.jobSource = value;
        break;

      case 'placementagency':
        mainFilters.moreFilters.excludePlacementAgencyJobs = true;
        break;

      case 'language':
        mainFilters.moreFilters.isPostingsInEnglishAndFrench = true;
        mainFilters.moreFilters.isPostingsInEnglish = false;
        break;

      case 'education':
        this.applyUrlState(CheckboxInfo.education, key, value);
        break;

      case 'industry':
        this.applyUrlState(CheckboxInfo.industry, key, value);
        break;

      case 'benefits':
        this.applyUrlState(CheckboxInfo.salary, key, value);
        break;

      case 'salaryrange':
        for (let i = 0; i < value.split(',').length; i++) {
          const k = value.split(',')[i];
          if (k >= 1 && k <= 6) {
            mainFilters.salaryFilters['amountRange' + k] = true;
          }
          if (k == 7) {
            mainFilters.salaryFilters.searchSalaryUnknown = true;
          }
        }
        break;

      case 'salaryrangemax':
        mainFilters.salaryFilters.maxAmount = value == 0 ? '' : value;
        break;

      case 'salaryrangemin':
        mainFilters.salaryFilters.minAmount = value;
        break;

      case 'salaryinterval':
        mainFilters.salaryFilters.salaryEarningInterval = value;
        break;

      case 'hoursofwork':
      case 'periodofemployment':
      case 'employmentterms':
      case 'workplacetype':
        this.applyUrlState(CheckboxInfo.jobType, key, value);
        break;

      case 'search':
        mainFilters.keywordFilters.keyword = value;
        break;

      case 'city': {
        const cities = value.split(',');
        for (let k = 0; k < cities.length; k++) {
          // only set the top cityOrPosal filter if the city is in the
          // first location position (i.e. if the array is currently empty)
          if (!mainFilters.locationFields.length) {
            if (!mainFilters.keywordFilters.cityOrPostal.length) {
              mainFilters.keywordFilters.cityOrPostal = cities[k];
              mainFilters.oldCityOrPostal = cities[k];
            }
          }

          const lf = new LocationField();
          lf.city = cities[k];
          lf.radius = -1;

          const locationFields = [...mainFilters.locationFields];
          locationFields.push(lf);
          mainFilters.locationFields = locationFields;

          // set the radius
          if (mainFilters.locationFields.length > 1) {
            mainFilters.locationFields[0].radius = -1;
          }
        }
        break;
      }
      case 'postal': {
        const postals = value.split(',');
        for (let k = 0; k < postals.length; k++) {
          // only set the top cityOrPostal filter if the postal code is in the
          // first location position (i.e. if the array is currently empty)
          if (!mainFilters.locationFields.length) {
            if (!mainFilters.keywordFilters.cityOrPostal.length) {
              mainFilters.keywordFilters.cityOrPostal = postals[k];
              mainFilters.oldCityOrPostal = postals[k];
            }
          }

          const lf = new LocationField();
          lf.postal = PostalCodeService.formatPostalCode(postals[k]);
          lf.radius = -1;

          const locationFields = [...mainFilters.locationFields];
          locationFields.push(lf);
          mainFilters.locationFields = locationFields;

          // set the radius
          if (mainFilters.locationFields.length === 1) {
            const defaultRadius = this.settings.shared.settings.defaultSearchRadius || 15;
            mainFilters.locationFields[0].radius = defaultRadius;
          } else {
            mainFilters.locationFields[0].radius = -1;
          }
        }
        break;
      }
      case 'region': {
        const regions = value.split(',');
        for (let k = 0; k < regions.length; k++) {
          const lf = new LocationField();
          lf.region = LocationService.getLocationTitle(regions[k]);
          lf.radius = -1;

          const locationFields = [...mainFilters.locationFields];
          locationFields.push(lf);
          mainFilters.locationFields = locationFields;

          // set the radius
          if (mainFilters.locationFields.length > 1) {
            mainFilters.locationFields[0].radius = -1;
          }
        }
        break;
      }
      case 'radius':
        if (mainFilters.locationFields.length === 1) {
          mainFilters.locationFields[0].radius = value;
        }
        break;

      case 'employer':
        mainFilters.keywordFilters.keyword = value;
        mainFilters.keywordFilters.searchField = 'employer';
        break;
      case 'job':
        mainFilters.keywordFilters.keyword = value;
        mainFilters.keywordFilters.searchField = 'jobId';
        break;
      case 'title':
        mainFilters.keywordFilters.keyword = value;
        mainFilters.keywordFilters.searchField = 'title';
        break;
      case 'pagesize':
        mainFilters.pagination.resultsPerPage = value;
        break;
      case 'page':
        mainFilters.pagination.currentPage = value;
        break;
      case 'sortby':
        mainFilters.sortOrder = value;
        break;
    }
  }

  get iniRedirect() {
    const href = window.location.href;
    return href.substring(href.indexOf('#') + 1);
  }

  getRedirect(param: string, value: number, allowMultiple: boolean): string {
    let redirect = this.iniRedirect;
    redirect = this.clearParam(param, redirect);
    redirect = this.addParam(param, value, redirect, allowMultiple);
    return redirect;
  }

  sortByChange(param: string): void {
    let redirect = this.getRedirect(param, this.currentFilter.sortOrder, false);

    // change back to page 1
    this.currentFilter.pagination.currentPage = 1;
    redirect = this.clearParam('page', redirect);

    this.update(this.currentFilter);

    this.goLocation(redirect);

    // update the filter tags
    this.setFilters();
  }

  getNocCode(nocCode: string): string {
    const result = nocCode.toUpperCase().startsWith('NOC ')
      ? nocCode.split(' ')
      : ('NOC ' + nocCode).split(' ');
    return result.length > 1 ? `${result[0]} ${result[1]}` : nocCode;
  }

  getUrlParams(locationPath: string): string {
    let result = '';
    if (locationPath && locationPath.indexOf(';') !== -1) {
      result = locationPath.substring(locationPath.indexOf(';'));
    }
    return result;
  }
}
