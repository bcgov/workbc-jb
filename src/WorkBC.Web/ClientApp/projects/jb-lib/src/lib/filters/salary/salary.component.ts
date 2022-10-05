import { Component, ElementRef, Input } from '@angular/core';
import {
  trigger,
  state,
  style,
  animate,
  transition
} from '@angular/animations';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { FilterService } from '../services/filter.service';
import { SalaryFilterModel } from '../models/filter.model';
import { BaseFilterComponent } from '../models/base-component.model';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-salary',
  templateUrl: './salary.component.html',
  styleUrls: ['./salary.component.scss'],
  host: {
    '(document:click)': 'onClick($event)'
  },
  animations: [
    trigger('smoothCollapse', [
      state(
        'initial',
        style({
          height: '0',
          overflow: 'hidden',
          opacity: '0'
        })
      ),
      state(
        'final',
        style({
          overflow: 'hidden',
          opacity: '1'
        })
      ),
      transition('initial=>final', animate('200ms')),
      transition('final=>initial', animate('200ms'))
    ])
  ]
})
export class SalaryComponent extends BaseFilterComponent {
  @Input() inJobAlert = false;
  isCollapsed = true;

  constructor(
    private dropdown: NgbDropdown,
    private _eref: ElementRef,
    filterService: FilterService,
    public settings: SystemSettingsService
  ) {
    super(dropdown, filterService);
  }

  get unknownSalaries(): string {
    return this.settings.shared.tooltips.unknownSalaries;
  }

  get howIsSalaryCalculatedTitle(): string {
    return this.settings.shared.filters.howIsSalaryCalculatedTitle;
  }

  get howIsSalaryCalculatedBody(): string {
    return this.settings.shared.filters.howIsSalaryCalculatedBody;
  }

  get salaryTitle(): string {
    return this.settings.shared.filters.salaryTitle;
  }

  get vm(): SalaryFilterModel {
    return this.filterService.currentFilter.salaryFilters;
  }
  set vm(value: SalaryFilterModel) {
    this.filterService.currentFilter.salaryFilters = value;
  }

  onClick(event) {
    // if user clicks off the dropdown or opens another one, close this one
    if (!this._eref.nativeElement.contains(event.target)) this.dropdown.close();
  }

  clear(): void {
    this.vm = new SalaryFilterModel();
    this.isCollapsed = true;
  }

  customChanged() {
    if (!this.vm.amountRange6) {
      this.vm.minAmount = '';
      this.vm.maxAmount = '';
    }
  }

  submitSalarySearch(dropdownMenuId: string): void {
    if (this.vm.amountRange6) {
      if (this.vm.minAmount == '' && this.vm.maxAmount != '') {
        this.vm.minAmount = '0';
      } else if (this.vm.minAmount == '') {
        this.vm.minAmount = '0';
        this.vm.maxAmount = 'unlimited';
      } else if (this.vm.maxAmount == '') {
        this.vm.maxAmount = 'unlimited';
      }

      this.testForNegativeNumber(this.vm.minAmount);
    }

    this.getJobs(dropdownMenuId);
  }

  onMinAmountChange(amount) {
    this.vm.minAmount = this.testForNegativeNumber(amount);
  }

  onMaxAmountChange(amount) {
    this.vm.maxAmount = this.testForNegativeNumber(amount);
  }

  testForNegativeNumber(num) {
    let retval = num;

    //Testing to ensure there isn't negative numbers for salary
    if (num != '') {
      //try to convert to a number
      const minValue = +num;
      if (minValue.toString() === 'NaN' || minValue < 0) {
        //reset value back to 0
        retval = '';
      }
    }

    return retval;
  }

  getPopoverTriggers() {
    // On mobile, .toggle() won't work nicely with 'mouseenter:mouseleave' trigger.
    const isMobile = /Mobi/i.test(navigator.userAgent) || /Android/i.test(navigator.userAgent);
    return isMobile ? 'manual' : 'mouseenter:mouseleave';
  }
}
