import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GlobalService } from './global.service';
import {
  SystemSettingsModel,
  JbAccountSettings,
  JbSearchSettings,
  JbLibSettings
} from '../models/system-settings.model';

@Injectable({
  providedIn: 'root'
})
export class SystemSettingsService {
  private settings = new SystemSettingsModel();

  get jbSearch(): JbSearchSettings {
    return this.settings.jbSearch;
  }

  get jbAccount(): JbAccountSettings {
    return this.settings.jbAccount;
  }

  get shared(): JbLibSettings {
    return this.settings.shared;
  }

  constructor(private http: HttpClient, private globalService: GlobalService) {
    this.http
      .get<SystemSettingsModel>(
        this.globalService.getApiBaseUrl() + 'api/SystemSettings/ClientSettings'
      )
      .subscribe(data => {
        this.settings = data;
      });
  }
}
