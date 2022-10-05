export class JbDashBoard {
  constructor(
    public newAccountMessageTitle = '',
    public newAccountMessageBody = '',
    public notification1Title = '',
    public notification1Body = '',
    public notification1Hash = '',
    public notification1Enabled = false,
    public notification2Title = '',
    public notification2Body = '',
    public notification2Hash = '',
    public notification2Enabled = false,
    public introText = '',
    public jobsDescription = '',
    public careersDescription = '',
    public accountDescription = '',
    public resource1Title = '',
    public resource2Title = '',
    public resource3Title = '',
    public resource1Body = '',
    public resource2Body = '',
    public resource3Body = '',
    public resource1Url = '',
    public resource2Url = '',
    public resource3Url = '',
  ) { }
}

export class JbAccountSettings {
  errors = {
    termsOfUseRequired: '',
    loginFailed: '',
    forgotPasswordEmailNotFound: '',
    recommendedJobsNoResultsOneCheckbox: '',
    recommendedJobsNoResultsMultipleCheckboxes: '',
    forgotPasswordInvalidToken: '',
    emptyPassword: '',
    invalidEmail: ''
  };
  settings = {};

  dashboard = new JbDashBoard();
  registration = {
    termsOfUseTitle: '',
    termsOfUseText: '',
    confirmationTitle: '',
    confirmationBody: '',
    activationTitle: '',
    activationBody: ''
  };
  shared = { whyIdentify: '', passwordComplexity: '' };
  login = {
    forgotPasswordIntroText: '',
    forgotPasswordConfirmationTitle: '',
    forgotPasswordConfirmationBody: ''
  };
  jobAlerts = { noEmailHelpQuestion: '', noEmailHelpAnswer: '' };
  recommendedJobs = {
    introText: '',
    introTextNoRecommendedJobs: '',
    filterIntroText: ''
  };
  careerProfiles = {
    callToAction1BodyText: '',
    callToAction1LinkText: '',
    callToAction1LinkUrl: '',
    callToAction2BodyText: '',
    callToAction2LinkText: '',
    callToAction2LinkUrl: '',
  };
  industryProfiles = {
    callToAction1BodyText: '',
    callToAction1LinkText: '',
    callToAction1LinkUrl: '',
    callToAction2BodyText: '',
    callToAction2LinkText: '',
    callToAction2LinkUrl: '',
  };
}

export class JbSearchSettings {
  errors = {
    invalidPostalCode: '',
    invalidCity: '',
    duplicatePostal: '',
    duplicateCity: '',
    outOfProvincePostal: '',
    noSearchResults: '',
    tooManyMapPins: '',
    cappedResultsWarning: '',
    missingMapPins: '',
    missingOneMapPin: '',
    noMapPins: ''
  };

  setting = {};

  jobDetail = {
    callToAction1Intro: '',
    callToAction1Title: '',
    callToAction1BodyText: '',
    callToAction1LinkText: '',
    callToAction1LinkUrl: '',
    callToAction2Intro: '',
    callToAction2Title: '',
    callToAction2BodyText: '',
    callToAction2LinkText: '',
    callToAction2LinkUrl: '',
  };
}

export class JbLibSettings {
  errors = { jobAlertTitleRequired: '', jobAlertTitleDuplicate: '' };
  settings = { defaultSearchRadius: 15, isProduction: false };
  tooltips = { unknownSalaries: '', nocCode: '', jobSource: '', onSite: '', hybrid: '', travelling: '', virtual: '' };

  filters = {
    howIsSalaryCalculatedBody: '',
    howIsSalaryCalculatedTitle: '',
    datePostedTitle: '',
    educationNote: '',
    educationTitle: '',
    industryTitle: '',
    jobTypeTitle: '',
    locationRadiusNote: '',
    locationRegionSearchLabel: '',
    locationSearchLabel: '',
    locationTitle: '',
    moreFiltersTitle: '',
    salaryTitle: '',
    keywordInputPlaceholder: ''
  };
}

export class SystemSettingsModel {
  jbAccount = new JbAccountSettings();
  jbSearch = new JbSearchSettings();
  shared = new JbLibSettings();
}
