import { AfterViewInit, Component, ElementRef, Input, ViewChild } from '@angular/core';
import { UserService } from '../../services/user.service';
import { LoggingService } from '../../services/logging.service';
import { ToastrService } from 'ngx-toastr';
import { SystemSettingsService } from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-thank-you',
  templateUrl: './thank-you.component.html',
  styleUrls: ['./thank-you.component.scss'],
})
export class ThankYouComponent implements AfterViewInit {
  @Input() email: string;

  @ViewChild('thankYouHeading')
  private thankYouHeading: ElementRef<HTMLElement>;

  constructor(
    private userService: UserService,
    private loggingService: LoggingService,
    private settings: SystemSettingsService,
    private toastr: ToastrService
  ) {}

  ngAfterViewInit(): void {
    // Focus the heading, not the container: NVDA's browse cursor follows
    // focus to a tabindex="-1" heading (announcing its text), but not to a
    // live-region div — which left Tab jumping from the old buffer position
    // into the footer.
    //
    // Scroll explicitly instead of via focus() or window.scrollTo(0, 0):
    // the Drupal theme sets html { scroll-behavior: smooth }, so a
    // scroll-to-top races the focus-triggered scroll and can win, leaving
    // the heading below the fold.
    setTimeout(() => {
      const heading = this.thankYouHeading?.nativeElement;
      if (heading) {
        heading.focus({ preventScroll: true });
        heading.scrollIntoView({ block: 'center' });
      }
    });
  }

  get confirmationTitle(): string {
    return this.settings.jbAccount.registration.confirmationTitle;
  }

  get confirmationBody(): string {
    const result = this.settings.jbAccount.registration.confirmationBody;
    return result.replace('{0}', this.email);
  }

  resendEmail(): void {
    this.userService.sendActivationEmail(this.email).subscribe(() => {
      this.toastr.success(
        `An activation email has been sent to ${this.email}. Please follow the instructions to activate your account.`,
        'Activation Email Sent'
      );
      this.loggingService.log('Activation email has been successfully sent.');
    });
  }
}
