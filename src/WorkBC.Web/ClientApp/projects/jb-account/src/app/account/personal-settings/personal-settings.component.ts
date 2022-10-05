import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Validators, FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';

import { BaseJobSeekerComponent } from '../../models/base-component.model';
import { SecurityQuestionService } from '../../services/security-question.service';
import { LocationService } from '../../services/location.service';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { RegisterModel } from '../../models/register.model';
import { JobSeekerFlags } from '../../models/job-seeker-flags.model';
import { ToastrService } from 'ngx-toastr';
import {
  StorageService,
  AuthenticationService,
  SimpleDialogComponent,
  SystemSettingsService,
  GlobalService
} from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-personal-settings',
  templateUrl: './personal-settings.component.html',
  styleUrls: ['./personal-settings.component.scss'],
})
export class PersonalSettingsComponent extends BaseJobSeekerComponent
  implements OnInit, AfterViewInit {
  forPasswordReset = false;
  saved = false;

  constructor(
    private formBuilder: FormBuilder,
    protected securityQuestionService: SecurityQuestionService,
    protected locationService: LocationService,
    protected userService: UserService,
    private router: Router,
    protected dialog: MatDialog,
    private storageService: StorageService,
    private toastr: ToastrService,
    settings: SystemSettingsService,
    private authenticationService: AuthenticationService,
    private globalService: GlobalService
  ) {
    super(securityQuestionService, locationService, userService, settings);
  }

  ngOnInit(): void {
    super.ngOnInit();

    this.personalSettingsFormGroup = this.formBuilder.group(
      {
        firstName: [
          this.authenticationService.currentUser.firstName,
          [Validators.required, Validators.maxLength(50)],
        ],
        lastName: [
          this.authenticationService.currentUser.lastName,
          [Validators.required, Validators.maxLength(50)],
        ],
        email: [
          this.authenticationService.currentUser.email,
          [
            Validators.required,
            Validators.email,
            Validators.pattern(this.validEmailPattern),
          ],
        ],
        oldPassword: [
          '',
          [
            Validators.required,
            Validators.minLength(6),
            this.passwordValidator,
          ],
        ],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(6),
            this.passwordValidator,
          ],
        ],
        confirmPassword: [
          '',
          [
            Validators.required,
            Validators.minLength(6),
            this.passwordValidator,
          ],
        ],
        countryId: [BaseJobSeekerComponent.defaultCountryId],
        provinceId: [BaseJobSeekerComponent.defaultProvinceId],
        city: [''],
        locationId: [0],
        securityQuestionId: [0, [this.securityQuestionValidator]],
        securityAnswer: ['', [Validators.required, Validators.maxLength(50)]],
        isApprentice: [false],
        isIndigenousPerson: [false],
        isPersonWithDisability: [false],
        isMatureWorker: [false],
        isYouth: [false],
        isVisibleMinority: [false],
        isVeteran: [false],
        isStudent: [false],
        isNewImmigrant: [false],
      },
      { validators: [this.passwordMatchValidator, this.cityValidator] }
    );

    //this.onCityValueChanges(this.personalSettingsFormGroup, this.vm);
    const currentUserInfo$ = this.getCurrentUserInfo();
    if (currentUserInfo$) {
      currentUserInfo$.subscribe((jobSeeker) => {
        this.setVm(jobSeeker);
      });
    }
  }

  ngAfterViewInit(): void {
    this.setRegionsRetrieval(this.vm);
  }

  get whyIdentify(): string {
    return this.settings.jbAccount.shared.whyIdentify;
  }

  get passwordComplexity(): string {
    return this.settings.jbAccount.shared.passwordComplexity;
  }

  get pageTitle(): string {
    return this.forPasswordReset ? 'Reset Password' : 'Personal Settings';
  }

  resetPassword(): void {

    this.clearError();
    this.forPasswordReset = true;

    if (this.submitted) {
      this.submitted = false;
    }

    if (!this.vm.oldPassword.pristine) {
      this.vm.oldPassword.reset();
    }

    if (!this.vm.password.pristine) {
      this.vm.password.reset();
    }

    if (!this.vm.confirmPassword.pristine) {
      this.vm.confirmPassword.reset();
    }
  }

  onSubmit(): void {
    this.submitted = true;
    this.hideCityErrors = false;
    this.hideRegionErrors = false;

    if (this.forPasswordReset) {
      if (
        this.hasErrors(this.vm.oldPassword) ||
        this.hasErrors(this.vm.password) ||
        this.hasErrors(this.vm.confirmPassword) ||
        (this.personalSettingsFormGroup.errors &&
          this.personalSettingsFormGroup.errors.mismatch)
      ) {
        return;
      }

      this.clearError();

      const passwordResetModel = {
        currentPassword: this.vm.oldPassword.value,
        newPassword: this.vm.password.value,
      };
      this.userService
        .changePassword(passwordResetModel)
        .subscribe((result) => {
          if (result.succeeded) {
            this.toastr.success('You\'ve successfully changed your password.');
            this.forPasswordReset = false;
          } else if (result.errors && result.errors.length > 0) {
            this.error = `${result.errors[0].description}`;
            console.error(
              `Error code: ${result.errors[0].code}, description: ${result.errors[0].description}`
            );
          }
        });
    } else {
      if (
        this.hasErrors(this.vm.firstName) ||
        this.hasErrors(this.vm.lastName) ||
        this.hasErrors(this.vm.email) ||
        this.hasErrors(this.vm.securityQuestionId) ||
        this.hasErrors(this.vm.securityAnswer) ||
        this.cityValidator(this.personalSettingsFormGroup) ||
        this.isInvalidCity(this.vm.city.value) ||
        !this.isValidRegion(this.vm) ||
        !this.noEmailRelatedError
      ) {
        this.scrollIntoView(this.personalSettingsFormGroup, this.vm, false);
        return;
      }

      this.clearError();

      const user = new RegisterModel(
        this.vm.email.value,
        '', //this.vm.password.value,
        this.vm.firstName.value,
        this.vm.lastName.value,
        this.vm.countryId.value,
        this.vm.provinceId.value,
        this.vm.city.value,
        this.vm.locationId.value,
        this.vm.securityQuestionId.value,
        this.vm.securityAnswer.value,
        new JobSeekerFlags(
          this.vm.isApprentice.value,
          this.vm.isIndigenousPerson.value,
          this.vm.isPersonWithDisability.value,
          this.vm.isMatureWorker.value,
          this.vm.isYouth.value,
          this.vm.isVisibleMinority.value,
          this.vm.isVeteran.value,
          this.vm.isStudent.value,
          this.vm.isNewImmigrant.value
        )
      );

      this.userService.updatePersonalSettings(user).subscribe(
        () => {
          this.saved = true;
          this.toastr.success(
            'You\'ve successfully changed your account information.'
          );

          const currentUser = this.authenticationService.currentUser;

          const isNeeded =
            this.vm.firstName.value !== currentUser.firstName ||
            this.vm.lastName.value !== currentUser.lastName ||
            this.vm.email.value !== currentUser.email;

          if (this.vm.firstName.value !== currentUser.firstName) {
            currentUser.firstName = this.vm.firstName.value;
          }
          if (this.vm.lastName.value !== currentUser.lastName) {
            currentUser.lastName = this.vm.lastName.value;
          }
          if (this.vm.email.value !== currentUser.email) {
            currentUser.email = this.vm.email.value;
            currentUser.username = this.vm.email.value;
          }

          if (isNeeded) {
            this.storageService.setItem(
              this.authenticationService.currentUserKey,
              JSON.stringify(currentUser)
            );
            this.authenticationService.currentUserSubject.next(currentUser);
          }

          this.cancel();
        },
        (error) => {
          this.error = error;
          this.scrollIntoView(this.personalSettingsFormGroup, this.vm, false);
        }
      );
    }
  }

  cancel(): void {
    if (this.forPasswordReset) this.forPasswordReset = false;
    else this.router.navigate(['/']);
  }

  deleteAccount(): void {
    this.dialog
      .open(SimpleDialogComponent, {
        data: {
          title: 'DELETE ACCOUNT',
          btnLabel: 'Continue',
          content:
            'Are you sure you want to delete your account? You will no longer be able to log in or access your saved items.',
        },
        width: '85%',
        maxWidth: 500,
      })
      .afterClosed()
      .subscribe((result) => {
        if (result) {
          this.userService.deleteAccount().subscribe(() => {
            this.toastr.success('You\'ve successfully deleted your account.');

            this.authenticationService.logout();
            //location.reload(true);
            this.router.navigate(['/']);
          });
        }
      });
  }


  get mapUrl(): string {
    return this.globalService.getApiBaseUrl() + 'assets/images/bc-region-map2x.png';
  }
}
