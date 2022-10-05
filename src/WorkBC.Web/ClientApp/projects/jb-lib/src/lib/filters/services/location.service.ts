import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GlobalService } from '../../services/global.service';


@Injectable({
    providedIn: 'root'
})
export class LocationService {

    baseUrl: string;

    constructor(private http: HttpClient, private globalService: GlobalService) {
        this.baseUrl = this.globalService.getApiBaseUrl();
    }

  getCities(city: string, includeRegion = false) {
    city = city.replace('/', '_'); //replace "/" - it breaks the url structure
    const url = `${this.baseUrl}api/location/cities/${city}${includeRegion ? '/true' : ''}`;
        return this.http.get<string[]>(url);
  }

  public static getLocationTitle(location: string) {
    let title = location;
    switch (location) { // add items that have different naming to their key
      case 'MainlandSouthwest': {
        title = 'Mainland / Southwest';
      } break;
      case 'NorthCoastNechako': {
        title = 'North Coast & Nechako';
      } break;
      case 'ThompsonOkanagan': {
        title = 'Thompson-Okanagan';
      } break;
      case 'VancouverIslandCoast': {
        title = 'Vancouver Island / Coast';
      } break;
    }
    return title;
  }

  public static getLocationId(location) {
    let id = location;
    switch (location) {
      case 'Mainland / Southwest': {
        id = 'MainlandSouthwest';
      } break;
      case 'North Coast & Nechako': {
        id = 'NorthCoastNechako';
      } break;
      case 'Thompson-Okanagan': {
        id = 'ThompsonOkanagan';
      } break;
      case 'Vancouver Island / Coast': {
        id = 'VancouverIslandCoast';
      } break;
    }
    return id;
  }
}

