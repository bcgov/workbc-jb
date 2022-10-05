import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-social-sharing',
  templateUrl: './social-sharing.component.html',
  styleUrls: ['./social-sharing.component.scss']
})
export class SocialSharingComponent {

  constructor(public activeModal: NgbActiveModal) { }

  @Input() title;

  @Input() jobId;

  @Input() buttonLabel: string;

  close() {
    this.activeModal.close('Close click');
  }

  get capitalTitle(): string {
    return this.title.charAt(0).toUpperCase() + this.title.slice(1);
  }
}
