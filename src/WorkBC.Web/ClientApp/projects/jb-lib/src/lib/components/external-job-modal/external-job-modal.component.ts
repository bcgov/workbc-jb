import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-external-job-modal',
  templateUrl: './external-job-modal.component.html',
  styleUrls: ['./external-job-modal.component.scss']
})
export class ExternalJobModalComponent {

  public jobName: string;
  public jobOrigin: string;
  public jobUrl: string;

  constructor(public activeModal: NgbActiveModal) {

  }
}
