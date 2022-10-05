import { GlobalService } from '../../../../jb-lib/src/public-api';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterModel, Register, ForgotPasswordModel } from '../models/register.model';


const httpOptions = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json'
    })
};

@Injectable({
    providedIn: 'root'
})
export class UserService {

  constructor(
    private http: HttpClient,
    private global: GlobalService
  ) { }

  private get apiUrl(): string {
    return this.global.getApiBaseUrl();
  }

  register(user: RegisterModel) {
    const url = `${this.apiUrl}api/users/register`;
    return this.http.post(url, user);
  }

  updatePersonalSettings(user: RegisterModel) {
    const url = `${this.apiUrl}api/users/update-personal-settings`;
    return this.http.put(url, user);
  }

  changePassword(passwordResetModel: { currentPassword: string, newPassword: string }) {
      const url = `${this.apiUrl}api/users/change-pwd`;
      return this.http.put<{ succeeded: boolean, errors: { code: string, description: string }[] }>(url, passwordResetModel);
  }

  deleteAccount() {
      return this.http.delete(`${this.apiUrl}api/users`);
  }

  getCurrentUser() {
    return this.http.get<Register>(`${this.apiUrl}api/users/current`);
  }

  confirmEmail(userId: string, code: string) {
    const url = `${this.apiUrl}api/users/confirm-email`;
    return this.http.post<{ isEmailAlreadyConfirmed: boolean }>(url, { userId: userId, code: code }, httpOptions);
  }

  sendActivationEmail(email: string) {
    const url = `${this.apiUrl}api/users/send-activation-email`;
    return this.http.post(url, JSON.stringify(email), httpOptions);
  }

  sendPwdResetEmail(email: string) {
    const url = `${this.apiUrl}api/users/send-pwd-reset-email`;
    return this.http.post(url, JSON.stringify(email), httpOptions);
  }

  verifyRecaptcha(token: string) {
    const url = `${this.apiUrl}api/users/verify-recaptcha`;
    return this.http.post(url, JSON.stringify(token), httpOptions);
  }

  verifyToken(forgotPasswordModel: ForgotPasswordModel) {
    const url = `${this.apiUrl}api/users/verify-token`;
    return this.http.post<boolean>(url, forgotPasswordModel);
  }

  resetPassword(forgotPasswordModel: ForgotPasswordModel) {
    const url = `${this.apiUrl}api/users/reset-pwd`;
    return this.http.post<{ succeeded: boolean, errors: [{ code: string, description: string }] }>(url, forgotPasswordModel);
  }
}
