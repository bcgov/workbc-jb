import { Component, ElementRef, Input } from '@angular/core';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { MoreFiltersFilterModel } from '../models/filter.model';
import { FilterService } from '../services/filter.service';
import { Observable, Subscription, fromEvent } from 'rxjs';
import {
  map,
  startWith,
  debounceTime,
  distinctUntilChanged,
  switchMap
} from 'rxjs/operators';
import { DataService } from '../services/data.service';
import { NocCode } from '../models/job.model';
import { BaseComponent } from '../models/base-component.model';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-more',
  templateUrl: './more.component.html',
  styleUrls: ['./more.component.scss'],
  host: {
    '(document:click)': 'onClick($event)'
  }
})
export class MoreComponent extends BaseComponent {
  @Input() inJobAlert = false;
  nocCodes$: Observable<NocCode[]>;
  selectedNoc: string;
  dirtyNoc: boolean;
  invalidNoc = false;
  private subscription: Subscription;

  constructor(
    private dropdown: NgbDropdown,
    private _eref: ElementRef,
    private dataService: DataService,
    private settings: SystemSettingsService,
    filterService: FilterService
  ) {
    super(dropdown, filterService);
  }

  get moreFiltersTitle(): string {
    return this.settings.shared.filters.moreFiltersTitle;
  }

  get nocCodeTooltip(): string {
    return this.settings.shared.tooltips.nocCode;
  }

  get jobSourceTooltip(): string  {
    return this.settings.shared.tooltips.jobSource;
  }

  get vm(): MoreFiltersFilterModel {
    return this.filterService.currentFilter.moreFilters;
  }
  set vm(value: MoreFiltersFilterModel) {
    this.filterService.currentFilter.moreFilters = value;
  }

  get postLanguage(): string {
    return this.vm.isPostingsInEnglish
      ? '1'
      : this.vm.isPostingsInEnglishAndFrench
      ? '2'
      : '0';
  }
  set postLanguage(value: string) {
    this.vm.isPostingsInEnglish = value === '1';
    this.vm.isPostingsInEnglishAndFrench = value === '2';
  }
  get isMobile() {
    const result = /Mobi/i.test(navigator.userAgent) || /Android/i.test(navigator.userAgent);
    return result;
  }

  ngAfterViewInit(): void {
    this.setNocCodesRetrieval();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private setNocCodesRetrieval() {
    const searchBox = document.getElementById('NOCcodes');

    const typeahead = fromEvent(searchBox, 'input').pipe(
      map((e: KeyboardEvent) => (e.target as HTMLInputElement).value),
      startWith(''),
      //filter(text => text.length > 0),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(partialNocCode => {
        this.nocCodes$ = this.dataService.getNocCodes(partialNocCode);
        return this.nocCodes$;
      })
    );

    this.subscription = typeahead
      .subscribe
      //data => {
      //    console.log(data.length);
      //}
      ();
  }

  onClick(event) {
    // if user clicks off the dropdown or opens another one, close this one
    if (!this._eref.nativeElement.contains(event.target)) this.dropdown.close();
  }

  selectNoc(event) {
    this.selectedNoc = event.option.value;

    //user selected a NOC in the list - so this would be a valid NOC code
    this.invalidNoc = false;

    // clear the autocomplete options after a value is selected from the autocomplete
    setTimeout(() => {
      this.nocCodes$ = new Observable<NocCode[]>();
    }, 50);
  }

  checkNoc() {
    if (!this.selectedNoc || this.selectedNoc !== this.vm.nocCode) {
      const code: string = (this.vm.nocCode).replace(/\D/g, '');
      if (this.dirtyNoc || code.length > 0 && code.length !== 4) {
        this.invalidNoc = true;
      }
    }
    else
      this.invalidNoc = false;

    //clear error when no noc code selected (empty field)
    if (this.vm.nocCode == '')
      this.invalidNoc = false;
  }

  setDirtyNoc() {
    this.dirtyNoc = true;

    //remove error when input is empty
    const code: string = (this.vm.nocCode).replace(/\D/g, '');
    if (code.length == 0)
      this.invalidNoc = false;
  }

  clear() {
    this.vm = new MoreFiltersFilterModel();
    this.invalidNoc = false;
  }

  handleSpace(e) {
    e.stopPropagation();
    return true;
  }

  getJobs(dropdownMenuId: string) {
    if (this.invalidNoc)
      return;

    super.getJobs(dropdownMenuId);
  }

  getPopoverTriggers() {
    // On mobile, .toggle() won't work nicely with 'mouseenter:mouseleave' trigger.
    return this.isMobile ? 'manual' : 'mouseenter:mouseleave';
  }

  getNocTriggers() {
    return this.isMobile ? 'manual' : 'mouseenter';
  }
}
