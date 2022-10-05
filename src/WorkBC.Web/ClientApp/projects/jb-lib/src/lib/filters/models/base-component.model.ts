import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { FilterService } from '../services/filter.service';

export class BaseFilterComponent {
  constructor(
    private ngbDropdown: NgbDropdown,
    protected filterService: FilterService
  ) {}

  getJobs(dropdownMenuId: string): void {
    // reset the page number to 1 ever time somebody applies a filter
    this.filterService.currentFilter.pagination.currentPage = 1;

    // apply the filter
    this.filterService.setFilters();

    if (this.ngbDropdown) {
      this.ngbDropdown.close();
    }

    // Make sure dropdown is completely closed before scroll into view
    setTimeout(() => {
      this.filterService.scrollIntoView(dropdownMenuId);
    }, 0);
  }
}

export class BaseComponent extends BaseFilterComponent {
  invalidCity = false;
  invalidPostal = false;
  submitted = false;

  stopPropagation(event: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
  }
}
