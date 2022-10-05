import { Component, ElementRef, Input } from '@angular/core';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { EducationFilterModel } from '../models/filter.model';
import { FilterService } from '../services/filter.service';
import { BaseFilterComponent } from '../models/base-component.model';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-education',
  templateUrl: './education.component.html',
  styleUrls: ['./education.component.scss'],
  host: {
    '(document:click)': 'onClick($event)'
  }
})
export class EducationComponent extends BaseFilterComponent {
  @Input() inJobAlert = false;

  constructor(
    private dropdown: NgbDropdown,
    private _eref: ElementRef,
    private settings: SystemSettingsService,
    filterService: FilterService
  ) {
    super(dropdown, filterService);
  }

  get educationNote(): string {
    return this.settings.shared.filters.educationNote;
  }
  get educationTitle(): string {
    return this.settings.shared.filters.educationTitle;
  }

  get vm(): EducationFilterModel {
    return this.filterService.currentFilter.educationFilters;
  }
  set vm(value: EducationFilterModel) {
    this.filterService.currentFilter.educationFilters = value;
  }

  onClick(event) {
    // if user clicks off the dropdown or opens another one, close this one
    if (!this._eref.nativeElement.contains(event.target)) this.dropdown.close();
  }

  clear() {
    this.vm = new EducationFilterModel();
  }
}
