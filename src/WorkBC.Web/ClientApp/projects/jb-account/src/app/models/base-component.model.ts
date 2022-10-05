import { FormControl, AbstractControl, FormGroup } from '@angular/forms';
import { Observable, BehaviorSubject, Subscription, of } from 'rxjs';
import {
  SecurityQuestion,
  SecurityQuestionService
} from '../services/security-question.service';
import {
  Country,
  LocationService,
  Province
} from '../services/location.service';
import { OnInit, OnDestroy, Directive } from '@angular/core';
import {
  debounceTime,
  startWith,
  distinctUntilChanged,
  switchMap
} from 'rxjs/operators';
import { Region } from './regions.model';
import {
  SystemSettingsService
} from '../../../../jb-lib/src/public-api';
import { MatDialog } from '@angular/material/dialog';
import { Register } from './register.model';
import { UserService } from '../services/user.service';

export abstract class SortableComponent {
  sortBy = 3;

  protected sortData(objects: { title: string }[]): void {
    switch (+this.sortBy) {
      case 3:
        objects.sort((a, b) =>
          a.title.toLowerCase() > b.title.toLowerCase() ? 1 : -1
        );
        break;
      case 4:
        objects.sort((a, b) =>
          a.title.toLowerCase() > b.title.toLowerCase() ? -1 : 1
        );
        break;
    }
  }
}

@Directive()
export abstract class BaseFormComponent {

  error = '';
  errorField = -1;

  constructor(protected dialog?: MatDialog) {}


  clearError(): void {
    if (this.error) this.error = '';
  }
}

@Directive()
export class BaseComponent extends BaseFormComponent {

  submitted = false;

