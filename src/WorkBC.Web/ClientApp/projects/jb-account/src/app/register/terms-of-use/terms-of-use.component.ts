import { Component, ViewEncapsulation } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { SystemSettingsService } from '../../../../../jb-lib/src/public-api';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-terms-of-use',
  templateUrl: './terms-of-use.component.html',
  styleUrls: ['./terms-of-use.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class TermsOfUseComponent {
  constructor(
    private dialogRef: MatDialogRef<TermsOfUseComponent>,
    private settings: SystemSettingsService,
    private sanitizer: DomSanitizer
  ) {}

  get termsOfUseTitle(): string {
    return this.settings.jbAccount.registration.termsOfUseTitle;
  }

  get termsOfUseText(): SafeHtml {
    const terms = new DOMParser().parseFromString(
      this.settings.jbAccount.registration.termsOfUseText,
      'text/html'
    );
    return this.sanitizer.bypassSecurityTrustHtml(terms.body.innerHTML);
  }

  close(): void {
    this.dialogRef.close();
  }
}
