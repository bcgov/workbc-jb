import { Injectable } from '@angular/core';
import { getLocationInfo } from '../../shared/constants';

@Injectable({
    providedIn: 'root'
})
export class PostalCodeService {

  public static formatPostalCode(postalCode: string): string {
    if (!postalCode) {
      return '';
    }
    const p = postalCode.toUpperCase().replace(/\s/g, '');
    if (p.length !== 6) {
      return postalCode;
    } else {
      return p.substring(0, 3) + ' ' + p.substring(3, 6);
    }
  }

  public static unformatPostalCode(postalCode: string): string {
    return PostalCodeService.formatPostalCode(postalCode).replace(' ', '');
  }

  public static isPostalCode(s: string): boolean {
    if (!s) {
      return false;
    }
    const p = s.replace(/\s/g, '').toUpperCase();
    if (p.length !== 6) {
      return false;
    }
    const regex = /^[A-Z][0-9][A-Z][0-9][A-Z][0-9]$/;
    return regex.exec(p) !== null;
  }

  public static getLocationTag(region: string, city: string, postal: string, radius: number): string {
    if (region !== '') {
      return 'Location: ' + region;
    }

    if (city !== '') {
      if (radius === -1) {
        return 'Location: ' + city;
      }

      return getLocationInfo(radius, city);
    }

    if (postal !== '') {
      if (radius === -1) {
        return 'Location: ' + this.formatPostalCode(postal);
      }

      return getLocationInfo(radius, this.formatPostalCode(postal));
    }

    // debug condition.  Should never be hit
    return 'Location: Empty';
  }

  public static hasNumber(myString): boolean {
    const regExp = new RegExp(/\d/);
    return regExp.test(myString);
  }

  public static isPotentialCity(cityOrPostcode: string): boolean {
    cityOrPostcode = cityOrPostcode.replace(/ +(?= )/g, '').trim();
    return !this.hasNumber(cityOrPostcode) || cityOrPostcode.length > 7;
  }

  public static isPotentialPostal(cityOrPostcode: string): boolean {
    cityOrPostcode = cityOrPostcode.replace(/ +(?= )/g, '').trim();
    return this.hasNumber(cityOrPostcode) && cityOrPostcode.length <= 7;
  }

  public static isOutOfProvince(cityOrPostcode: string): boolean {
    if (!this.isPostalCode(cityOrPostcode)) {
      return false;
    }

    return cityOrPostcode.trim().toUpperCase().charAt(0) != 'V';
  }

  public static isInvalidPostalCode(postalCode: string): boolean {
    // blank postal codes are valid (ignored)
    if (!postalCode || postalCode.trim().length === 0) {
      return false;
    }

    const regExp = new RegExp(/([ABCEGHJKLMNPRSTVXY]\d)([ABCEGHJKLMNPRSTVWXYZ]\d){2}/i);

    return !regExp.test(postalCode.trim().replace(/\W+/g, ''));
  }
}
