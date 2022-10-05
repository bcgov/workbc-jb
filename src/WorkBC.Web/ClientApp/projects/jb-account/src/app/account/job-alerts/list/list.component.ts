import { Component, OnInit, ViewChild } from '@angular/core';
import { JobService, JobAlertModel, PaginationModel, PaginationComponent, SimpleDialogComponent, GlobalService } from '../../../../../../jb-lib/src/public-api';
import { map } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

  jobAlerts: JobAlertModel[] = [];
  allJobAlerts: JobAlertModel[] = [];
  loading = false;
  paginationModel: PaginationModel;

  @ViewChild('pagination') paginationElement: PaginationComponent;

  constructor(
    private jobService: JobService,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private globalService: GlobalService
  ) { }

  ngOnInit(): void {
    //if (this.jobAlerts.length > 0) {
      this.loading = true;

      this.paginationModel = new PaginationModel();

      this.jobService.getJobAlerts().pipe(
        map(x => x.map(y =>
          new JobAlertModel(y.title, y.alertFrequency, y.urlParameters, y.jobSearchFilters, y.id, this.jobService)
        ))
      ).subscribe(result => {
        this.jobAlerts = result;
        this.allJobAlerts = [...this.jobAlerts];

        this.route.paramMap.pipe(
        ).subscribe(params => {
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
        this.paginationElement.setResultCount(this.allJobAlerts.length);

        this.loading = false;
      });
    //}
  }

  onCurrentPageChanged(): void {
    this.setPageResult();
  }

  private setPageResult(): void {
    const start = (this.paginationModel.currentPage - 1) * this.paginationModel.resultsPerPage;
    const remaining = this.allJobAlerts.length - start;
    const end = start + (this.paginationModel.resultsPerPage > remaining ? remaining : +this.paginationModel.resultsPerPage);
    this.jobAlerts = this.allJobAlerts.slice(start, end);
  }

  deleteJobAlert(event: Event, jobAlert: JobAlertModel): void {
    event.preventDefault();
    this.dialog.open(SimpleDialogComponent, {
      data: { title: 'DELETE ALERT', btnLabel: 'Delete', content: 'Are you sure you want to delete this alert?' },
      width: '85%',
      maxWidth: 500
    }).afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.jobService.deleteJobAlert(jobAlert.id)
          .subscribe(() => {
            this.allJobAlerts = this.allJobAlerts.filter(x => x.id !== jobAlert.id);

            this.paginationElement.setResultCount(
              this.allJobAlerts.length,
              this.jobAlerts.length - 1 === 0
                ? this.paginationModel.currentPage - 1
                : this.paginationModel.currentPage
            );

            this.setPageResult();
          });
      }
    });
  }

  getSearchUrl(urlParameters: string): string {
    const urlParams = urlParameters.indexOf('#') !== -1 ? urlParameters.replace(/#/g, '%23') : urlParameters;
    return `${this.globalService.getJbSearchUrl()}#/job-search${urlParams}`;
  }
}
