import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { GlobalService } from '../../../../jb-lib/src/public-api';

@Injectable({
    providedIn: 'root'
})
export class IndustryProfileService {

  constructor(
    private http: HttpClient,
    private globalService: GlobalService
  ) { }

  getIndustryProfiles(): Observable<IndustryProfile[]> {
    return this.http.get<IndustryProfile[]>(`${this.globalService.getApiBaseUrl()}api/industry-profiles`);
  }

  get industryProfilesTotal(): Observable<number> {
    const url = `${this.globalService.getApiBaseUrl()}api/industry-profiles/total`;
    return this.http.get<number>(url);
  }

  deleteIndustryProfile(id: number): Observable<object> {
    const url = `${this.globalService.getApiBaseUrl()}api/industry-profiles/${id}`;
    return this.http.delete(url);
  }
}

export interface IndustryProfile {
  id: number;
  title: string;
  //count: number;
  profileId: string;
  //industryIds: string;
}

