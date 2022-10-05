import {
  Component,
  ElementRef,
  ViewChild,
  Renderer2,
  Input
} from '@angular/core';
import { OnInit } from '@angular/core';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { FilterService } from '../services/filter.service';
import { LocationField, LocationFilterModel, MainFilterModel } from '../models/filter.model';
import { Observable, Subscription, fromEvent } from 'rxjs';
import {
  map,
  startWith,
  debounceTime,
  distinctUntilChanged,
  switchMap,
  filter
} from 'rxjs/operators';
import { LocationService } from '../services/location.service';
import { BaseComponent } from '../models/base-component.model';
import { PostalCodeService } from '../services/postalcode.service';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-location',
  templateUrl: './location.component.html',
  styleUrls: ['./location.component.scss'],
  host: {
    '(document:click)': 'onClick($event)'
  }
})
export class LocationComponent extends BaseComponent implements OnInit {
  @ViewChild('tooltipRef', { read: ElementRef })
  tooltipRef: ElementRef;

  @Input() inJobAlert = false;

  regionNames: Array<string> = [
    'Mainland / Southwest',
    'North Coast & Nechako',
    'Thompson-Okanagan',
    'Vancouver Island / Coast',
    'Kootenay',
    'Cariboo',
    'Northeast'
  ];

  invalidCity = false;
  invalidPostal = false;
  duplicateCity = false;
  duplicatePostal = false;

  cities$: Observable<string[]>;
  private subscription: Subscription;
  private cityOptions: string[] = [];
  
  get mainFilter(): MainFilterModel {
    return this.filterService.currentFilter;
  }

  get vm(): LocationFilterModel {
    return this.mainFilter.locationFilter;
  }
  set vm(value: LocationFilterModel) {
    this.mainFilter.locationFilter = value;
  }

  constructor(
    private dropdown: NgbDropdown,
    private _eref: ElementRef,
    private renderer: Renderer2,
    private locationService: LocationService,
    private settings: SystemSettingsService,
    filterService: FilterService
  ) {
    super(dropdown, filterService);
  }

  ngOnInit(): void {
    //This event will fire everytime a search filter changes
    this.filterService.mainFilterModels$.subscribe(filter => {
      if (filter) {
        this.vm.locationFields = filter.locationFields;
      }
    });
  }

