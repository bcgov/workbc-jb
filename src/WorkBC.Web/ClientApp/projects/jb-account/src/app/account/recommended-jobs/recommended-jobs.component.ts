import { Component, OnInit, ViewChild } from '@angular/core';
import {
  Job,
  JobService,
  RecommendationFilterVm,
  RecommendedJobFilter,
  RecommendedJobResult,
  RecommendedJob,
  PaginationModel,
  RecommendationFilterInput,
  StorageService,
  PaginationComponent,
  SystemSettingsService
} from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-recommended-jobs',
  templateUrl: './recommended-jobs.component.html',
  styleUrls: ['./recommended-jobs.component.scss']
})
export class RecommendedJobsComponent implements OnInit {
  filterInput: RecommendationFilterInput;
  jobs: Job[] = [];
  filter: RecommendedJobFilter;
  loading = false;
  paginationModel = new PaginationModel();
  totalRecommendedJobs: number = null;

  @ViewChild('pagination')
  paginationElement: PaginationComponent;

  constructor(
    private jobService: JobService,
    private settings: SystemSettingsService,
    private storageService: StorageService
  ) {}

  ngOnInit(): void {
    // get saved filter from Session Storage
    const state = JSON.parse(sessionStorage.getItem('recommendedJobsState'));
    if (state) {
      const pageSize = '' + state.pageSize;
      const page = '' + state.page;
      this.filter = new RecommendedJobFilter(+page, +pageSize, state.sortOrder, null, state);
      setTimeout(() => {
        this.paginationModel.resultsPerPage = +pageSize;
        this.paginationModel.currentPage = +page;
      }, 0);
    } else {
      this.filter = new RecommendedJobFilter();
    }

    this.setData();

    this.jobService
      .getRecommendedJobsTotal()
      .subscribe((result) => (this.totalRecommendedJobs = result));
  }

  get introText(): string {
    return this.settings.jbAccount.recommendedJobs.introText;
  }

  get introTextNoRecommendedJobs(): string {
    return this.settings.jbAccount.recommendedJobs.introTextNoRecommendedJobs;
  }

  get showNoResults(): boolean {
    return !this.filteredJobCount && !!this.totalRecommendedJobs;
  }

  get recommendedJobsNoResults(): string {
    if (!this.showNoResults) return '';

    return this.filterCount === 1 ?
      this.settings.jbAccount.errors.recommendedJobsNoResultsOneCheckbox :
      this.settings.jbAccount.errors.recommendedJobsNoResultsMultipleCheckboxes;
  }

  private getJobs(recommendedJobs: RecommendedJob[]): Job[] {
    const result: Job[] = [];
    for (const recommendedJob of recommendedJobs) {
      result.push(new Job(null, recommendedJob));
    }
    return result;
  }

  private setData(): void {
    this.loading = true;

    // save the filter in session storage
    sessionStorage.setItem('recommendedJobsState', JSON.stringify(this.filter));

    this.jobService
      .getRecommendedJobs(this.filter)
      .subscribe((data: RecommendedJobResult<RecommendedJob>) => {
        this.jobs = data ? this.getJobs(data.result) : [];
        this.paginationModel.totalResults = data ? data.count : 0;
        this.paginationElement.setResultCount(this.paginationModel.totalResults);
        this.filterInput = new RecommendationFilterInput(
          data.city,
          this.storageService.totalSavedJobs,
          data.jobSeekerFlags
        );
        this.loading = false;
      });
  }

  get filterCount(): number {
    return this.filter.filterCount;
  }

  get filteredJobCount(): number {
    return this.paginationModel.totalResults;
  }

  onFilterApplied(vm: RecommendationFilterVm): void {
    this.filter.updateReasons(vm);
    this.sortByChange();
  }

  sortByChange(): void {
    if (this.paginationModel.currentPage !== 1) {
      this.paginationModel.currentPage = 1;
    }

    this.setCurrentPage();
    this.setData();
  }

  onCurrentPageChanged(): void {
    this.setCurrentPage();

    if (this.filter.pageSize != this.paginationModel.resultsPerPage) {
      this.filter.pageSize = this.paginationModel.resultsPerPage;
    }

    this.setData();
  }

  private setCurrentPage(): void {
    if (this.filter.page != this.paginationModel.currentPage) {
      this.filter.page = this.paginationModel.currentPage;
    }
  }
}
