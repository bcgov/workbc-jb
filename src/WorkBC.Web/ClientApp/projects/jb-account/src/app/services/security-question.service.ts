import { GlobalService } from '../../../../jb-lib/src/public-api';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';


@Injectable({
    providedIn: 'root'
})
export class SecurityQuestionService {

  constructor(
    private http: HttpClient,
    private global: GlobalService
  ) { }

  getSecurityQuestions() {
    return this.http.get<SecurityQuestion>(`${this.global.getApiBaseUrl()}api/security-questions`);
  }

}

export interface SecurityQuestion {
    id: number;
    questionText: string;
}
