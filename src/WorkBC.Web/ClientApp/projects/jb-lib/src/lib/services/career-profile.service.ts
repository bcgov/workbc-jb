import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GlobalService } from './global.service';

@Injectable({
  providedIn: 'root'
})
export class CareerProfileService {
  constructor(private http: HttpClient, private globalService: GlobalService) {}

  saveCareerProfile(noc: string): Observable<boolean> {
    const url = `${this.globalService.getApiBaseUrl()}api/career-profiles/save/` + noc;
    const result = this.http.post<boolean>(url, null);
    return result;
  }
}
