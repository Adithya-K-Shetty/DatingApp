import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

//this can be injected into other component by creating instance of this
//reusablility
//specifically used for services which performs network operations
//like getting data from server etc...
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toastr: ToastrService) {}

  //used for cross cutting concers(common over multiple components)
  //Interceptors can be used to centralize error handling logic for HTTP requests
  //Interceptors can implement caching mechanisms for HTTP requests and responses
  //They can add authentication tokens or headers to outgoing requests, intercept unauthorized responses
  // They can intercept failed requests, perform retries with exponential backoff, or cancel requests that exceed specified time limits
  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      //the type HttpErrorResponse defined here
      //helps the intellisense to give suggestions
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                const modelStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modelStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modelStateErrors.flat();
              } else {
                this.toastr.error(error.error, error.status.toString());
              }
              break;
            case 401:
              this.toastr.error('Unauthorised', error.status.toString());
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {
                state: { error: error.error },
              };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        throw error;
      })
    );
  }
}
