import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { GlobalService } from '../../../../jb-lib/src/public-api';

@Injectable({
    providedIn: 'root'
})
export class CareerProfileService {

  constructor(
    private http: HttpClient,
    private globalService: GlobalService
  ) { }

  getCareerProfiles(): Observable<CareerProfile[]> {
    return this.http.get<CareerProfile[]>(`${this.globalService.getApiBaseUrl()}api/career-profiles`);
  }

  get careerProfilesTotal(): Observable<number> {
    const url = `${this.globalService.getApiBaseUrl()}api/career-profiles/total`;
    return this.http.get<number>(url);
  }

  deleteCareerProfile(id: number): Observable<object> {
    const url = `${this.globalService.getApiBaseUrl()}api/career-profiles/${id}`;
    return this.http.delete(url);
  }
}

export interface CareerProfile {
  id: number;
  title: string;
  nocCode: string;
}

