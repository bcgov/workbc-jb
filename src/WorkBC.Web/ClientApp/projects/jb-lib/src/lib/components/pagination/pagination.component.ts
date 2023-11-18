import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { RouteEventsService } from '../../services/route-events';
import { FilterService } from '../../filters/services/filter.service';
import { PaginationModel } from '../../filters/models/filter.model';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent implements OnInit {
  @Input() showDropdownPage = true;
  @Input() updateUrl = true;
  @Input() vm: PaginationModel = this.filterService.currentFilter.pagination;
  @Input() maxPageSize  = 50;
  @Input() maxResultCount = 1000000;
  @Output() currentPageChanged = new EventEmitter();

  pageCount: number;
  pages: Array<number> = [];
  showPaging = true;

  constructor(
    private filterService: FilterService,
    private routeEventsService: RouteEventsService,
    private settings: SystemSettingsService
  ) {}

  ngOnInit(): void {
    this.setResultCount(this.vm.totalResults, this.vm.currentPage);

    // Force the page size dropdown to have a value after the app was loaded
    // using the browser's back button.
    setTimeout(() => {
      const pageSizeDropDown = (document.getElementById("resultsPerPage") as HTMLSelectElement)
      if (pageSizeDropDown && !pageSizeDropDown.value) {
        pageSizeDropDown.value = this.vm.resultsPerPage.toString();
      }
    },
    0);
  }

  setResultCount(newCount: number, newCurrentPage = 1): void {
    let countChanged = false;

    //If the total results changed, we need to reset to page one and re-calculate the number of pages
    if (this.vm.totalResults != newCount && this.vm.totalResults != 0) {
      //Do NOT reset the paging if the user came from the job details page
      if (
        this.routeEventsService.oldUrl.toLowerCase().indexOf('job-details') == -1 &&
        this.vm.currentPage !== newCurrentPage
      ) {
        this.vm.currentPage = newCurrentPage;
        countChanged = true;

        if (this.updateUrl) {
          //update url params
          let redirect = this.filterService.getRedirect(
            'page',
            this.vm.currentPage,
            true
          );
          if (this.vm.currentPage === 1) {
            redirect = this.filterService.clearParam('page', redirect);
          }

          //update page in the url
          this.filterService.location.go(redirect);
        }
      }
    }

    //reset - go back to page one
    if (this.vm.totalResults !== newCount) {
      this.vm.totalResults = newCount;
    }
    this.pageCount = Math.ceil(this.cappedResultCount() / this.vm.resultsPerPage);

    this.updatePagingControls();

    //toggle paging based on results
    this.showPaging = this.vm.totalResults > this.vm.resultsPerPage;

    //if the user added/removed filters we need to update the url
    if (countChanged) {
      this.setPageUrl();
    }
  }

  //update number of results shown on the page
  onChangeResultsPerPage(newValue: number): void {
    //update object
    this.vm.resultsPerPage = newValue;
    this.pageCount = Math.ceil(this.cappedResultCount() / this.vm.resultsPerPage);

    this.goToFirst();
  }

  updatePagingControls(): void {
    //setup
    const _pagesVisible = 5;
    const _pagesVisibleAfterSelected = 2;
    const _pagesThreshold = 2;

    //refresh pagecount
    this.pageCount = Math.ceil(this.cappedResultCount() / this.vm.resultsPerPage);

    //check for the first two pages
    if (this.vm.currentPage <= _pagesThreshold) {
      this.pages = new Array<number>();
      for (let i = 0; i < this.pageCount && i < _pagesVisible; i++) {
        this.pages.push(i + 1);
      }
    }
    //check for the last two pages
    else if (Number(this.vm.currentPage) + _pagesThreshold >= this.pageCount) {
      this.pages = new Array<number>();
      for (let i = this.pageCount - _pagesVisible; i < this.pageCount; i++) {
        if (i + 1 > 0) {
          this.pages.push(i + 1);
        }
      }
    } else {
      //middle pages

      //test to see if we should update the numbers on screen
      if (
        this.pages.indexOf(
          this.vm.currentPage - 0 + (_pagesVisibleAfterSelected - 0)
        )
      ) {
        //Using the "-0" to force numeric values
        //else its joining the totals and not adding them
        if (
          this.pageCount >=
          this.vm.currentPage - 0 + (_pagesVisibleAfterSelected - 0)
        ) {
          //reset the page number array
          this.pages = new Array<number>();

          //set starting page
          let startingPage = this.vm.currentPage - _pagesVisible;

          if (startingPage - _pagesThreshold < 1) {
            startingPage = -_pagesThreshold + 1;
          }

          //show the next page numbers in line
          for (
            let i = startingPage;
            i < this.pageCount && i <= this.vm.currentPage;
            i++
          ) {
            this.pages.push(i + _pagesThreshold);
          }

          //remove unecessary items
          if (this.pages.length > 5) {
            const numberToRemove = this.pages.length - _pagesVisible;
            this.pages.splice(0, numberToRemove);
          }
        }
      }
    }
  }

  //change page number currently visible
  changePage(page): void {
    //update page number
    if (this.vm.currentPage !== page) {
      this.vm.currentPage = page;
    }

    //update paging controls
    this.updatePagingControls();

    this.setUrlAndUpdateResults();
  }

  goToFirst(): void {
    //go to first page
    this.vm.currentPage = 1;

    //reset the page numbers in the pagination to start with 1
    this.pages = new Array<number>();
    for (let i = 0; i < this.pageCount && i < 5; i++) {
      this.pages.push(i + this.vm.currentPage);
    }

    this.setUrlAndUpdateResults();
  }

  goToLast() {
    //go to the last page
    this.vm.currentPage = Math.ceil(
      this.cappedResultCount() / this.vm.resultsPerPage
    );
    this.pageCount = this.vm.currentPage;

    //reset the page number array
    this.pages = new Array<number>();

    //update page numbers
    //show the next page numbers in line
    for (
      let i = this.vm.currentPage - 5;
      i <= this.pageCount && i <= this.vm.currentPage;
      i++
    ) {
      //check for negative numbers - do not add them to the array as they will result in negative numbers in the pagin
      if (i > 0) {
        this.pages.push(i);
      }
    }

    this.setUrlAndUpdateResults();
  }

  goToNext() {
    //Go to the next page number
    this.vm.currentPage++;

    //update page numbers
    this.changePage(this.vm.currentPage);
  }

  //go to previous result page
  goToPrevious() {
    //update local variable
    this.vm.currentPage--;

    if (this.vm.currentPage == 1) {
      this.goToFirst();
    } else {
      //update page numbers
      this.changePage(this.vm.currentPage);
    }
  }

  private setUrlAndUpdateResults() {
    this.currentPageChanged.emit();

    //set paging params
    this.setPageUrl();

    //update page results
    this.updateResults();
  }

  //"refresh" results on screen
  updateResults() {
    //hide paging if there is only one page results
    this.showPaging = this.vm.totalResults > this.vm.resultsPerPage;

    //scroll to top
    //el.scrollIntoView does not want to work for some reason, so reverted to window.scrollTo
    window.scrollTo({ top: 0 });
  }

  setPageUrl() {
    if (this.updateUrl) {
      //get URL
      let redirect = this.filterService.getRedirect(
        'page',
        this.vm.currentPage,
        false
      );
      if (this.vm.currentPage === 1) {
        redirect = this.filterService.clearParam('page', redirect);
      }

      //remove page size param
      redirect = this.filterService.clearParam('pagesize', redirect);

      //add page-size param
      if (this.vm.resultsPerPage !== 20) {
        redirect = this.filterService.addParam(
          'pagesize',
          this.vm.resultsPerPage,
          redirect,
          false
        );
      }

      //trigger observerable to reload
      this.filterService.setFilters();

      //redirect to page with params
      this.filterService.location.go(redirect);
    }
  }

  maxSize(check: number) {
    return this.maxPageSize >= check;
  }

  cappedResultCount() {
    if (this.vm.totalResults > this.maxResultCount) {
      return this.maxResultCount;
    }
    return this.vm.totalResults;
  }

  get cappedResultsWarning(): string {
    return this.settings.jbSearch.errors.cappedResultsWarning
      .replace('{0}', this.maxResultCount.toLocaleString('en'));
  }
}
