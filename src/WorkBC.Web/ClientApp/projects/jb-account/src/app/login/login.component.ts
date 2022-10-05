import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  FormControl,
} from '@angular/forms';
import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { BaseComponent } from '../models/base-component.model';
import { UserService } from '../services/user.service';
import { ToastrService } from 'ngx-toastr';
import {
  StorageService,
  AuthenticationService,
  LoginModel,
  SystemSettingsService} from '../../../../jb-lib/src/public-api';
import { MatDialog } from '@angular/material/dialog';
import { ResetPasswordDialogComponent } from './reset-password-dialog/reset-password-dialog.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent extends BaseComponent implements OnInit {
  loginForm: FormGroup;
  returnUrl = '/dashbord';

  @ViewChild('email', { static: true }) emailField: ElementRef;

  constructor(
    private formBuilder: FormBuilder,
    private settings: SystemSettingsService,
    private router: Router,
    private storageService: StorageService,
    private userService: UserService,
    private toastr: ToastrService,
    protected dialog: MatDialog,
    private authenticationService: AuthenticationService
  ) {
    super();

    // redirect to home if already logged in
    if (this.authenticationService.currentUser) {
      this.router.navigate(['/']);
    } else {
      this.storageService.removeLocalStorageItem(
        this.storageService.savedJobsKey
      );
    }
  }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: [
        '',
        [
          Validators.required,
          Validators.email,
          Validators.pattern(this.validEmailPattern),
        ],
      ],
      password: ['', [Validators.required]],
      staySignedIn: [this.storageService.staySignedIn],
    });

    // get return url from route parameters or default to '/'
    this.onValueChanges([this.vm.email, this.vm.password]);
  }

  get messageForInvalidEmail(): string {
    return this.settings.jbAccount.errors.invalidEmail;
  }

  get messageForEmptyPassword(): string {
    return this.settings.jbAccount.errors.emptyPassword;
  }

  // convenience getter for easy access to form fields
  get vm() {
    return this.loginForm.controls as {
      [key: string]: FormControl;
    };
  }

  get itemType(): string {
    let result = '';

    if (this.hasTmpSaveJob) {
      result = 'job posting';
    }

    if (this.hasTmpJobAlert) {
      if (!result) {
        result = 'job alert';
      } else {
        return 'item';
      }
    }

    if (this.totalTmpSavedCareerProfiles) {
      if (!result) {
        result = 'career profile';
      } else {
        return 'item';
      }
    }

    if (this.totalTmpSavedIndustryProfiles) {
      result = !result ? 'industry profile' : 'item';
    }

    return result;
  }

  get hasMoreThanOneType(): boolean {
    return this.itemType === 'item';
  }

  get totalSavedItems(): number {
    return (
      this.totalTmpSavedJobs +
      this.totalTmpSavedCareerProfiles +
      (this.hasTmpJobAlert ? 1 : 0) +
      this.totalTmpSavedIndustryProfiles
    );
  }

  get totalSavedItemsText(): string {

    switch (this.totalSavedItems) {
      case 1:
        return 'one';
      case 2:
        return 'two';
      case 3:
        return 'three';
      case 4:
        return 'four';
      case 5:
        return 'five';
      case 6:
        return 'six';
      case 7:
        return 'seven';
      case 8:
        return 'eight';
      case 9:
        return 'nine';
      default:
        return this.totalSavedItems.toString();
    }
  }

  get totalTmpSavedJobs(): number {
    const tmpSavedJobs = this.storageService.getLocalStorageItem(
      this.storageService.tmpSavedJobsKey
    );
    return tmpSavedJobs ? tmpSavedJobs.split(',').length : 0;
  }

  get hasTmpSaveJob(): boolean {
    return this.totalTmpSavedJobs > 0;
  }

  get totalTmpSavedCareerProfiles(): number {
    const tmpCareerProfile = this.storageService.getLocalStorageItem(
      this.storageService.tmpCareerProfileKey
    );
    return tmpCareerProfile ? tmpCareerProfile.split(',').length : 0;
  }

  get totalTmpSavedIndustryProfiles(): number {
    const tmpIndustryProfile = this.storageService.getLocalStorageItem(
      this.storageService.tmpIndustryProfileKey
    );
    return tmpIndustryProfile ? tmpIndustryProfile.split(',').length : 0;
  }

  get hasTmpJobAlert(): boolean {
    const tmpJobAlert = this.storageService.getLocalStorageItem(
      this.storageService.tmpJobAlertKey
    );
    return !!tmpJobAlert;
  }

  get showSavingJobMessage(): boolean {
    return (
      this.hasTmpSaveJob ||
      this.hasTmpJobAlert ||
      !!this.totalTmpSavedCareerProfiles ||
      !!this.totalTmpSavedIndustryProfiles
    );
  }

  onSubmit(): void {
    this.submitted = true;

    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    //this.loading = true;
    this.storageService.staySignedIn = this.vm.staySignedIn.value;

    const loginModel = new LoginModel(
      this.vm.email.value,
      this.vm.password.value
    );
    //this.loggingService.log(loginData);
    this.authenticationService
      .login(loginModel)
      .pipe(first())
      .subscribe(
        () => {
          this.router.navigate([this.returnUrl]);
        },
        (error) => {
          this.error = error.message;
          this.errorField = error.field;
        }
      );
  }

  forgotPasswordModal(event: Event): void {
    event.preventDefault();

    this.dialog.open(ResetPasswordDialogComponent, {
      width: '85%',
      maxWidth: 500,
    });
  }

  resendEmail(): void {
    this.userService.sendActivationEmail(this.vm.email.value).subscribe(() => {
      this.toastr.success(
        `An activation email has been sent to ${this.vm.email.value}. Please follow the instructions to activate your account.`,
        'Activation Email Sent'
      );
      //this.loggingService.log('Activation email has been successfully sent.');
    });
  }

  get showEmailActivationLink(): boolean {
    return this.error === 'Your account is awaiting email activation.';
  }
}
