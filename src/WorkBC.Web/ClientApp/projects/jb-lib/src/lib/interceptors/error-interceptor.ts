import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthenticationService } from '../services/authentication.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private authService: AuthenticationService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => this.handleError(error))
    );
  }

  private handleError(httpErrorResponse: HttpErrorResponse) {
    if (httpErrorResponse.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', httpErrorResponse.error.message);
    } else {
      if (httpErrorResponse.status === 401) {
        // auto logout if 401 response returned from api
        this.authService.logout();
        location.reload();
      }
    }

    let error: string;
    if (httpErrorResponse.error && httpErrorResponse.error.message) {
      error = `${httpErrorResponse.error.message}`;
      if (httpErrorResponse.error.field) {
        // some errors return a message and a field ID.  Some just return a message.
        // each Angular Component will require some minor updates in order to handle a field ID 
        return throwError({ message: error, field: +httpErrorResponse.error.field });
      }
    } else {
      if (httpErrorResponse.status === 200) {
        error = 'Server did not return a valid JSON response';
      } else if (httpErrorResponse.status === 400) {
        error = 'Bad request (400)';
      } else if (httpErrorResponse.status === 500) {
        error = 'Server error (500)';
      } else {
        error = 'Unfortunately, we were not able to contact our server. Please try again shortly. If the issue persists, please contact us by phone or email.'
      }
    }

    //console.error(error);
    return throwError(error);
  }
}

