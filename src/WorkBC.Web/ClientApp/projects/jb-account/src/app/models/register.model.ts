import { JobSeekerFlags } from './job-seeker-flags.model';
import { UserBase } from '../../../../jb-lib/src/public-api';

export interface Register extends UserBase {
    countryId: number;
    provinceId: number;
    city: string;
    locationId: number;
    securityQuestionId: number;
    securityAnswer: string;

    jobSeekerFlags: JobSeekerFlags;
}

export class RegisterModel implements Register {
    username: string;

    constructor(
        public email: string = '',
        public password: string = '',
        public firstName: string = '',
        public lastName: string = '',
        public countryId: number = 0,
        public provinceId: number = 0,
        public city: string = '',
        public locationId: number = 0,
        public securityQuestionId: number = 0,
        public securityAnswer: string = '',
        public jobSeekerFlags: JobSeekerFlags = null
    ) {
        if (email) {
            this.username = email;
        }
    }
}

export interface ForgotPasswordModel {
  email: string;
  token: string;
  newPassword?: string;
}
