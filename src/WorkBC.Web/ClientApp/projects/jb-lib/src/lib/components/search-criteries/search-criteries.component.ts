import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FilterService } from '../../filters/services/filter.service';
import { Router } from '@angular/router';

@Component({
  selector: 'lib-search-criteries',
  templateUrl: './search-criteries.component.html',
  styleUrls: ['./search-criteries.component.scss']
})
export class SearchCriteriesComponent {
  @Input() inJobAlert = false;
  @Input() filterTags: Array<string> = [];

  @Output() deleted = new EventEmitter<string>();
  @Output() deletedAll = new EventEmitter();
  @Output() cleared = new EventEmitter();

  showMore = false;

  constructor(
    private filterService: FilterService,
    private router: Router
  ) {}

  get activeTags(): Array<string> {
    if (this.inJobAlert) {
      return this.filterTags.filter(
        x =>
          !x.startsWith('Keywords: ') &&
          !x.startsWith('Job title: ') &&
          !x.startsWith('Employer: ') &&
          !x.startsWith('Job number: ')
      );
    } else {
      return this.filterTags;
    }
  }

  removeFilter(filter: string): void {
    this.filterService.removeFilter(filter, this.inJobAlert);
  }

  clearAll(): void {
    this.cleared.emit();
    this.filterService.removeFilter("page")
    this.filterService.setBookmarkableUrl(null, this.inJobAlert, this.router.url);
  }
}
