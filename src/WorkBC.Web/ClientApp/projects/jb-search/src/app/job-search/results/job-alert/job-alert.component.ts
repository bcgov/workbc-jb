import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import {
  FilterService,
  AuthenticationService,
  JobService,
  JobAlertModel,
  StorageService,
  SystemSettingsService,
  GlobalService
} from '../../../../../../jb-lib/src/public-api';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-job-alert',
  templateUrl: './job-alert.component.html',
  styleUrls: ['./job-alert.component.scss'],
})
export class JobAlertComponent implements OnInit {
  @Input() urlParams: string;

  private jobSearchFilters: string;
  submitted: boolean;

  error = '';
  title = '';
  alertFrequency = 1;
  showMore = false;
  activeFilters: Array<string> = []; // ["Job type: Part-time", "Job number: 604659", "Location: Vancouver", "Job title: software", "Job description: software", "Employer: software"];

  constructor(
    public activeModal: NgbActiveModal,
    private authenticationService: AuthenticationService,
    private jobService: JobService,
    private toastr: ToastrService,
    private storageService: StorageService,
    private settings: SystemSettingsService,
    private filterService: FilterService,
    private globalSevice: GlobalService
  ) {}

  ngOnInit(): void {
    this.filterService.mainFilterModels$.subscribe((mainFilterModel) => {
      this.activeFilters = mainFilterModel.activeFilters;
      this.jobSearchFilters = JSON.stringify(
        mainFilterModel.convertToElasticSearchJobSearchFilters()
      );
    });
  }

  get jbAccountUrl(): string {
    return this.globalSevice.getJbAccountUrl();
  }

  get MessageForRequiredJobAlertTitle(): string {
    return this.settings.shared?.errors.jobAlertTitleRequired;
  }

  get MessageForDuplicateJobAlertTitle(): string {
    return this.settings.shared.errors.jobAlertTitleDuplicate;
  }

  close(): void {
    this.activeModal.close('Close click');
  }

  onTitleChanged(): void {
    if (this.error) this.error = '';
  }

  createAlert(event: MouseEvent): void {
    this.submitted = true;

    if (!this.title || this.title.trim().length === 0) {
      event.preventDefault();
      return;
    }

    const jobAlertModel = new JobAlertModel(
      this.title,
      this.alertFrequency,
      this.urlParams,
      this.jobSearchFilters
    );

    const currentUser = this.authenticationService.currentUser;
    if (currentUser) {
      event.preventDefault();

      this.jobService.saveJobAlert(jobAlertModel).subscribe(
        () => {
          this.toastr.success(
            'Your Job Alert has successfully been added to your account.'
          );
          this.close();
        },
        (error: string) => {
          console.error(error);
          if (error && error.indexOf('title') !== -1) {
            this.error = this.MessageForDuplicateJobAlertTitle;
          }
        }
      );
    } else {
      this.storageService.setLocalStorageItem(
        this.storageService.tmpJobAlertKey,
        JSON.stringify(jobAlertModel)
      );
      this.close();
    }
  }
}
