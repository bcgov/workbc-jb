import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { MainFilterModel } from '../models/filter.model';
import {
  Results,
  GoogleMapData,
  LocationInformation,
  NocCode,
  Job,
  JobBase,
  MapDataCls
} from '../models/job.model';
import { BaseService } from './base.service';
import { GlobalService } from '../../services/global.service';

@Injectable({
  providedIn: 'root'
})
export class DataService extends BaseService {
  //Base URL for API
  baseUrl: string;

  //HTTP Headers
  private readonly headerOptions = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient, private globalService: GlobalService) {
    super();
    this.baseUrl = this.globalService.getApiBaseUrl();
  }

  getResults(filter: MainFilterModel): Observable<Results<JobBase>> {
    //Object to POST
    const body =  filter.convertToElasticSearchJobSearchFilters();

    //HTTP POST
    return this.http.post<Results<JobBase>>(
      this.baseUrl + 'api/Search/JobSearch',
      body,
      { headers: this.headerOptions }
    );
  }

  getMapResults(filter: MainFilterModel): Observable<MapDataCls> {
    //Object to POST
    const body = filter.convertToElasticSearchJobSearchFilters();

    //HTTP POST
    const result = this.http.post<MapDataCls>(
      this.baseUrl + 'api/Search/GetMapData',
      body,
      { headers: this.headerOptions }
    );
    return result;
  }

  getLocationInformation(jobIds: string): Observable<LocationInformation[]> {
    //HTTP POST
    return this.http.post<LocationInformation[]>(
      this.baseUrl + 'api/Search/GetLocationInformation',
      JSON.stringify(jobIds),
      { headers: this.headerOptions }
    );
  }

  getJobDetail(jobId, language, isToggle): Observable<Results<Job>> {
    let parms = new HttpParams();
    parms = parms.append('jobId', jobId);
    parms = parms.append('language', language);
    parms = parms.append('isToggle', isToggle);

    const options = { params: parms };

    //HTTP GET
    return this.http.get<Results<Job>>(
      this.baseUrl + 'api/Search/GetJobDetail',
      options
    );
  }

  getNocCodes(partialNocCode: string): Observable<NocCode[]> {
    if (!partialNocCode) return of<NocCode[]>([]);

    const result = this.http.get<NocCode[]>(
      `${this.baseUrl}api/Search/SearchNocCodes/${partialNocCode}`
    );
    return result;
  }
}
