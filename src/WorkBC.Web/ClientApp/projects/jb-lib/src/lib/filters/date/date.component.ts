import { Component, ElementRef, Input } from '@angular/core';
import { FilterService } from '../services/filter.service';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { DateFilterModel, JbDate } from '../models/filter.model';
import { BaseFilterComponent } from '../models/base-component.model';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-date',
  templateUrl: './date.component.html',
  styleUrls: ['./date.component.scss'],
  host: {
    '(document:click)': 'onClick($event)'
  }
})
export class DateComponent extends BaseFilterComponent {
  @Input() inJobAlert = false;

  invalidStartDate = false;
  invalidEndDate = false;
  invalidEndDateBeforeStartDate = false;

  constructor(
    filterService: FilterService,
    private dropdown: NgbDropdown,
    private settings: SystemSettingsService,
    private _eref: ElementRef
  ) {
    super(dropdown, filterService);
    this.vm.isDisabled = this.vm.rangeSelected != 3;
  }

  get datePostedTitle(): string {
    return this.settings.shared.filters.datePostedTitle;
  }

  onClick(event) {
    // makes sure the dropdown stays open when user picks a date.
    let datePicker = false;
    let parent = event.target.parentNode;

    while (parent != null && !datePicker) {
      if (parent.tagName && parent.tagName === 'NGB-DATEPICKER') {
        datePicker = true;
      }
      parent = parent.parentNode;
    }

    // if user clicks off the dropdown or opens another one, close this one
    if (!this._eref.nativeElement.contains(event.target) && !datePicker)
      this.dropdown.close();
  }

  get vm(): DateFilterModel {
    return this.filterService.currentFilter.dateFilters;
  }
  set vm(value: DateFilterModel) {
    this.filterService.currentFilter.dateFilters = value;
  }

  get maxDate(): JbDate {
    const now = new Date();
    return {
      year: now.getFullYear(),
      month: now.getMonth() + 1,
      day: now.getDate()
    };
  }

  changeRange(value) {

    switch (value) {
      case '1': {
        //today
        const todayDate = this.maxDate;
        this.vm.startDate = todayDate;
        this.vm.endDate = todayDate;
        break;
      }
      case '2':{
        //past 3 days
        const endDate = this.maxDate;
        const pastThreeDays = new Date();
        pastThreeDays.setDate(pastThreeDays.getDate() - 3); //subtract 3 days from current date
        const startDate = {
          year: pastThreeDays.getFullYear(),
          month: pastThreeDays.getMonth() + 1,
          day: pastThreeDays.getDate()
        };

        this.vm.startDate = startDate;
        this.vm.endDate = endDate;
        break;
      }
      default:
        //any other dates
        this.vm.endDate = null;
        this.vm.startDate = null;
        break;
    }

    this.vm.isDisabled = value != 3;
  }

  startDateSelect() {
    this.invalidStartDate = false;
    this.validateDateRange();
  }

  endDateSelect() {
    this.invalidEndDate = false;
    this.validateDateRange();
  }

  validateDateRange() {
    this.invalidEndDateBeforeStartDate = false;

    if (this.vm.endDate != null && this.vm.startDate != null) {
      const startDate = new Date(this.vm.startDate.year, this.vm.startDate.month, this.vm.startDate.day);
      const endDate = new Date(this.vm.endDate.year, this.vm.endDate.month, this.vm.endDate.day);

      if (endDate < startDate) {
        this.invalidEndDateBeforeStartDate = true;
      }
    }
  }

  clear() {
    this.vm = new DateFilterModel();
    this.invalidStartDate = false;
    this.invalidEndDate = false;
    this.invalidEndDateBeforeStartDate = false;
  }

  getJobs(dropdownMenuId: string) {
    this.invalidEndDate = false;
    this.invalidStartDate = false;
    this.invalidEndDateBeforeStartDate = false;

    if (this.vm.rangeSelected == 3) {
      if (this.vm.endDate == null) {
        this.invalidEndDate = true;
      }

      if (this.vm.startDate == null) {
        this.invalidStartDate = true;
      }

      this.validateDateRange();

      if (this.invalidEndDate || this.invalidStartDate || this.invalidEndDateBeforeStartDate) {
        return;
      }
    }

    super.getJobs(dropdownMenuId);
  }
}