  private static readonly regExp = /^(?=.*[A-Z])(?=.*\d)[a-zA-Z\d@$!%*#?&]{6,}$/;
  get passwordPattern() {
    return BaseComponent.regExp;
  }

  readonly validEmailPattern =
    '^[a-zA-Z0-9._%+&-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z0-9]{2,4}$';

  protected onValueChanges(formControls: FormControl[]) {
    for (const formControl of formControls) {
      formControl.valueChanges.subscribe(() => {
        this.clearError();
      });
    }
  }

  isValid(formControl: FormControl): boolean {
    return (
      formControl.valid ||
      (!this.submitted && (!formControl.dirty || !formControl.touched))
    );
  }

  passwordValidator(control: AbstractControl) {
    return BaseComponent.regExp.test(control.value)
      ? null
      : { invalidPassword: true };
  }
}

@Directive()
export class BaseJobSeekerComponent extends BaseComponent
  implements OnInit, OnDestroy {
  personalSettingsFormGroup: FormGroup;
  securityQuestions$: Observable<SecurityQuestion>;
  countries$: Observable<Country>;
  provinces$: Observable<Province>;
  cities$: Observable<string[]>;
  jobSeeker: Register;
  hideCityErrors = false;
  hideRegionErrors = false;

  subject = new BehaviorSubject<Region[]>([]);
  regions$ = this.subject.asObservable();

  protected subscription: Subscription;

  protected static readonly defaultCountryId = 37;
  protected static readonly defaultProvinceId = 2;

  protected cityOptions: string[] = [];

  constructor(
    protected securityQuestionService: SecurityQuestionService,
    protected locationService: LocationService,
    protected userService?: UserService,
    protected settings?: SystemSettingsService
  ) {
    super();
  }

  ngOnInit(): void {
    this.securityQuestions$ = this.securityQuestionService.getSecurityQuestions();
    this.countries$ = this.locationService.getCountries();
    this.provinces$ = this.locationService.getProvinces();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  hasErrors(formControl: FormControl): boolean {
    return !!formControl.errors;
  }

  isInvalidCity(location: string): boolean {
    return (
      this.submitted &&
      location &&
      (this.cityOptions.length === 0 ||
        !this.cityOptions.find(x => x.toLowerCase() === location.toLowerCase()))
    );
  }

  securityQuestionValidator(control: AbstractControl) {
    return control.value != 0 ? null : { invalidSecurityQuestionId: true };
  }

  isValidRegion(vm: { [key: string]: FormControl }): boolean {
    return (
      !vm.city.value ||
      this.currentRegions.length < 2 ||
      vm.locationId.value > 0
    );
  }

  passwordNotMatch(formGroup: FormGroup): boolean {
    const confirmPasswordControl = formGroup.get('confirmPassword');
    return (
      formGroup.errors &&
      formGroup.errors.mismatch &&
      !this.invalidPassword(confirmPasswordControl) &&
      (this.submitted || confirmPasswordControl.touched)
    );
  }

  passwordRequired(formControl: FormControl): boolean {
    return formControl.errors && formControl.errors.required;
  }

  invalidPassword(formControl: AbstractControl): boolean {
    return formControl.errors && formControl.errors.invalidPassword;
  }

  protected setVm(jobSeeker: Register, fromDialog = false): void {
    this.jobSeeker = jobSeeker;
    if (jobSeeker) {
      if (this.vm.countryId.value !== jobSeeker.countryId) {
        this.vm.countryId.setValue(jobSeeker.countryId);
      }

      if (this.vm.provinceId.value !== jobSeeker.provinceId) {
        this.vm.provinceId.setValue(jobSeeker.provinceId);
      }

      if (this.vm.city.value !== jobSeeker.city) {
        this.vm.city.setValue(jobSeeker.city);
      }

      if (this.vm.locationId.value !== jobSeeker.locationId) {
        this.vm.locationId.setValue(jobSeeker.locationId);
      }

      if (this.vm.securityQuestionId.value !== jobSeeker.securityQuestionId) {
        this.vm.securityQuestionId.setValue(jobSeeker.securityQuestionId);
      }

      if (this.vm.securityAnswer.value !== jobSeeker.securityAnswer) {
        this.vm.securityAnswer.setValue(jobSeeker.securityAnswer);
      }

      if (!fromDialog) {
        if (
          this.vm.isApprentice.value !== jobSeeker.jobSeekerFlags.isApprentice
        ) {
          this.vm.isApprentice.setValue(jobSeeker.jobSeekerFlags.isApprentice);
        }

        if (
          this.vm.isIndigenousPerson.value !==
          jobSeeker.jobSeekerFlags.isIndigenousPerson
        ) {
          this.vm.isIndigenousPerson.setValue(
            jobSeeker.jobSeekerFlags.isIndigenousPerson
          );
        }

        if (
          this.vm.isPersonWithDisability.value !==
          jobSeeker.jobSeekerFlags.isPersonWithDisability
        ) {
          this.vm.isPersonWithDisability.setValue(
            jobSeeker.jobSeekerFlags.isPersonWithDisability
          );
        }

        if (
          this.vm.isMatureWorker.value !==
          jobSeeker.jobSeekerFlags.isMatureWorker
        ) {
          this.vm.isMatureWorker.setValue(
            jobSeeker.jobSeekerFlags.isMatureWorker
          );
        }

        if (this.vm.isYouth.value !== jobSeeker.jobSeekerFlags.isYouth) {
          this.vm.isYouth.setValue(jobSeeker.jobSeekerFlags.isYouth);
        }

        if (
          this.vm.isVisibleMinority.value !==
          jobSeeker.jobSeekerFlags.isVisibleMinority
        ) {
          this.vm.isVisibleMinority.setValue(
            jobSeeker.jobSeekerFlags.isVisibleMinority
          );
        }

        if (this.vm.isVeteran.value !== jobSeeker.jobSeekerFlags.isVeteran) {
          this.vm.isVeteran.setValue(jobSeeker.jobSeekerFlags.isVeteran);
        }

        if (this.vm.isStudent.value !== jobSeeker.jobSeekerFlags.isStudent) {
          this.vm.isStudent.setValue(jobSeeker.jobSeekerFlags.isStudent);
        }

        if (
          this.vm.isNewImmigrant.value !==
          jobSeeker.jobSeekerFlags.isNewImmigrant
        ) {
          this.vm.isNewImmigrant.setValue(
            jobSeeker.jobSeekerFlags.isNewImmigrant
          );
        }

        if (this.vm.email.value !== jobSeeker.email) {
          this.vm.email.setValue(jobSeeker.email);
        }
      }

      if (this.vm.firstName.value !== jobSeeker.firstName) {
        this.vm.firstName.setValue(jobSeeker.firstName);
      }
      if (this.vm.lastName.value !== jobSeeker.lastName) {
        this.vm.lastName.setValue(jobSeeker.lastName);
      }
    }
  }

  protected getCurrentUserInfo(): Observable<Register> {
    return this.userService ? this.userService.getCurrentUser() : null;
  }

  protected scrollIntoView(
    formGroup: FormGroup,
    vm: { [key: string]: FormControl },
    checkPasswords = true
  ): void {
    let elementId = 'SecurityQuestion';

    if (
      !this.isValid(vm.firstName) ||
      !this.isValid(vm.lastName) ||
      !this.isValid(vm.email) ||
      !this.noEmailRelatedError ||
      (checkPasswords &&
        (!this.isValid(vm.password) ||
          !this.isValid(vm.confirmPassword) ||
          this.passwordNotMatch(formGroup)))
    ) {
      elementId = 'PersonalInfo';
    }

    document.getElementById(elementId).scrollIntoView();
  }

  get currentRegions(): Region[] {
    return this.subject.value ? this.subject.value : [];
  }

  get noEmailRelatedError(): boolean {
    return !this.submitted || this.error.toLowerCase().indexOf('email') == -1;
  }

  protected getFormGroup(
    formGroup: FormGroup
  ): {
    [key: string]: FormControl;
  } {
    return formGroup
      ? (formGroup.controls as {
          [key: string]: FormControl;
        })
      : null;
  }

  get vm(): {
    [key: string]: FormControl;
  } {
    return this.getFormGroup(this.personalSettingsFormGroup);
  }

  protected resetLocationId(
    vm: { [key: string]: FormControl },
    locationId: number
  ) {
    if (vm.locationId.value != locationId) {
      vm.locationId.setValue(locationId);
    }
  }

  protected resetRegions(vm: { [key: string]: FormControl }) {
    if (this.subject.value.length > 0) {
      this.subject.next([]);
      this.resetLocationId(vm, 0);
    }
  }

  protected setRegionsRetrieval(vm: { [key: string]: FormControl }): void {

    const typeahead = vm.city.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((city: string) => {
        if (!city || city.trim().length === 0) {
          return of<Region[]>([]);
        }

        this.cities$ = this.locationService.getCities(city);
        this.cities$.subscribe(cities => (this.cityOptions = cities));

        return this.locationService.getRegions(city);
      })
    );

    this.subscription = typeahead.subscribe(regions => {
      this.subject.next(regions);
      this.resetLocationId(
        vm,
        regions.length === 1
          ? regions[0].locationId
          : regions.filter(x => x.locationId === vm.locationId.value).length ===
            0
          ? 0
          : vm.locationId.value
      );
    });
  }

  protected resetCity(vm: { [key: string]: FormControl }) {
    if (vm.city.value) {
      vm.city.setValue('');
      this.hideCityErrors = true;
      this.hideRegionErrors = true;
      this.resetRegions(vm);
    }
  }

  protected notDefaultCountry(vm: { [key: string]: FormControl }) {
    const result =
      vm.countryId.value != BaseJobSeekerComponent.defaultCountryId;
    if (result) {
      if (vm.provinceId.value) {
        vm.provinceId.setValue(0);
      }
      this.resetCity(vm);
    } else {
      if (!vm.provinceId.value) {
        vm.provinceId.setValue(BaseJobSeekerComponent.defaultProvinceId);
      }
    }
    return result;
  }

  get isNotCanada(): boolean {
    const result = this.notDefaultCountry(this.vm);
    return result;
  }

  protected notDefaultProvince(vm: { [key: string]: FormControl }) {
    const result =
      vm.provinceId.value != BaseJobSeekerComponent.defaultProvinceId;
    if (result) {
      this.resetCity(vm);
    }
    return result;
  }

  get isNotBC(): boolean {
    const result = this.notDefaultProvince(this.vm);
    return result;
  }

  protected cityValidator(formGroup: FormGroup) {
    return formGroup.get('countryId').value !=
      BaseJobSeekerComponent.defaultCountryId ||
      formGroup.get('provinceId').value !=
        BaseJobSeekerComponent.defaultProvinceId ||
      !!formGroup.get('city').value
      ? null
      : { invalidCity: true };
  }

  protected passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password').value;
    const confirmPassword = formGroup.get('confirmPassword').value;
    return !password || !confirmPassword || password === confirmPassword
      ? null
      : { mismatch: true };
  }
}
