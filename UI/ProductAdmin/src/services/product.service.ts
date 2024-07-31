import { Injectable } from '@angular/core';
import { ErrorHandlerService, HandleError } from './error-handler.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { LocationService } from './location.service';
import { catchError, Observable } from 'rxjs';
import { TestDataResult } from '../models/test-data-result';


@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private productServiceLocation:string;
  private handleError: HandleError;

  constructor(private http: HttpClient,private location:LocationService,httpErrorHandler: ErrorHandlerService) {
    this.productServiceLocation = this.location.getLocationUrl()+'Products';
    this.handleError = httpErrorHandler.createHandleError('ProductService');
  }

   getProducts(currentPage: number =1 , pageSize: number = 10):Observable<TestDataResult>{
    const paras:HttpParams = new HttpParams()
      .set('page',currentPage)
      .set('pageSize',pageSize);
    return this.http.get<TestDataResult>(this.productServiceLocation,{params:paras})
    .pipe(
      catchError(this.handleError<TestDataResult>('getProducts',))
    );
  }
}
