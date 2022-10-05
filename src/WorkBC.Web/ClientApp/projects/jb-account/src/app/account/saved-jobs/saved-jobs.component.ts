import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  StorageService,
  Job,
  PaginationComponent,
  DataService,
  JobService,
  PaginationModel,
  FilterService,
  GlobalService
} from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-saved-jobs',
  templateUrl: './saved-jobs.component.html',
  styleUrls: ['./saved-jobs.component.scss'],
})
export class SavedJobsComponent implements OnInit, AfterViewInit {
  loading = false;
  results: Job[] = [];
  savedJobList: Job[] = [];
  sortOrder: number;
  paginationModel = new PaginationModel();

  @ViewChild('pagination')
  paginationElement: PaginationComponent;

  constructor(
    private storageService: StorageService,
    private jobService: JobService,
    private route: ActivatedRoute,
    private filterService: FilterService,
    private dataService: DataService,
    private globalService: GlobalService
  ) {}

  ngOnInit(): void {
    this.loading = true;

    this.sortOrder = +sessionStorage.getItem('savedJobsSortOrder') || 11;

    this.jobService.getSavedJobs().subscribe((savedJobs) => {
      if (savedJobs && savedJobs.length) {
        this.results = this.jobService.getJobs(savedJobs);
        this.savedJobList = [...this.results];
        this.sortData();

        this.storageService.setLocalStorageItem(
          this.storageService.savedJobsKey,
          this.results.map((x) => x.JobId).join()
        );

        this.route.paramMap.pipe().subscribe((params) => {
          const pagesize = +params.get('pagesize');
          if (pagesize && this.paginationModel.resultsPerPage !== pagesize) {
            this.paginationModel.resultsPerPage = pagesize;
          }

          const page = +params.get('page');
          if (page && this.paginationModel.currentPage !== page) {
            this.paginationModel.currentPage = page;
          }

          this.setPageResult();
        });

        this.paginationElement.setResultCount(this.savedJobList.length);
      }
      this.loading = false;
    });
  }

  ngAfterViewInit(): void {
    //setTimeout(() => {
    //  this.paginationElement.setResultCount(this.totalSavedJobs);
    //});
  }

  onCurrentPageChanged(): void {
    this.setPageResult();
  }

  private setPageResult(): void {
    const start =
      (this.paginationModel.currentPage - 1) *
      this.paginationModel.resultsPerPage;
    const remaining = this.savedJobList.length - start;
    const end =
      start +
      (this.paginationModel.resultsPerPage > remaining
        ? remaining
        : +this.paginationModel.resultsPerPage);
    this.results = this.savedJobList.slice(start, end);
  }

  get jbSearchUrl(): string {
    return this.globalService.getJbSearchUrl();
  }

  get totalSavedJobs(): number {
    return this.storageService.totalSavedJobs;
  }

  sortByChange(): void {
    if (this.paginationModel.currentPage > 1) {
      this.paginationModel.currentPage = 1;
      this.setRedirect();
    }
    sessionStorage.setItem('savedJobsSortOrder', this.sortOrder.toString());
    this.sortData();
    this.setPageResult();
  }

  private setRedirect(): void {
    let redirect = this.filterService.iniRedirect;
    redirect = this.filterService.clearParam('page', redirect);
    this.filterService.goLocation(redirect, true);
  }

  private sortData(): void {
    switch (+this.sortOrder) {
      case 1:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.savedJob.datePosted > b.savedJob.datePosted ? -1 : 1
        );
        break;
      case 2:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.savedJob.datePosted > b.savedJob.datePosted ? 1 : -1
        );
        break;
      case 3:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.Title.toLowerCase() > b.Title.toLowerCase() ? 1 : -1
        );
        break;
      case 4:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.Title.toLowerCase() > b.Title.toLowerCase() ? -1 : 1
        );
        break;
      case 5:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.City.toLowerCase() > b.City.toLowerCase() ? 1 : -1
        );
        break;
      case 6:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.City.toLowerCase() > b.City.toLowerCase() ? -1 : 1
        );
        break;
      case 7:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.EmployerName.toLowerCase() > b.EmployerName.toLowerCase() ? 1 : -1
        );
        break;
      case 8:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.EmployerName.toLowerCase() > b.EmployerName.toLowerCase() ? -1 : 1
        );
        break;
      case 9:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          (a.savedJob.salary ? a.savedJob.salary : 99999999) >
          (b.savedJob.salary ? b.savedJob.salary : 99999999)
            ? 1
            : -1
        );
        break;
      case 10:
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.savedJob.salary > b.savedJob.salary ? -1 : 1
        );
        break;
      default:
        // 11
        this.savedJobList = this.savedJobList.sort((a, b) =>
          a.savedJob.id > b.savedJob.id ? -1 : 1
        );
    }
  }

  private removeTheSavedJob(jobId: string): void {
    const savedJobs = this.storageService.getLocalStorageItem(
      this.storageService.savedJobsKey
    );
    if (savedJobs) {
      const savedJobArray = savedJobs.split(',');
      if (savedJobArray.indexOf(jobId) > -1) {
        savedJobArray.splice(savedJobArray.indexOf(jobId), 1);
        this.storageService.setLocalStorageItem(
          this.storageService.savedJobsKey,
          savedJobArray.join()
        );

        this.savedJobList = this.savedJobList.filter((x) => x.JobId !== jobId);

        this.paginationElement.setResultCount(
          this.savedJobList.length,
          this.results.length - 1 === 0
            ? this.paginationModel.currentPage - 1
            : this.paginationModel.currentPage
        );

        this.setPageResult();
      }
    }
  }

  onDeleted(job: Job): void {
    this.jobService.deleteSavedJob(job.JobId).subscribe(() => {
      this.removeTheSavedJob(job.JobId);
    });
  }
}
