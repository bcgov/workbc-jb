import {
  Component,
  OnInit,
  ViewEncapsulation,
  AfterViewInit
} from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { BaseJobSeekerComponent } from '../../models/base-component.model';
import { SecurityQuestionService } from '../../services/security-question.service';
import { LocationService } from '../../services/location.service';
import {
  AuthenticationService,
  StorageService,
  GlobalService
} from '../../../../../jb-lib/src/public-api';
import { UserService } from '../../services/user.service';
import { MatDialogRef } from '@angular/material/dialog';
import { RegisterModel } from '../../models/register.model';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'personal-settings-dialog',
  templateUrl: './personal-settings-dialog.component.html',
  styleUrls: [
    '../personal-settings/personal-settings.component.scss',
    './personal-settings-dialog.component.scss'
  ],
  encapsulation: ViewEncapsulation.None
})
export class PersonalSettingsDialogComponent extends BaseJobSeekerComponent
  implements OnInit, AfterViewInit {
  loading = true;
  showSecurityQuestion = false;
  showLocation = false;

  constructor(
    private dialogRef: MatDialogRef<PersonalSettingsDialogComponent>,
    private formBuilder: FormBuilder,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    private storageService: StorageService,
    protected securityQuestionService: SecurityQuestionService,
    protected userService: UserService,
    protected locationService: LocationService,
    private globalService: GlobalService
  ) {
    super(securityQuestionService, locationService, userService);
  }

  ngOnInit(): void {
    super.ngOnInit();

    this.personalSettingsFormGroup = this.formBuilder.group(
      {
        firstName: [
          this.authenticationService.currentUser.firstName,
          [Validators.required, Validators.maxLength(50)]
        ],
        lastName: [
          this.authenticationService.currentUser.lastName,
          [Validators.required, Validators.maxLength(50)]
        ],
        countryId: [BaseJobSeekerComponent.defaultCountryId],
        provinceId: [BaseJobSeekerComponent.defaultProvinceId],
        city: [''],
        locationId: [0],
        securityQuestionId: [0, [this.securityQuestionValidator]],
        securityAnswer: ['', [Validators.required, Validators.maxLength(50)]]
      },
      { validators: [this.cityValidator] }
    );

    const currentUserInfo$ = this.getCurrentUserInfo();
    if (currentUserInfo$) {
      currentUserInfo$.subscribe(jobSeeker => {
        this.setVm(jobSeeker, true);
        this.continue();
      });
    }
  }

  private continue(): void {
    this.showSecurityQuestion =
      this.vm.securityQuestionId.value == 0 || !this.vm.securityAnswer.value;
    this.showLocation =
      !this.isValidRegion(this.vm) ||
      !!this.cityValidator(this.personalSettingsFormGroup);

    if (
      !this.showPersonalInfo &&
      !this.showSecurityQuestion &&
      !this.showLocation
    ) {
      this.dialogRef.close(true);
      return;
    }

    setTimeout(() => {
      this.loading = false;
      this.dialogRef.updateSize('90%');
    });
  }

  ngAfterViewInit(): void {
    this.setRegionsRetrieval(this.vm);
  }

  get hasFirstName(): boolean {
    return !!this.authenticationService.currentUser.firstName;
  }

  get hasLastName(): boolean {
    return !!this.authenticationService.currentUser.lastName;
  }

  get showPersonalInfo(): boolean {
    return !this.hasFirstName || !this.hasLastName;
  }

  update(): void {
    this.submitted = true;
    this.hideCityErrors = false;
    this.hideRegionErrors = false;

    if (
      this.hasErrors(this.vm.firstName) ||
      this.hasErrors(this.vm.lastName) ||
      this.hasErrors(this.vm.securityQuestionId) ||
      this.hasErrors(this.vm.securityAnswer) ||
      this.cityValidator(this.personalSettingsFormGroup) ||
      !this.isValidRegion(this.vm)
    ) {
      return;
    }

    const user = new RegisterModel(
      this.authenticationService.currentUser.email,
      '', //this.vm.password.value,
      this.vm.firstName.value,
      this.vm.lastName.value,
      this.vm.countryId.value,
      this.vm.provinceId.value,
      this.vm.city.value,
      this.vm.locationId.value,
      this.vm.securityQuestionId.value,
      this.vm.securityAnswer.value
    );

    this.userService.updatePersonalSettings(user).subscribe(() => {
      this.toastr.success(
        'You\'ve successfully changed your account information.'
      );

      const currentUser = this.authenticationService.currentUser;

      const isNeeded =
        this.vm.firstName.value !== currentUser.firstName ||
        this.vm.lastName.value !== currentUser.lastName;

      if (this.vm.firstName.value !== currentUser.firstName) {
        currentUser.firstName = this.vm.firstName.value;
      }
      if (this.vm.lastName.value !== currentUser.lastName) {
        currentUser.lastName = this.vm.lastName.value;
      }

      if (isNeeded) {
        this.storageService.setItem(
          this.authenticationService.currentUserKey,
          JSON.stringify(currentUser)
        );
        this.authenticationService.currentUserSubject.next(currentUser);
      }

      this.dialogRef.close(1);
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }

  get mapUrl(): string {
    return this.globalService.getApiBaseUrl() + 'assets/images/bc-region-map2x.png';
  }
}
