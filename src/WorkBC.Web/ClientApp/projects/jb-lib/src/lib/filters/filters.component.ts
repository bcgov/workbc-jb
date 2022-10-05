import {
  Component,
  ElementRef,
  ViewChild,
  OnInit,
  AfterViewInit,
  OnDestroy,
  Input,
  Output,
  EventEmitter,
  ViewChildren,
  //QueryList
} from '@angular/core';
import { NgbDropdownConfig, NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { fromEvent, Observable, Subscription } from 'rxjs';
import {
  debounceTime,
  distinctUntilChanged,
  filter,
  map,
  startWith,
  switchMap
} from 'rxjs/operators';
import { FilterService } from './services/filter.service';
import { LocationService } from './services/location.service';
import { BaseComponent } from './models/base-component.model';
import { PostalCodeService } from './services/postalcode.service';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { SystemSettingsService } from '../services/system-settings.service';
import { LocationField } from './models/filter.model';

//Auto complete
//https://github.com/gmerabishvili/angular-ng-autocomplete#readme

@Component({
  selector: 'app-filter',
  templateUrl: './filters.component.html',
  styleUrls: ['./filters.component.scss']
})
export class FiltersComponent extends BaseComponent
  implements OnInit, AfterViewInit, OnDestroy {

  @Input() inJobAlert = false;
  @Output() openChanged = new EventEmitter<unknown>();
  @Output() keywordOrSearchFieldChanged = new EventEmitter<boolean>();

  isCollapsed = true;

  // Filter Data
  cityOrPostal: string;
  cityOptions: string[] = [];
  city = '';
  postal = '';
  radius = this.settings.shared.settings.defaultSearchRadius || 15;
  
  cities$: Observable<string[]>;
  private subscription: Subscription;

  //reference to the point that we need to scroll up
  @ViewChild('scrolltarget') scrollTo: ElementRef;

  //reference to the cities autocomplete
  @ViewChild('cityOrPostalInput', {
    read: MatAutocompleteTrigger
})
  cityOrPostalInput: MatAutocompleteTrigger;

  constructor(
    filterService: FilterService,
    private locationService: LocationService,
    private settings: SystemSettingsService,
    config: NgbDropdownConfig
  ) {
    super(null, filterService);
    config.autoClose = false;
  }

  ngOnInit(): void {
    // WBCJB-2088 - Monkey patching NgbDropdown so _isEventFromToggle 
    // checks if _anchor is defined before checking nativeElement
    // this broke after the upate from Angular 9 to Angular 10
    (NgbDropdown.prototype as any)._isEventFromToggle = function(event) { 
      if (this._anchor) {
        this._anchor.nativeElement.contains(event.target);
      } else {
        return false;
      }
    }

    //This event will fire everytime a search filter changes
    if (!this.inJobAlert) {
      this.filterService.mainFilterModels$.subscribe(filter => {
        this.keyword = filter.keywordFilters.keyword;
        this.searchField = filter.keywordFilters.searchField;
        this.cityOrPostal = filter.keywordFilters.cityOrPostal;
        this.resetValidation();
      });
    }
  }

  ngAfterViewInit(): void {
    if (!this.inJobAlert) {
      this.setCitiesRetrieval();
    }
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  _keyword = '';
  get keyword(): string {
    return this._keyword;
  }
  set keyword(value: string) {
    this._keyword = value;

    const vmFilters = this.filterService.currentFilter.keywordFilters;
    if (vmFilters.keyword != value) {
      vmFilters.keyword = value;
    }

    const isJobNumberSearch = value && this.searchField === 'jobId';
    this.keywordOrSearchFieldChanged.emit(isJobNumberSearch);
  }

  _searchField = 'all';
  get searchField(): string {
    return this._searchField;
  }
  set searchField(value: string) {
    this._searchField = value;

    const vmFilters = this.filterService.currentFilter.keywordFilters;
    if (vmFilters.searchField != value) {
      vmFilters.searchField = value;
    }

    const isJobNumberSearch = this.keyword && value === 'jobId';
    this.keywordOrSearchFieldChanged.emit(isJobNumberSearch);
  }

  get htmlForInvalidPostalCode(): string {
    let template: string;
    if (PostalCodeService.isOutOfProvince(this.cityOrPostal)) {
      template = this.settings.jbSearch.errors.outOfProvincePostal;
    } else {
      template = this.settings.jbSearch.errors.invalidPostalCode;
    }
    return template.replace('{0}', this.cityOrPostal);
  }

  get htmlForInvalidCity(): string {
    const template = this.settings.jbSearch.errors.invalidCity;
    return template.replace('{0}', this.cityOrPostal);
  }

  get keywordInputPlaceholder(): string {
    return this.settings.shared.filters.keywordInputPlaceholder;
  }

  private resetValidation() {
    this.invalidCity = false;
    this.invalidPostal = false;
  }

  private setCitiesRetrieval() {
    const searchBox = document.getElementById('cityOrPostal');

    const typeahead = fromEvent(searchBox, 'input').pipe(
      map((e: KeyboardEvent) => (e.target as HTMLInputElement).value),
      startWith(''),
      filter(text => text.length > 0),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(city => {
        this.cities$ = this.locationService.getCities(city.trim(), true);
        return this.cities$;
      })
    );

    this.subscription = typeahead.subscribe(data => {
      this.cityOptions = data;

      this.resetValidation();

      const lowerCity = this.cityOrPostal.toLowerCase();
      const matchingCities = this.cityOptions.filter(
        city =>
          city.toLowerCase().startsWith(lowerCity.trim()) ||
          city.toLowerCase().indexOf(' ' + lowerCity.trim()) > -1
      );

      if (!PostalCodeService.isPotentialPostal(this.cityOrPostal)) {
        if (this.cityOptions.length === 0 || !matchingCities.length) {
          this.invalidCity = true;
        }
      }

      if (this.submitted) {
        this.submitted = false;
      }
    });
  }

  private setLocationValues(city: string, postal: string, radius: number) {
    this.city = city;
    this.postal = postal;
    this.radius = radius;
  }

  get invalidCityOrPostal(): boolean {
    return this.cityOrPostal && (this.invalidCity || this.invalidPostal);
  }

  isPostal(): boolean {
    return PostalCodeService.isPostalCode(this.cityOrPostal) && !PostalCodeService.isOutOfProvince(this.cityOrPostal);
  }

  formatPostal(): string {
    return PostalCodeService.formatPostalCode(this.cityOrPostal);
  }

  getLocationTag(): string {
    return PostalCodeService.getLocationTag(
      '',
      this.city,
      this.postal,
      this.radius
    );
  }

  // onBlur validation
  validateCityOrPostal() {
    setTimeout(() => {

      this.resetValidation();
      const keywordFilters = this.filterService.currentFilter.keywordFilters;
      //keywordFilters.cityOrPostal = this.cityOrPostal;

      if (this.cityOrPostal && this.cityOrPostal.length) {
        if (PostalCodeService.isPotentialPostal(this.cityOrPostal)) {
          if (!this.isPostal()) {
            this.invalidPostal = true;
            keywordFilters.city = '';
            keywordFilters.postal = '';
          } else {
            keywordFilters.city = '';
            keywordFilters.postal = this.cityOrPostal;
            keywordFilters.radius = this.settings.shared.settings.defaultSearchRadius;
          }
        } else if (this.cityOptions.length > 0) {
          const lowerCity = this.cityOrPostal.toLowerCase();
          if (!this.cityOptions.some(city => city.toLowerCase() === lowerCity.trim())) {
            this.invalidCity = true;
            keywordFilters.city = '';
            keywordFilters.postal = '';
          } else {
            keywordFilters.city = this.cityOrPostal;
            keywordFilters.postal = '';
            keywordFilters.radius = -1;
          }
        } else {
          this.invalidCity = true;
          keywordFilters.city = '';
          keywordFilters.postal = '';
        }
      }

      // clear the typeahead
      this.cities$ = new Observable<string[]>();

    }, 300);
  }

  refreshCityList() {
    if (this.cityOrPostal.length) {
      this.cities$ = this.locationService.getCities(this.cityOrPostal.trim(), true);
      this.cities$.subscribe(c => { this.cityOptions = c });
    }
  }

  // clears the cities typeahead when the value is cleared with the backspace key
  clearIfEmpty(event) {
    if (event.target.value.length === 0) {
      this.cities$ = new Observable<string[]>();
    }
  }

  getJobs() {
    this.submitted = true;

    //this.resetValidation();

    let redirect = this.filterService.iniRedirect;

    // clear the location filters if the top City or Postal input has been edited
    if (this.cityOrPostal.trim() !== this.filterService.currentFilter.oldCityOrPostal) {
      this.filterService.currentFilter.locationFilter.locationFields = new Array<LocationField>();
      // set a random string as the old Postal code to ensure that the new location filter will be applied
      this.filterService.currentFilter.oldCityOrPostal = 'value cleared / force re-apply filter';
    }

    if (this.cityOrPostal) {
      if (this.isPostal()) {
        //if it is a postal code.
        const radius = this.filterService.currentFilter.locationFields.length > 1
          ? -1
          : this.settings.shared.settings.defaultSearchRadius || 15;
        this.setLocationValues('', this.formatPostal(), radius);

        //trigger change in observ that will trigger a new search
        redirect = this.filterService.setFilterKeyword(redirect, this);
        this.filterService.setFilters();
      } else {
        this.cityOrPostal = this.cityOrPostal.trim();
        const lowerCity = this.cityOrPostal.toLowerCase();

        let cities: string[] = [];
        let isOpen = false;
        let isExactMatch = false;

        if (this.cityOptions.length > 0) {
          cities = this.cityOptions.filter(
            city =>
              city.toLowerCase().startsWith(lowerCity) ||
              city.toLowerCase().indexOf(' ' + lowerCity) > -1
          );
          isOpen = this.cityOrPostalInput.panelOpen;
          isExactMatch = this.cityOptions.filter(city => city.toLowerCase() === lowerCity).length > 0;
        }

        if (cities.length > 0 && (isOpen || isExactMatch)) {
          //City
          this.setLocationValues(
            isExactMatch ? this.cityOrPostal : this.cityOptions[0],
            '',
            -1
          );

          if (!isExactMatch) {
            this.cityOrPostal = this.city;
          }

          // update the input field with the autocomplete value
          this.cityOrPostalInput.closePanel();
        } else if (this.cityOrPostal == this.filterService.currentFilter.oldCityOrPostal) {
          // don't perform validation against the autocomplete if the input wasn't changed
          this.setLocationValues(this.cityOrPostal, '', -1);
        } else {
          this.invalidPostal = PostalCodeService.isPotentialPostal(this.cityOrPostal);
          this.invalidCity = !this.invalidPostal;
        }

        if (!this.invalidCity && !this.invalidPostal) {
          redirect = this.filterService.setFilterKeyword(redirect, this);
          this.filterService.setFilters();
        } else {
          this.filterService.currentFilter.oldCityOrPostal = '';
        }
      }
    }
    else {
      const newRedirect = this.filterService.setFilterKeyword(redirect, this);
      if (redirect !== newRedirect) {
        redirect = newRedirect;
      }
      
      this.filterService.setFilters();
    }
  }

  onSelected(searchField: string): void {
    this.searchField = searchField;
  }

  @ViewChildren(NgbDropdown) dropdowns; // !: QueryList<NgbDropdown>;

  onOpenChange(isOpen: boolean, dropdownMenu: string): void {
    const dropdown = this.dropdowns.find(x => x.isOpen());
    this.openChanged.emit({ isOpen: isOpen, dropdownMenu: dropdownMenu, hasOpen: !!dropdown });
  }
}
