import { GlobalService } from '../../../../jb-lib/src/public-api';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { Region } from '../models/regions.model';


@Injectable({
    providedIn: 'root'
})
export class LocationService {

  constructor(
    private http: HttpClient,
    private global: GlobalService
  ) { }

  private get apiUrl(): string {
    return this.global.getApiBaseUrl();
  }

  getCountries() {
      return this.http.get<Country>(`${this.apiUrl}api/location/countries`);
  }

  getProvinces() {
      return this.http.get<Province>(`${this.apiUrl}api/location/provinces`);
  }

  getAllRegions() {
      return this.http.get<Region[]>(`${this.apiUrl}api/location/all-regions`);
  }

  getRegions(city: string) {
      if (!city || city.trim().length === 0) return of<Region[]>([]);

      return this.http.get<Region[]>(`${this.apiUrl}api/location/regions/${city}`);
  }

  getCities(city: string) {
    if (!city || city.trim().length === 0) return of<string[]>([]);

    return this.http.get<string[]>(`${this.apiUrl}api/location/cities/${city}`);
  }
}

export interface Country {
    id: number;
    name: string;
}

export interface Province {
    provinceId: number;
    name: string;
    shortName: string;
}
