import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { BaseComponent } from '../../models/base-component.model';
import { SystemSettingsService } from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.scss']
})
export class PasswordResetComponent extends BaseComponent implements OnInit {
  email: string;

  code: string;
  continueBtnClicked: boolean;
  isVerified: boolean;
  badToken = false;
  emailSent = false;

  newPassword: string;
  confirmedPassword: string;
  resetPwdClicked: boolean;
  succeeded: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private settings: SystemSettingsService,
    private userService: UserService) {
      super();
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(
      (params: ParamMap) => {
        if (!this.email) {
          this.email = params.get('email');
        }

        this.code = params.get('code');
        if (this.code) {
          if (this.code !== '0') {
            this.continue();
          } else {
            this.code = '';
            this.emailSent = true;
          }
        }

        if (this.emailSent) {
          // scroll to the top of the page
          setTimeout(() => { scrollTo(0, 0); }, 250);
        }
      }
    );
  }

  get passwordComplexity(): string {
    return this.settings.jbAccount.shared.passwordComplexity;
  }

  get invalidTokenMessage(): string {
    return this.settings.jbAccount.errors.forgotPasswordInvalidToken;
  }

  get confirmationTitle(): string {
    return this.settings.jbAccount.login.forgotPasswordConfirmationTitle;
  }

  get confirmationBody(): string {
    const result = this.settings.jbAccount.login.forgotPasswordConfirmationBody;
    return result.replace('{0}', this.email);
  }

  codeChange(): void {
    if (this.continueBtnClicked) {
      this.continueBtnClicked = false;
    }
  }

  passwordChange(): void {
    if (this.resetPwdClicked) {
      this.resetPwdClicked = false;
    }
  }

  continue(): void {
    this.continueBtnClicked = true;

    if (this.email && this.code) {
      const data = { email: this.email, token: this.code };
      this.userService.verifyToken(data).subscribe(isVerified => {
        this.isVerified = isVerified;
        if (!isVerified) {
          this.badToken = true;
        }
      });
    }
  }

  resetPassword(): void {
    this.resetPwdClicked = true;

    if (this.newPassword && this.confirmedPassword && this.newPassword === this.confirmedPassword) {
      const data = { email: this.email, token: this.code, newPassword: this.newPassword };
      this.userService.resetPassword(data).subscribe(result => {
        this.succeeded = result.succeeded;

        if (this.succeeded) {
          // scroll to the top of the page
          setTimeout(() => { scrollTo(0, 0); }, 250);
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 5000);
        }
      });
    }
  }

  get confirmPasswordError(): boolean {
    return (this.newPassword !== this.confirmedPassword) && this.confirmedPassword && this.resetPwdClicked;
  }
}
