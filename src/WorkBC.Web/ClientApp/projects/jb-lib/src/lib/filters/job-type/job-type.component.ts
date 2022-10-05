import { Component, ElementRef, Input } from '@angular/core';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { FilterService } from '../services/filter.service';
import { JobTypeFilterModel } from '../models/filter.model';
import { BaseFilterComponent } from '../models/base-component.model';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-job-type',
  templateUrl: './job-type.component.html',
  styleUrls: ['./job-type.component.scss'],
  host: {
    '(document:click)': 'onClick($event)'
  }
})
export class JobTypeComponent extends BaseFilterComponent {
  @Input() inJobAlert = false;

  constructor(
    private dropdown: NgbDropdown,
    private _eref: ElementRef,
    private settings: SystemSettingsService,
    filterService: FilterService
  ) {
    super(dropdown, filterService);
  }

  get jobTypeTitle(): string {
    return this.settings.shared.filters.jobTypeTitle;
  }

  get onSiteTooltip(): string {
    return this.settings.shared.tooltips.onSite;
  }

  get hybridTooltip(): string {
    return this.settings.shared.tooltips.hybrid;
  }

  get travellingTooltip(): string {
    return this.settings.shared.tooltips.travelling;
  }

  get virtualTooltip(): string {
    return this.settings.shared.tooltips.virtual;
  }

  get vm(): JobTypeFilterModel {
    return this.filterService.currentFilter.jobTypeFilters;
  }
  set vm(value: JobTypeFilterModel) {
    this.filterService.currentFilter.jobTypeFilters = value;
  }

  onClick(event) {
    // if user clicks off the dropdown or opens another one, close this one
    if (!this._eref.nativeElement.contains(event.target)) this.dropdown.close();
  }

  clear() {
    this.vm = new JobTypeFilterModel();
  }

  getPopoverTriggers() {
    // On mobile, .toggle() won't work nicely with 'mouseenter:mouseleave' trigger.
    const isMobile = /Mobi/i.test(navigator.userAgent) || /Android/i.test(navigator.userAgent);
    return isMobile ? 'manual' : 'mouseenter:mouseleave';
  }
}
