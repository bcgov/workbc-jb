import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';

@Injectable()
export class CacheInterceptor implements HttpInterceptor {

  intercept(req: HttpRequest<unknown>, next: HttpHandler) {
    const httpRequest = req.clone({
      headers: req.headers.set('Cache-Control', 'no-cache')
        .set('Pragma', 'no-cache')
        .set('Expires', 'Sat, 01 Jan 2000 00:00:00 GMT')
        .set('If-Modified-Since', '0')
    });

    return next.handle(httpRequest);
  }
}
