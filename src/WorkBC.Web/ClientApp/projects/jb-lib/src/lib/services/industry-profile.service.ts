import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GlobalService } from './global.service';

@Injectable({
  providedIn: 'root'
})
export class IndustryProfileService {
  constructor(private http: HttpClient, private globalService: GlobalService) {}

  saveIndustryProfile(naics: string): Observable<boolean> {
    const url = `${this.globalService.getApiBaseUrl()}api/industry-profiles/save/` + naics;
    const result = this.http.post<boolean>(url, null);
    return result;
  }
}
