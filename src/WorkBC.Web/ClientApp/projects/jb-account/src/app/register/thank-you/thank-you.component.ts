import { Component, Input } from '@angular/core';
import { UserService } from '../../services/user.service';
import { LoggingService } from '../../services/logging.service';
import { ToastrService } from 'ngx-toastr';
import { SystemSettingsService } from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-thank-you',
  templateUrl: './thank-you.component.html',
  styleUrls: ['./thank-you.component.scss'],
})
export class ThankYouComponent {
  @Input() email: string;

  constructor(
    private userService: UserService,
    private loggingService: LoggingService,
    private settings: SystemSettingsService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    window.scrollTo(0, 0);
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
