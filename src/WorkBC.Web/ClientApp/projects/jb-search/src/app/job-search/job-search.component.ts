import { Component } from '@angular/core';

@Component({
  selector: 'app-job-search',
  templateUrl: './job-search.component.html',
  styleUrls: ['./job-search.component.css']
})
export class JobSearchComponent {

  isJobNoSearch = false;
  onKeywordOrSearchFieldChanged(isJobNoSearch: boolean): void {
    if (this.isJobNoSearch !== isJobNoSearch) {
      setTimeout(() => {
        this.isJobNoSearch = isJobNoSearch;
      });
    }
  }
}
