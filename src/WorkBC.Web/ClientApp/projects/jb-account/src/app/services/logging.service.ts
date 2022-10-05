import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoggingService {

    error(message?: unknown, ...optionalParams: unknown[]): void {
        console.error(message, optionalParams);
    }

    log(message?: unknown, ...optionalParams: unknown[]): void {
        console.log(message, optionalParams);
    }

    warn(message?: unknown, ...optionalParams: unknown[]): void {
        console.warn(message, optionalParams);
    }

}


