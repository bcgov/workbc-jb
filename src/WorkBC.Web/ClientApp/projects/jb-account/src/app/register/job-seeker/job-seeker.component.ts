import {
  AfterViewChecked,
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { BaseJobSeekerComponent } from '../../models/base-component.model';
import { JobSeekerFlags } from '../../models/job-seeker-flags.model';
import { RecaptchaResponse } from '../../models/recaptcha.model';
import { RegisterModel } from '../../models/register.model';
import { LocationService } from '../../services/location.service';
import { LoggingService } from '../../services/logging.service';
import { SecurityQuestionService } from '../../services/security-question.service';
import { UserService } from '../../services/user.service';
import { MatDialog } from '@angular/material/dialog';
import { TermsOfUseComponent } from '../terms-of-use/terms-of-use.component';
import { SystemSettingsService, GlobalService } from '../../../../../jb-lib/src/public-api';

@Component({
  selector: 'app-job-seeker',
  templateUrl: './job-seeker.component.html',
  styleUrls: ['./job-seeker.component.scss']
})
export class JobSeekerComponent extends BaseJobSeekerComponent
  implements OnInit, AfterViewInit, AfterViewChecked {
  @Output() registered = new EventEmitter<string>();

  registerForm: FormGroup;
  isNotRobot = false;
  saved = false;

  private renderedWidget: unknown;

  @ViewChild('recaptcha', { static: true }) recaptchaElement: ElementRef;
  @ViewChild('firstName', { static: true }) firstNameField: ElementRef;

  constructor(
    protected userService: UserService,
    private formBuilder: FormBuilder,
    private cdRef: ChangeDetectorRef,
    protected securityQuestionService: SecurityQuestionService,
    protected locationService: LocationService,
    protected dialog: MatDialog,
    settings: SystemSettingsService,
    private loggingService: LoggingService,
    private globalService: GlobalService
  ) {
    super(securityQuestionService, locationService, null, settings);
  }

  get whyIdentify(): string {
    return this.settings.jbAccount.shared.whyIdentify;
  }

  get passwordComplexity(): string {
    return this.settings.jbAccount.shared.passwordComplexity;
  }

  get termsOfUseRequired(): string {
    return this.settings.jbAccount.errors.termsOfUseRequired;
  }

  get hasChanges(): boolean {
    return (
      !!this.vm.firstName.value ||
      !!this.vm.lastName.value ||
      !!this.vm.email.value ||
      !!this.vm.password.value ||
      !!this.vm.confirmPassword.value ||
      this.vm.securityQuestionId.value != 0 ||
      !!this.vm.securityAnswer.value ||
      this.vm.countryId.value != BaseJobSeekerComponent.defaultCountryId ||
      this.vm.provinceId.value != BaseJobSeekerComponent.defaultProvinceId ||
      !!this.vm.city.value ||
      this.isNotRobot ||
      this.vm.agreed.value ||
      this.vm.isApprentice.value ||
      this.vm.isIndigenousPerson.value ||
      this.vm.isPersonWithDisability.value ||
      this.vm.isMatureWorker.value ||
      this.vm.isYouth.value ||
      this.vm.isVisibleMinority.value ||
      this.vm.isVeteran.value ||
      this.vm.isStudent.value ||
      this.vm.isNewImmigrant.value
    );
  }

  ngOnInit(): void {
    super.ngOnInit();

    this.registerForm = this.formBuilder.group(
      {
        firstName: ['', [Validators.required, Validators.maxLength(50)]],
        lastName: ['', [Validators.required, Validators.maxLength(50)]],
        email: [
          '',
          [
            Validators.required,
            Validators.email,
            Validators.pattern(this.validEmailPattern)
          ]
        ],
        password: [
          '',
          [Validators.required, Validators.minLength(6), this.passwordValidator]
        ],
        confirmPassword: ['', [Validators.required, Validators.minLength(6)]],
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
        agreed: [false, [Validators.requiredTrue]]
      },
      { validators: [this.passwordMatchValidator, this.cityValidator] }
    );

    this.addRecaptchaScript();

    this.onValueChanges([this.vm.email]);

  }

  ngAfterViewInit(): void {
    this.setRegionsRetrieval(this.vm);
  }

  ngAfterViewChecked(): void {
    this.cdRef.detectChanges();
  }

  addRecaptchaScript() {
    window['grecaptchaCallback'] = () => {
      this.renderReCaptcha();
    };

    (function(d, s, id, obj) {
      if (d.getElementById(id)) {
        obj.renderReCaptcha();
        return;
      }

      const js = d.createElement(s) as HTMLScriptElement;
      js.id = id;
      js.src =
        'https://www.google.com/recaptcha/api.js?onload=grecaptchaCallback&amp;render=explicit';
      js.defer = true;

      const fjs = d.getElementsByTagName(s)[0];
      fjs.parentNode.insertBefore(js, fjs);
    })(document, 'script', 'recaptcha-jssdk', this);

  }

  renderReCaptcha(): void {
    this.renderedWidget = window['grecaptcha'].render(
      this.recaptchaElement.nativeElement,
      {
        sitekey: '6LelmqUZAAAAAH-TBWblVJemENuJqps3hnYbLX69',
        callback: response => {
          this.loggingService.log(`Recaptcha response: ${response}`);
          if (response) {
            this.userService
              .verifyRecaptcha(response)
              .subscribe((x: RecaptchaResponse) => {
                this.enableOrDisableSubmitBtn(x.success);
                this.loggingService.log(x);
              });
          }
        },
        'expired-callback': () => {
          this.enableOrDisableSubmitBtn(false);
        }
      }
    );
  }

  enableOrDisableSubmitBtn(isNotRobot: boolean): void {
    this.isNotRobot = isNotRobot;
    const submitBtn = document.getElementById('submitBtn') as HTMLButtonElement;
    submitBtn.disabled = !isNotRobot;
  }

  // convenience getter for easy access to form fields
  get vm() {
    return this.getFormGroup(this.registerForm);
  }

  get isNotCanada() {
    const result = this.notDefaultCountry(this.vm);
    return result;
  }

  get isNotBC() {
    const result = this.notDefaultProvince(this.vm);
    return result;
  }

  private resetGrecaptcha() {
    window['grecaptcha'].reset(this.renderedWidget);
    this.enableOrDisableSubmitBtn(false);
  }

  onSubmit(): void {
    this.submitted = true;
    this.hideCityErrors = false;
    this.hideRegionErrors = false;

    // stop here if form is invalid
    if (
      this.registerForm.invalid ||
      !this.isValidRegion(this.vm) ||
      !this.noEmailRelatedError ||
      this.isInvalidCity(this.vm.city.value)
    ) {
      this.scrollIntoView(this.registerForm, this.vm);
      this.resetGrecaptcha();
      return;
    }

    this.clearError();

    const user = new RegisterModel(
      this.vm.email.value,
      this.vm.password.value,
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

    //this.loading = true;
    this.userService
      .register(user)
      .pipe(first())
      .subscribe(
        () => {
          //this.loggingService.log('Registration successful', value);
          this.saved = true;
          this.registered.emit(user.email);
        },
        error => {
          this.error = error;
          this.scrollIntoView(this.registerForm, this.vm);
        }
      );

    this.resetGrecaptcha();
  }

  showTermsOfUse(event: Event): void {
    event.preventDefault();

    this.dialog.open(TermsOfUseComponent, {
      width: '85%',
      maxWidth: 647
    });
  }

  get mapUrl(): string {
    return this.globalService.getApiBaseUrl() + 'assets/images/bc-region-map2x.png';
  }
}
