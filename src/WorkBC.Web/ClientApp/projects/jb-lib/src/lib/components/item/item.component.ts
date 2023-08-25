import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ExternalJobModalComponent } from '../external-job-modal/external-job-modal.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { XmlJobModalComponent } from '../xml-job-modal/xml-job-modal.component';
import { Job } from '../../filters/models/job.model';
import { GlobalService } from '../../services/global.service';
import { MatDialog } from '@angular/material/dialog';
import { SimpleDialogComponent } from '../simple-dialog/simple-dialog.component';
import { SavedJobNoteComponent } from '../saved-job-note/saved-job-note.component';
import { SavedJobNoteModel } from '../../models/saved-job-note.model';
import { AuthenticationService } from '../../services/authentication.service';
import { SystemSettingsService } from '../../services/system-settings.service';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss']
})
export class ItemComponent {
  @Input() item: Job;
  @Input() inSavedJobsView = false;
  @Input() inRecommendedJobsView = false;
  @Output() deleted = new EventEmitter<Job>();

  //XML returned for a specific job
  xml: string;

  //Modal property
  closeResult: string;

  constructor(
    private modalService: NgbModal,
    private http: HttpClient,
    private globalService: GlobalService,
    private authenticationService: AuthenticationService,
    private dialog: MatDialog,
    private settings: SystemSettingsService
  ) {
  }

  get icon(): string {
    return this.inSavedJobsView ? 'pencil' : 'save';
  }

  get iconWidthOrHeight(): string {
    return this.inSavedJobsView ? '14px' : '12px';
  }

  get isExpired(): boolean {
    let result = false;
    if (this.item) {
      result = this.inSavedJobsView && !this.item.IsActive;
      if (!result && this.item.ExpireDate) {
        const today = new Date();
        //const offSet = new Date().getTimezoneOffset();
        //const offSetHours = offSet / 60 * -1;
        //const offSetMinutes = offSet % 60;
        //console.log("Today:" + today);
        today.setHours(23, 59, 0, 0);
        const expireDate = new Date(this.item.ExpireDate);
        //expireDate.setHours(offSetHours, offSetMinutes, 0, 0);
        //console.log(expireDate, today);
        result = expireDate < today;
      }
    }
    return result;
  }

  get addOrView(): string {
    return this.item.Note ? 'View' : 'Add';
  }

  get printRoot(): string {
    return this.globalService.getApiBaseUrl();
  }

  get jobDetailsUrl(): string {
    return this.jbSearchRoot + '#/job-details/' + this.item.JobId +
      (this.inSavedJobsView ? ';sj=true' : (this.inRecommendedJobsView ? ';rj=true' : ''));
  }

  get jbSearchRoot(): string {
    return this.globalService.getJbSearchUrl();
  }

  get isProduction(): boolean {
    return this.settings.shared.settings.isProduction;
  }

  openModal(event: MouseEvent): void {
    if (!this.isExpired) {
      event.preventDefault();
      event.stopPropagation();
      const modalRef = this.modalService.open(ExternalJobModalComponent, {
        container: 'app-root',
        size: 'lg',
        centered: true
      });
      modalRef.componentInstance.jobUrl = this.item.ExternalSource.Source[0].Url;
      modalRef.componentInstance.jobName = this.item.Title;
      modalRef.componentInstance.jobOrigin = this.item.ExternalSource.Source[0].Source;
    }
  }

  open(jobId) {
    if (!this.isExpired) {
      this.http
        .get<string>(
          this.globalService.getApiBaseUrl() +
          'api/Search/GetJobXml?jobId=' +
          jobId
        )
        .subscribe(result => {
          this.xml = result;

          const modalRefXml = this.modalService.open(XmlJobModalComponent, {
            container: 'app-root',
            size: 'lg',
            centered: true
          });
          modalRefXml.componentInstance.jobXml = this.xml;
          modalRefXml.componentInstance.jobId = jobId;
        });
    }
  }

  addOrViewNote(event: Event) {
    event.preventDefault();
    if (!this.isExpired || this.item.Note) {
      this.dialog.open(SavedJobNoteComponent, {
        data: {
          jobId: this.item.JobId,
          isExpired: this.isExpired,
          savedJobNoteModel: new SavedJobNoteModel(this.item.JobId, '', this.item.Note)
        },
        width: '85%',
        maxWidth: 500
      }).afterClosed().subscribe((result: { updatedNote: string }) => {
        if (result) {
          this.item.Note = result.updatedNote;
        }
      });
    }
  }

  get isDisabled(): boolean {
    return this.isExpired && !this.item.Note;
  }

  deleteJob(event: Event) {
    event.preventDefault();
    //event.stopPropagation();

    this.dialog.open(SimpleDialogComponent, {
      data: { title: 'DELETE JOB', btnLabel: 'Delete', content: 'Are you sure you want to delete this job?' },
      width: '85%',
      maxWidth: 500
    }).afterClosed().subscribe(result => {
      if (result) {
        this.deleted.emit(this.item);
      }
    });
  }
}
