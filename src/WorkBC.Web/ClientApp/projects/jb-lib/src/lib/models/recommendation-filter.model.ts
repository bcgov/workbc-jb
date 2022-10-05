export class JobSeekerFlags {
  constructor(
    public isYouth = false,
    public isIndigenous = false,
    public isNewcomer = false,
    public isApprentice = false,
    public isMatureWorker = false,
    public hasDisability = false,
    public isStudent = false,
    public isVeteran = false,
    public isMinority = false
  ) {}
}

export class RecommendationFilterVm extends JobSeekerFlags {
  constructor(
    jobSeekerFlags: JobSeekerFlags = null,
    public hasSameJobTitle = false,
    public hasSameNoc = false,
    public hasSameEmployer = false,
    public inTheSameCity = false
  ) {
    super(
      jobSeekerFlags ? jobSeekerFlags.isYouth : false,
      jobSeekerFlags ? jobSeekerFlags.isIndigenous : false,
      jobSeekerFlags ? jobSeekerFlags.isNewcomer : false,
      jobSeekerFlags ? jobSeekerFlags.isApprentice : false,
      jobSeekerFlags ? jobSeekerFlags.isMatureWorker : false,
      jobSeekerFlags ? jobSeekerFlags.hasDisability : false,
      jobSeekerFlags ? jobSeekerFlags.isStudent : false,
      jobSeekerFlags ? jobSeekerFlags.isVeteran : false,
      jobSeekerFlags ? jobSeekerFlags.isMinority : false
    );
  }
}

export class RecommendationFilterInput extends RecommendationFilterVm {
  constructor(
    public city = '',
    public totalSavedJobs = 0,
    jobSeekerFlags: JobSeekerFlags = null
  ) {
    super(jobSeekerFlags);
  }
}
