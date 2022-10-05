import { Component, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { BaseComponent } from '../../models/base-component.model';
import { NgForm } from '@angular/forms';
import { SystemSettingsService } from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-reset-password-dialog',
  templateUrl: './reset-password-dialog.component.html',
  styleUrls: ['./reset-password-dialog.component.scss']
})
export class ResetPasswordDialogComponent extends BaseComponent {

  email: string;

  @ViewChild('form', { static: true }) form: NgForm;

  constructor(
    private router: Router,
    private userService: UserService,
    private settings: SystemSettingsService,
    private dialogRef: MatDialogRef<ResetPasswordDialogComponent>) {
      super();
  }

  get forgotPasswordIntroText(): string {
    return this.settings.jbAccount.login.forgotPasswordIntroText;
  }

  get messageForInvalidEmail(): string {
    return this.settings.jbAccount.errors.invalidEmail;
  }

  get forgotPasswordEmailNotFound(): string {
    return this.settings.jbAccount.errors.forgotPasswordEmailNotFound;
  }

  emailChange(): void {
    if (this.submitted) {
      this.submitted = false;
      this.clearError();
    }
  }

  reset(): void {
    this.submitted = true;

    if (this.form.valid) {
      this.userService.sendPwdResetEmail(this.email).subscribe(
        () => {
          this.router.navigate([`/password-reset/${this.email}/0`]);
          this.dialogRef.close();
        },
        error => this.error = error
      );
    }
  }
}
