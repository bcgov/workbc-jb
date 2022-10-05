import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GlobalService } from './global.service';
import {
  Job,
  SavedJob,
  RecommendedJobResult,
  RecommendedJobFilter,
  RecommendedJob
} from '../filters/models/job.model';
import { SavedJobNoteModel } from '../models/saved-job-note.model';
import { JobAlertModel } from '../models/job-alert.model';
import { Observable } from 'rxjs';
import {
  RecommendationFilterVm,
  JobSeekerFlags
} from '../models/recommendation-filter.model';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
    //'Authorization': 'my-auth-token'
  })
};

@Injectable({
  providedIn: 'root'
})
export class JobService {
  constructor(private http: HttpClient, private globalService: GlobalService) {}

  getSavedJobIds(): Observable<number[]> {
    const url = `${this.globalService.getApiBaseUrl()}api/savedjobs/saved-job-ids`;
    const result = this.http.get<number[]>(url);
    return result;
  }

  getSavedJobs(): Observable<SavedJob[]> {
    const url = `${this.globalService.getApiBaseUrl()}api/savedjobs/saved-jobs`;
    return this.http.get<SavedJob[]>(url);
  }

  getJobs(savedJobs: SavedJob[]): Job[] {
    const result: Job[] = [];
    for (const savedJob of savedJobs) {
      result.push(new Job(savedJob));
    }
    return result;
  }

  saveJobs(jobIds: string): Observable<unknown> {
    const url = `${this.globalService.getApiBaseUrl()}api/savedjobs/save-jobs/${jobIds}`;
    //httpOptions.headers = httpOptions.headers.set('Authorization', 'Bearer ' + currentUser.token);
    const result = this.http.post(url, null, httpOptions);
    return result;
  }

  deleteSavedJob(jobId: string): Observable<unknown> {
    const url = `${this.globalService.getApiBaseUrl()}api/savedjobs/delete-saved-job/${jobId}`;
    return this.http.delete(url);
  }

  saveJobNote(savedJobNoteModel: SavedJobNoteModel): Observable<unknown> {
    const url = `${this.globalService.getApiBaseUrl()}api/savedjobs/save-job-note`;
    return this.http.put(url, savedJobNoteModel);
  }

  saveJobAlert(jobAlertModel: JobAlertModel): Observable<unknown> {
    const url = `${this.globalService.getApiBaseUrl()}api/jobalerts/save-job-alert`;
    return this.http.post(url, jobAlertModel);
  }

  getJobAlertsTotal(): Observable<number> {
    const url = `${this.globalService.getApiBaseUrl()}api/jobalerts/total`;
    return this.http.get<number>(url);
  }

  getJobAlerts(): Observable<JobAlertModel[]> {
    const url = `${this.globalService.getApiBaseUrl()}api/jobalerts`;
    const result = this.http.get<JobAlertModel[]>(url);
    return result;
  }

  getJobAlert(id: number): Observable<JobAlertModel> {
    const url = `${this.globalService.getApiBaseUrl()}api/jobalerts/${id}`;
    return this.http.get<JobAlertModel>(url);
  }

  deleteJobAlert(id: number): Observable<unknown> {
    const url = `${this.globalService.getApiBaseUrl()}api/jobalerts/delete-job-alert/${id}`;
    return this.http.delete(url);
  }

  getTotalMatchedJobs(filtersStr: string): Observable<number> {
    const url = `${this.globalService.getApiBaseUrl()}api/jobalerts/total-matched-jobs`;
    const result = this.http.post<number>(url, JSON.stringify(filtersStr), httpOptions);
    return result;
  }

  getUrlParameters(jobAlertId: number, aspNetUserId: string): Observable<string> {
    const url = `${this.globalService.getApiBaseUrl()}api/jobalerts/get-url-params?nid=${jobAlertId}&jsid=${aspNetUserId}`;
    const result = this.http.get<string>(url);
    return result;
  }

  private getRecommendedJobFilter(filter: RecommendedJobFilter): RecommendedJobFilter {
    let result = filter;

    if (
      !result.filterSavedJobTitles &&
      !result.filterSavedJobNocs &&
      !result.filterSavedJobEmployers &&
      !result.filterJobSeekerCity &&
      !result.filterIsApprentice &&
      !result.filterIsIndigenous &&
      !result.filterIsMatureWorkers &&
      !result.filterIsNewcomers &&
      !result.filterIsPeopleWithDisabilities &&
      !result.filterIsStudents &&
      !result.filterIsVeterans &&
      !result.filterIsVisibleMinority &&
      !result.filterIsYouth
    ) {
      result = new RecommendedJobFilter(
        result.page,
        result.pageSize,
        result.sortOrder,
        new RecommendationFilterVm(
          new JobSeekerFlags(
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
          ),
          false,
          false,
          false,
          false
        )
      );
    }

    return result;
  }

  getRecommendedJobs(
    filter: RecommendedJobFilter
  ): Observable<RecommendedJobResult<RecommendedJob>> {
    const url = `${this.globalService.getApiBaseUrl()}api/recommended-jobs`;
    return this.http.post<RecommendedJobResult<RecommendedJob>>(
      url,
      this.getRecommendedJobFilter(filter)
    );
  }

  getRecommendedJobsTotal(): Observable<number> {
    const url = `${this.globalService.getApiBaseUrl()}api/recommended-jobs/count`;
    return this.http.get<number>(url);
  }
}
