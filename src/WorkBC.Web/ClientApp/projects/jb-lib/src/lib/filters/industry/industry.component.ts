import { Component, ElementRef, Input } from '@angular/core';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { IndustryFilterModel } from '../models/filter.model';
import { FilterService } from '../services/filter.service';
import { BaseFilterComponent } from '../models/base-component.model';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-industry',
  templateUrl: './industry.component.html',
  styleUrls: ['./industry.component.scss'],
  host: {
    '(document:click)': 'onClick($event)'
  }
})
export class IndustryComponent extends BaseFilterComponent {
  @Input() inJobAlert = false;

  constructor(
    private dropdown: NgbDropdown,
    private _eref: ElementRef,
    private settings: SystemSettingsService,
    filterService: FilterService
  ) {
    super(dropdown, filterService);
  }

  get industryTitle(): string {
    return this.settings.shared.filters.industryTitle;
  }

  get vm(): IndustryFilterModel {
    return this.filterService.currentFilter.industryFilters;
  }
  set vm(value: IndustryFilterModel) {
    this.filterService.currentFilter.industryFilters = value;
  }

  onClick(event) {
    // if user clicks off the dropdown or opens another one, close this one
    if (!this._eref.nativeElement.contains(event.target)) this.dropdown.close();
  }

  clear() {
    this.vm = new IndustryFilterModel();
  }
}
