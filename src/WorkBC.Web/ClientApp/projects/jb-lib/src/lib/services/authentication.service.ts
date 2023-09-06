import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../models/user.model';
import { LoginModel } from '../models/login.model';
import { StorageService } from './storage.service';
import { GlobalService } from './global.service';
import { JobService } from './job.service';
import { CareerProfileService } from './career-profile.service';
import { IndustryProfileService } from './industry-profile.service';
import { JobAlertModel } from '../models/job-alert.model';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  readonly currentUserKey = 'currentUser';

  currentUserSubject: BehaviorSubject<User>;
  currentUser$: Observable<User>;

  // todo: remove the following line later on
  readonly bypassLogin = false;

  constructor(
    private http: HttpClient,
    private storageService: StorageService,
    private jobService: JobService,
    private globalService: GlobalService,
    private careerProfileService: CareerProfileService,
    private industryProfileService: IndustryProfileService
  ) {
    this.currentUserSubject = new BehaviorSubject<User>(
      JSON.parse(storageService.getItem(this.currentUserKey))
    );
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  get currentUser(): User {
    return this.currentUserSubject.value;
  }

  private handleTmpSavedJob(): void {
    const tmpSavedJobs = this.storageService.getLocalStorageItem(
      this.storageService.tmpSavedJobsKey
    );
    if (tmpSavedJobs) {
      this.jobService.saveJobs(tmpSavedJobs).subscribe(() => {
        this.storageService.removeLocalStorageItem(
          this.storageService.tmpSavedJobsKey
        );
        this.resetLocalStorageSavedJobs();
      });
    } else {
      this.resetLocalStorageSavedJobs();
    }
  }

  private handleTmpJobAlert(): void {
    const tmpJobAlert = this.storageService.getLocalStorageItem(
      this.storageService.tmpJobAlertKey
    );
    if (tmpJobAlert) {
      const jobAlertModel: JobAlertModel = JSON.parse(tmpJobAlert);
      jobAlertModel.overwriteExisting = true;
      this.jobService
        .saveJobAlert(jobAlertModel)
        .toPromise().then(() => {
        //.subscribe(() => {
          this.storageService.removeLocalStorageItem(
            this.storageService.tmpJobAlertKey
          );
        });
    }
  }

  private handleTmpCareerProfile(): void {
    const tmpCareerProfile = this.storageService.getLocalStorageItem(
      this.storageService.tmpCareerProfileKey
    );
    if (tmpCareerProfile) {
      this.careerProfileService
        .saveCareerProfile(tmpCareerProfile)
        .toPromise().then(() => {
          this.storageService.removeLocalStorageItem(
            this.storageService.tmpCareerProfileKey
          );

          //take the user back to the career page
          const profileIds = tmpCareerProfile.split(','); //check when there are multiple id's in the string

          // reload the page to update the total number of career profiles saved
          window.location.reload();
        });
    }
  }

  private handleTmpIndustryProfile(): void {
    const tmpIndustryProfile = this.storageService.getLocalStorageItem(
      this.storageService.tmpIndustryProfileKey
    );
    if (tmpIndustryProfile) {
      this.industryProfileService
        .saveIndustryProfile(tmpIndustryProfile)
        .toPromise().then(() => {
          this.storageService.removeLocalStorageItem(
            this.storageService.tmpIndustryProfileKey
          );

          //get the return url
          const url = this.storageService.getLocalStorageItem(this.storageService.tmpIndustryProfileUrlKey);

          //remove return url from local storage
          this.storageService.removeLocalStorageItem(
            this.storageService.tmpIndustryProfileUrlKey
          );

          // reload the page to update the total number of industry profiles saved
          window.location.reload();
        });
    }
  }

  login(loginModel: LoginModel): Observable<User> {
    const url = `${this.globalService.getApiBaseUrl()}api/users/authenticate`;
    return this.http.post<User>(url, loginModel).pipe(
      map(user => {
        // login successful if there's a jwt token in the response
        if (user && user.token) {
          // store user details and jwt token locally to keep user logged in
          this.storageService.setItem(
            this.currentUserKey,
            JSON.stringify(user)
          );
          this.currentUserSubject.next(user);

          this.handleTmpSavedJob();
          this.handleTmpJobAlert();
          this.handleTmpCareerProfile();
          this.handleTmpIndustryProfile();

          this.storageService.isImpersonating = false;

          this.storageService.setLocalStorageItem(
            this.storageService.justLoggedInkey, '1'
          );

          // trigger a custom event so the Kentico header will update
          window.dispatchEvent(new CustomEvent('jobboardlogin'));
        }

        return user;
      })
    );
  }

  private resetLocalStorageSavedJobs(): void {
    this.storageService.removeLocalStorageItem(
      this.storageService.savedJobsKey
    );
    this.jobService.getSavedJobIds().subscribe(saveJobs => {
      if (saveJobs) {
        this.storageService.setLocalStorageItem(
          this.storageService.savedJobsKey,
          saveJobs.join()
        );
      }
    });
  }

  logout(): void {
    // remove user from local storage to log user out
    this.storageService.removeItem(this.currentUserKey);
    this.storageService.removeItem(this.storageService.savedJobsKey);
    this.storageService.isImpersonating = false;
    this.currentUserSubject.next(null);
  }

  impersonate(token: string): Observable<User> {
    const url = `${this.globalService.getApiBaseUrl()}api/users/impersonate`;
    return this.http.post<User>(url, { token: token }).pipe(
      map(user => {
        // login successful if there's a jwt token in the response
        if (user && user.token) {
          // store user details and jwt token locally to keep user logged in
          this.storageService.setItem(
            this.currentUserKey,
            JSON.stringify(user)
          );
          this.currentUserSubject.next(user);
          this.storageService.isImpersonating = true;

          this.storageService.setLocalStorageItem(
            this.storageService.justLoggedInkey, '1'
          );

          // get the saved jobs
          this.resetLocalStorageSavedJobs();

          // clear other keys
          this.storageService.removeItem(this.storageService.tmpSavedJobsKey);
          this.storageService.removeItem(this.storageService.tmpJobAlertKey);

          // trigger a custom event so the Kentico header will update
          window.dispatchEvent(new CustomEvent('jobboardlogin'));
        }

        return user;
      })
    );
  }

}
