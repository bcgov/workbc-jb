import { Component, Input } from '@angular/core';
import { StorageService } from '../../services/storage.service';
import { AuthenticationService } from '../../services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import { Job } from '../../filters/models/job.model';
import { JobService } from '../../services/job.service';
import { GlobalService } from '../../services/global.service';

@Component({
  selector: 'lib-jb-a-link',
  templateUrl: './jb-a-link.component.html',
  styleUrls: ['./jb-a-link.component.css']
})
export class JbALinkComponent {
  @Input() jobId: string;
  @Input() job: Job;
  @Input() isEnglish = true;

  constructor(
    private storageService: StorageService,
    private jobService: JobService,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    private globalService: GlobalService
  ) { }

  get saveLabel(): string {
    let result = this.isEnglish ? 'Save' : 'Sauvegarder';
    const savedJobs = this.storageService.getLocalStorageItem(this.storageService.savedJobsKey);
    if (savedJobs && savedJobs.split(',').indexOf(this.jobId) !== -1 && this.authenticationService.currentUser) {
      result = this.saved;
    }
    return result;
  }

  private get saved(): string {
    return this.isEnglish ? 'Saved' : 'Enregistrée';
  }

  private setLocalStorageItem(key: string, jobId: string): void {
    const savedJobs = this.storageService.getLocalStorageItem(key);
    if (!savedJobs) {
      this.storageService.setLocalStorageItem(key, jobId);
    } else if (savedJobs.split(',').indexOf(jobId) === -1) {
      this.storageService.setLocalStorageItem(key, savedJobs + ',' + jobId);
    }
  }

  //private saveTmpData() {
  //  let tmpData = this.storageService.tmpData;
  //  if (tmpData.length === 0) {
  //    this.storageService.setLocalStorageItem("tmpData", JSON.stringify([this.job]));
  //  } else {
  //    tmpData.push(this.job);
  //    this.storageService.setLocalStorageItem("tmpData", JSON.stringify(tmpData));
  //  }
  //}

  saveJob(jobId: string, event: MouseEvent): void {
    event.preventDefault();
    event.stopPropagation();

    if (!this.isDisabled) {
      //this.saveTmpData();

      const currentUser = this.authenticationService.currentUser;
      if (currentUser) {
        this.jobService.saveJobs(jobId).subscribe(() => {
          this.setLocalStorageItem(this.storageService.savedJobsKey, jobId);
          this.toastr.success('The job has been saved successfully');
        });
      } else {
        this.setLocalStorageItem(this.storageService.tmpSavedJobsKey, jobId);
        document.location.href = this.globalService.getJbAccountUrl();
      }
    }
  }

  get isDisabled(): boolean {
    return this.saveLabel === this.saved;
  }
}
