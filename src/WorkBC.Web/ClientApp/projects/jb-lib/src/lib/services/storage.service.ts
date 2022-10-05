import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private readonly rememberMe = 'rememberMe';
  private readonly impersonatingKey = 'impersonating';

  readonly savedJobsKey = 'savedJobs';
  readonly tmpSavedJobsKey = 'tmpSavedJobs';
  readonly tmpJobAlertKey = 'tmpJobAlert';
  readonly tmpCareerProfileKey = 'tmpSavedCareerProfile';
  readonly tmpIndustryProfileKey = 'tmpSavedIndustryProfile';
  readonly tmpIndustryProfileUrlKey = 'tmpSavedIndustryProfileUrl';
  readonly justLoggedInkey = 'justLoggedIn';

  readonly currentUserKey = 'currentUser';
  readonly currentUserUsernameKey = 'currentUser.username';
  readonly currentUserEmailKey = 'currentUser.email';
  readonly currentUserFirstnameKey = 'currentUser.firstname';
  readonly currentUserLastnameKey = 'currentUser.lastname';
  readonly currentUserIdKey = 'currentUser.id';
  readonly currentUserTokenKey = 'currentUser.token';

  get staySignedIn(): boolean {
    const result = localStorage.getItem(this.rememberMe);
    return (result && result.toLowerCase() === 'true') || false;
  }
  set staySignedIn(value: boolean) {
    localStorage.setItem(this.rememberMe, value.toString());
  }

  get isImpersonating(): boolean {
    const result = this.getCookie(this.impersonatingKey);
    return (result && result.toLowerCase() === 'true') || false;
  }
  set isImpersonating(value: boolean) {
    if (value) {
      this.setCookie(this.impersonatingKey, 'true');
    } else if (this.isImpersonating) {
      this.removeCookie(this.impersonatingKey);
    }
  }

  get totalSavedJobs(): number {
    const savedJobs = this.getLocalStorageItem(this.savedJobsKey);
    return savedJobs ? savedJobs.split(',').length : 0;
  }

  //get tmpData(): Job[] {
  //  let tmpSavedJobs = this.getLocalStorageItem("tmpData");
  //  return tmpSavedJobs ? JSON.parse(tmpSavedJobs.replace(new RegExp("JobsId", "g"), "JobId")) : [];
  //}

  //set tmpData(value: Job[]) {
  //  this.setLocalStorageItem("tmpData", JSON.stringify(value));
  //}

  setItem(key: string, value: string): void {
    if (this.staySignedIn) {
      localStorage.setItem(key, value);
    }
    else {
      if (key == this.currentUserKey) {
        this.setCookie(this.currentUserUsernameKey, JSON.parse(value).username);
        this.setCookie(this.currentUserEmailKey, JSON.parse(value).email);
        this.setCookie(this.currentUserLastnameKey, JSON.parse(value).lastName);
        this.setCookie(this.currentUserFirstnameKey, JSON.parse(value).firstName);
        this.setCookie(this.currentUserIdKey, JSON.parse(value).id);
        this.setCookie(this.currentUserTokenKey, JSON.parse(value).token);
      }
      else {
        this.setCookie(key, value);
      }
    }
  }

  getItem(key: string): string | null {
    if (this.staySignedIn) {
      return localStorage.getItem(key);
    }
    else {
      if (key == this.currentUserKey) {
        if (this.getCookie(this.currentUserIdKey) != null) {
          const currentUsername = this.getCookie(this.currentUserUsernameKey);
          const currentFirstname = this.getCookie(this.currentUserFirstnameKey);
          const currentLastname = this.getCookie(this.currentUserLastnameKey);
          const currentEmail = this.getCookie(this.currentUserEmailKey);
          const currentToken = this.getCookie(this.currentUserTokenKey);
          const currentId = this.getCookie(this.currentUserIdKey);

          const currentUser = { username: currentUsername, firstName: currentFirstname, lastName: currentLastname, email: currentEmail, token: currentToken, id: currentId };
          return JSON.stringify(currentUser);
        }
        else return null;
      }
      else {
        return this.getCookie(key);
      }
    }
  }

  removeItem(key: string): void {
    if (key === this.currentUserKey) {
      this.removeCookie(this.currentUserUsernameKey);
      this.removeCookie(this.currentUserFirstnameKey);
      this.removeCookie(this.currentUserLastnameKey);
      this.removeCookie(this.currentUserEmailKey);
      this.removeCookie(this.currentUserTokenKey);
      this.removeCookie(this.currentUserIdKey);
    }

    localStorage.removeItem(key);
    this.removeCookie(key);
  }

  getLocalStorageItem(key: string): string | null {
    return localStorage.getItem(key);
  }

  setLocalStorageItem(key: string, value: string): void {
    localStorage.setItem(key, value);
  }

  removeLocalStorageItem(key: string): void {
    localStorage.removeItem(key);
  }

  private setCookie(key: string, value: string): void {
    document.cookie = key.trim() + '=' + value + ';Path=/;';
  }

  private getCookie(key: string): string | null {
    const keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
    return keyValue ? keyValue[2] : null;
  }

  private removeCookie(key: string): void {
    document.cookie = key + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC; Path=/;';
  }
}
