/** Angular Imports */
import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest ,HttpHeaders} from '@angular/common/http';

/** rxjs Imports */
import { Observable } from 'rxjs';

/** Environment Configuration */
import { environment } from 'environments/environment';
import { timeout } from 'rxjs/operators';

/**
 * Http request interceptor to prefix a request with `environment.serverUrl`.
 */
@Injectable()
export class ApiPrefixInterceptor implements HttpInterceptor {

  /**
   * Intercepts a Http request and prefixes it with `environment.serverUrl`.
   */
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    
   request = request.clone({ url: environment.serverUrl + "/api" + request.url});
    return next.handle(request).pipe(timeout(300000));
  }

}
