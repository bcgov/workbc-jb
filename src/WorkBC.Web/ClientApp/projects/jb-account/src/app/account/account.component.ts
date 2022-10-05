import { Component, OnInit, Input } from '@angular/core';
import {
  AuthenticationService,
  StorageService,
  JobService
} from '../../../../jb-lib/src/public-api';
import { CareerProfileService } from '../services/career-profile.service';
import { IndustryProfileService } from '../services/industry-profile.service';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent implements OnInit {
  @Input() activeMenu = '';
  @Input() jobAlerts = 0;
  @Input() totalSavedCareerProfiles = 0;
  @Input() totalSavedIndustryProfiles = 0;

  mobileCollapsed = true;
  jobsCollapsed = true;
  savedCollapsed = true;
  manageCollapsed = true;

  currentUser = '';
  totalRecommendedJobs = 0;

  constructor(
    private authenticationService: AuthenticationService,
    private jobService: JobService,
    private careerProfileService: CareerProfileService,
    private industryProfileService: IndustryProfileService,
    private storageService: StorageService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUser.firstName;

    this.jobService
      .getJobAlertsTotal()
      .subscribe((result) => (this.jobAlerts = result));

    this.jobService
      .getRecommendedJobsTotal()
      .subscribe((result) => (this.totalRecommendedJobs = result));

    this.careerProfileService.careerProfilesTotal.subscribe((result) => {
      this.totalSavedCareerProfiles = result;
    });

    this.industryProfileService.industryProfilesTotal.subscribe((result) => {
      this.totalSavedIndustryProfiles = result;
    });
  }

  clearRecommendedJobsState() {
    sessionStorage.removeItem('recommendedJobsState');
  }

  clearSavedJobsPageState() {
    sessionStorage.removeItem('savedJobsSortOrder');
  }

  get totalSavedJobs(): number {
    return this.storageService.totalSavedJobs;
  }
}