  ngAfterViewInit(): void {
    this.setCitiesRetrieval();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  get locationRadiusNote(): string {
    return this.settings.shared.filters.locationRadiusNote;
  }
  get locationTitle(): string {
    return this.settings.shared.filters.locationTitle;
  }
  get locationSearchLabel(): string {
    return this.settings.shared.filters.locationSearchLabel;
  }
  get locationRegionSearchLabel(): string {
    return this.settings.shared.filters.locationRegionSearchLabel;
  }

  get htmlForInvalidPostalCode(): string {
    let template: string;
    if (PostalCodeService.isOutOfProvince(this.vm.nameOrPostalCode)) {
      template = this.settings.jbSearch.errors.outOfProvincePostal;
    } else {
      template = this.settings.jbSearch.errors.invalidPostalCode;
    }
    return template.replace('{0}', this.vm.nameOrPostalCode);
  }

  get htmlForDuplicatePostal(): string {
    const template = this.settings.jbSearch.errors.duplicatePostal;
    return template.replace('{0}', this.vm.nameOrPostalCode);
  }

  get htmlForInvalidCity(): string {
    const template = this.settings.jbSearch.errors.invalidCity;
    return template.replace('{0}', this.vm.nameOrPostalCode);
  }

  get htmlForDuplicateCity(): string {
    const template = this.settings.jbSearch.errors.duplicateCity;
    return template.replace('{0}', this.vm.nameOrPostalCode);
  }

  get isInvalid(): boolean {
    return (
      this.vm.nameOrPostalCode &&
      (this.invalidCity ||
        this.invalidPostal ||
        this.duplicateCity ||
        this.duplicatePostal)
    );
  }

  get clearAllBgColor() {
    return this.inJobAlert ? 'transparent' : '';
  }
  get clearAllBorder() {
    return this.inJobAlert ? 'none' : '';
  }

  get autoCompletePlaceHolder(): string {
    if (
      this.vm.locationFields &&
      this.vm.locationFields.filter(l => l.region === '').length > 0
    ) {
      return 'Another city or postal code';
    } else {
      return 'Enter city name or postal code';
    }
  }

  removeMainLocation(location) {
    //remove location from main search
    this.filterService.removeMainLocation(location);

    //close dropdown of the filter
    this.dropdown.close();
  }

  private setCitiesRetrieval() {
    const searchBox = document.getElementById('NameOrPostalCode');

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
      this.invalidCity = false;
      this.invalidPostal = false;
      this.duplicateCity = false;
      this.duplicatePostal = false;

      if (!PostalCodeService.isPotentialPostal(this.vm.nameOrPostalCode)) {
        if (this.cityOptions.length === 0) {
          this.invalidCity = true;
        } else if (!this.isValidCity(this.vm.nameOrPostalCode)) {
          this.invalidCity = true;
        }
      }

      if (this.submitted) {
        this.submitted = false;
      }
    });
  }

  // onBlur validation
  validateCityOrPostal() {
    setTimeout(() => {
      this.invalidCity = false;
      this.invalidPostal = false;

      if (this.vm.nameOrPostalCode && this.vm.nameOrPostalCode.length) {
        const isPotentialPostal: boolean = PostalCodeService.isPotentialPostal(this.vm.nameOrPostalCode);

        const isValidPostalCode: boolean = PostalCodeService.isPostalCode(this.vm.nameOrPostalCode)
          && !PostalCodeService.isOutOfProvince(this.vm.nameOrPostalCode);

        if (isPotentialPostal) {
          if (!isValidPostalCode) {
            this.invalidPostal = true;
          }
        } else if (this.cityOptions.length > 0) {
          this.vm.nameOrPostalCode = this.vm.nameOrPostalCode.trim();
          const lowerCity = this.vm.nameOrPostalCode.toLowerCase();
          if (
            !this.cityOptions.some(city => city.toLowerCase() === lowerCity)
          ) {
            this.invalidCity = true;
          }
        } else {
          this.invalidCity = true;
        }
      }
    }, 300);
  }

  isCitySelected(location: string): boolean {
    return this.cityOptions.some(
      city => city.toLowerCase() === location.toLowerCase()
    );
  }

  isValidCity(location: string): boolean {
    const lowerCity = location.toLowerCase().trim();
    const filteredCities = this.cityOptions.filter(
      city =>
        city.toLowerCase().startsWith(lowerCity) ||
        city.toLowerCase().indexOf(' ' + lowerCity) > -1
    );
    return filteredCities.length > 0;
  }

  isInvalidPostcode(location: string): boolean {
    return PostalCodeService.isPotentialPostal(location) &&
      (!PostalCodeService.isPostalCode(location) || PostalCodeService.isOutOfProvince(location));
  }

  isInvalidCity(location: string): boolean {
    if (PostalCodeService.isPostalCode(location)) {
      return false;
    }
    return !this.isValidCity(location) && !this.isInvalidPostcode(location);
  }

  // clears the cities typeahead when the value is cleared with the backspace key
  clearIfEmpty(event) {
    if (event.target.value.length === 0) {
      //this.cityOptions = [];
      this.cities$ = new Observable<string[]>();
    }
  }

  onClick(event) {
    // if user clicks off the dropdown or opens another one, close this one
    if (
      !this._eref.nativeElement.contains(event.target) &&
      event.srcElement.localName != 'svg'
    ) {
      this.dropdown.close();
    }
  }

  addLocation() {
    const location = this.vm.nameOrPostalCode;

    this.duplicatePostal = false;
    this.duplicateCity = false;
    this.invalidCity = this.isInvalidCity(location);
    this.invalidPostal = this.isInvalidPostcode(location);

    if (!(this.invalidCity || this.invalidPostal)) {
      //test for duplicates
      if (
        this.vm.locationFields.some(
          x => x.city.toLowerCase() === location.toLowerCase()
        )
      ) {
        this.duplicateCity = true;
      } else if (
        this.vm.locationFields.some(
          x => x.postal === PostalCodeService.formatPostalCode(location)
        )
      ) {
        this.duplicatePostal = true;
      } else {
        // add the location
        this.selectLocation(location, null, false);

        //set radius value
        if (this.vm.locationFields.length == 1) {
          if (PostalCodeService.isPotentialPostal(location)) {
            this.vm.exactLocationLabel = 'Exact postal code only';
            this.vm.radius = this.settings.shared.settings.defaultSearchRadius || 15;
          } else if (this.isValidCity(location)) {
            this.vm.exactLocationLabel = 'Exact city name only';
            this.vm.radius = -1;
          }
        }

        this.clearCityOrPostcode(location);
      }
    }
  }

  changeRegion(location: string, event = null) {
    // NOTE: public bool has to be the same as the value passed from the frontend for the locations to work

    this.selectLocation(location, event, true);
  }

  clickRegion(location: string, event = null) {
    // NOTE: public bool has to be the same as the value passed from the frontend for the locations to work

    if (event !== null) {
      event.preventDefault();
    }

    const id = LocationService.getLocationId(location);
    this[id] = !this[id];

    this.selectLocation(location, event, true);
  }

  selectLocation(location: string, event = null, isRegion = false) {
    const title: string = LocationService.getLocationTitle(location);

    //Set city/region/postal properties
    const isValidPostal: boolean = PostalCodeService.isPostalCode(title);
    const isCitySelected: boolean = this.isCitySelected(title);

    const loc = new LocationField();

    if (isRegion) {
      loc.city = '';
      loc.region = title;
      loc.postal = '';
      loc.radius = -1;
    } else if (isCitySelected) {
      const cities = this.cityOptions.filter(
        city => city.toLowerCase() === location.toLowerCase()
      );

      if (cities.length > 0) {
        // add the first matching value from the autocomplete
        loc.city = cities[0];
      } else {
        loc.city = title;
      }

      loc.region = '';
      loc.postal = '';
      loc.radius = this.vm.radius;
    } else if (isValidPostal) {
      loc.city = '';
      loc.region = '';
      loc.postal = PostalCodeService.formatPostalCode(title);
      loc.radius = this.vm.radius;
    }

    if (isCitySelected || isRegion || isValidPostal) {
      if (
        !this.vm.locationFields.some(
          x => x.region.toLowerCase() === title.toLowerCase()
        )
      ) {
        const locationFields = [...this.vm.locationFields];
        locationFields.push(loc);
        this.vm.locationFields = locationFields;
      } else {
        this.removeLocation(title);
      }
    }

    this.stopPropagation(event);
  }

  onFocus(): void {
    if (!this.vm.nameOrPostalCode && this.cities$) this.cities$ = null; 
  }

  private clearCityOrPostcode(location: string = null) {
    if (
      this.vm.nameOrPostalCode &&
      (!location || this.vm.nameOrPostalCode === location)
    ) {
      this.vm.nameOrPostalCode = '';
    }
  }

  removeLocation(location: string, event = null) {
    location = location.toLowerCase();

    const index = this.vm.locationFields.findIndex(
      x =>
        x.region.toLowerCase() === location ||
        x.postal.toLowerCase() === location ||
        x.city.toLocaleLowerCase() == location
    );

    if (index !== -1) {
      const locationFields = [...this.vm.locationFields];
      locationFields.splice(index, 1);
      this.vm.locationFields = locationFields;
    }

    if (this.vm.locationFields.length == 1) {
      if (this.vm.locationFields[0].postal) {
        this.vm.radiusLabel = this.vm.locationFields[0].formatPostal();
        this.vm.exactLocationLabel = 'Exact postal code only';
        this.vm.radius = this.settings.shared.settings.defaultSearchRadius || 15;
      } else {
        this.vm.radiusLabel = this.vm.locationFields[0].city;
        this.vm.exactLocationLabel = 'Exact city name only';
        this.vm.radius = -1;
      }
    }

    this.stopPropagation(event);
  }

  hover($event, location) {
    this.vm[location + 'Hover'] = true;
    if ($event) {
      this.vm.tooltipText = LocationService.getLocationTitle(location);
      this.renderer.setStyle(
        this.tooltipRef.nativeElement,
        'top',
        $event.offsetY - 40 + 'px'
      );
      this.renderer.setStyle(
        this.tooltipRef.nativeElement,
        'left',
        $event.offsetX + 'px'
      );
    }
  }

  clearHover(location) {
    this.vm[location + 'Hover'] = false;
    this.vm.tooltipText = '';
  }

  get showLocationRadius(): boolean {
    return (
      this.vm.locationFields &&
      this.vm.locationFields.length === 1 &&
      !this.regionNames.find(
        x =>
          this.vm.locationFields &&
          this.vm.locationFields.length === 1 &&
          x === this.vm.locationFields[0].region
      )
    );
  }

  clear() {
    this.vm = new LocationFilterModel();
  }

  getJobs(event = null) {
    // if there is value in the text box then add it
    if (this.vm.nameOrPostalCode) {
      this.addLocation();
    }

    // bypass validation if the item we just tried to add isn't the only location in the list
    if (this.vm.locationFields.length > 0) {
      this.invalidCity = false;
      this.invalidPostal = false;
    }

    if (this.invalidCity || this.invalidPostal) {
      this.stopPropagation(event);
    } else {
      this.submitted = true;

      // if there is more than one location set all the locations to exact
      if (this.vm.locationFields.length > 1) {
        this.vm.locationFields.forEach(function(value) {
          value.radius = -1;
        });
      }

      const mainFilters = this.mainFilter;

      // clear the city or postal from the top search inputs if there are no more locations
      if (this.vm.locationFields.length === 0) {
        mainFilters.keywordFilters.cityOrPostal = '';
        mainFilters.oldCityOrPostal = '';
      } else {
        // copy the first location city or postal to the top search inputs
        if (this.vm.locationFields[0].region.length) {
          mainFilters.keywordFilters.cityOrPostal = '';
          mainFilters.oldCityOrPostal = '';
        } else if (this.vm.locationFields[0].postal.length) {
          mainFilters.keywordFilters.cityOrPostal = this.vm.locationFields[0].formatPostal();
          mainFilters.oldCityOrPostal = this.vm.locationFields[0].formatPostal();
        } else if (this.vm.locationFields[0].city.length) {
          mainFilters.keywordFilters.cityOrPostal = this.vm.locationFields[0].city;
          mainFilters.oldCityOrPostal = this.vm.locationFields[0].city;
        }
      }

      // clear errors on the top search inputs
      mainFilters.keywordFilters.invalidPostal = false;
      mainFilters.keywordFilters.invalidCity = false;

      // copy the tempLocations to the mainFilter
      mainFilters.locationFields = this.vm.locationFields;

      this.clearCityOrPostcode();

      super.getJobs('locationDropdownMenu');
    }
  }

  get addButtonDisabled(): boolean {
    if (!this.vm.nameOrPostalCode) {
      return true;
    }

    const isPotentialPostal: boolean = PostalCodeService.isPotentialPostal(
      this.vm.nameOrPostalCode
    );
    const isCitySelected: boolean = this.isCitySelected(this.vm.nameOrPostalCode);

    return !(isCitySelected || isPotentialPostal);
  }

  handleSpace(e) {
    e.stopPropagation();
    return true;
  }
}
