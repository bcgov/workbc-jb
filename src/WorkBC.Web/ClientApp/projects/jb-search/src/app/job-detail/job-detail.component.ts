import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Job, TextHeader, DataService, GlobalService, SystemSettingsService } from '../../../../jb-lib/src/public-api';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SocialSharingComponent } from './social-sharing/social-sharing.component';

@Component({
  selector: 'app-job-detail',
  templateUrl: './job-detail.component.html',
  styleUrls: ['./job-detail.component.scss']
})
export class JobDetailComponent implements OnInit {

  private jobId: number;
  public jobDetail: Job;
  public textHeaders: TextHeader;
  public jobLanguage: boolean;

  fromRecommendedJobs = false;
  fromSavedJobs = false;

  constructor(
    private dataService: DataService,
    private route: ActivatedRoute,
    private location: Location,
    private globalService: GlobalService,
    private settings: SystemSettingsService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.jobId = +params['id']; // (+) converts string 'id' to a number

      this.fromSavedJobs = params['sj'] && params['sj'].toLowerCase() === 'true';
      this.fromRecommendedJobs = params['rj'] && params['rj'].toLowerCase() === 'true';
      
      if (this.jobId > 0) {
        this.getJobDetails('en', false);       
      }

    });
  }

  get callToAction1Intro(): string {
    return this.settings.jbSearch.jobDetail.callToAction1Intro;
  }
  get callToAction1Title(): string {
    return this.settings.jbSearch.jobDetail.callToAction1Title;
  }
  get callToAction1BodyText(): string {
    return this.settings.jbSearch.jobDetail.callToAction1BodyText;
  }
  get callToAction1LinkText(): string {
    return this.settings.jbSearch.jobDetail.callToAction1LinkText;
  }
  get callToAction1LinkUrl(): string {
    return this.settings.jbSearch.jobDetail.callToAction1LinkUrl;
  }

  get callToAction2Intro(): string {
    return this.settings.jbSearch.jobDetail.callToAction2Intro;
  }
  get callToAction2Title(): string {
    return this.settings.jbSearch.jobDetail.callToAction2Title;
  }
  get callToAction2BodyText(): string {
    return this.settings.jbSearch.jobDetail.callToAction2BodyText;
  }
  get callToAction2LinkText(): string {
    return this.settings.jbSearch.jobDetail.callToAction2LinkText;
  }
  get callToAction2LinkUrl(): string {
    return this.settings.jbSearch.jobDetail.callToAction2LinkUrl;
  }

  get printRoot(): string {
    return this.globalService.getApiBaseUrl();
  }

  toggleLanguage() {
    if (this.jobLanguage) {
      this.getJobDetails('fr', true);
    }
    else {
      this.getJobDetails('en', true);
    }
  }

  getJobDetails(language, isToggle) {
    this.dataService.getJobDetail(this.jobId, language, isToggle).subscribe(job => {
      job.result[0].Lang = language;
      this.jobDetail = new Job(null, null, job.result[0]);
      this.textHeaders = job.textHeaders;
    });
  }

  goBack() {
    this.location.back();
  }

  get from(): string {
    return this.fromSavedJobs ? 'Saved Jobs' : (this.fromRecommendedJobs ? 'Recommended Jobs' : 'Job Search Results');
  }

  get fromJobSearch(): boolean {
    return !this.fromSavedJobs && !this.fromRecommendedJobs;
  }

  share(event) {
    event.preventDefault();
    const modalRef = this.modalService.open(SocialSharingComponent, { container: 'app-root', centered: true });
    modalRef.componentInstance.title = this.jobDetail.Title;
    modalRef.componentInstance.jobId = this.jobDetail.JobId;
  }
}
