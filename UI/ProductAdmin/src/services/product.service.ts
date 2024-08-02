import { Injectable } from '@angular/core';
import { ErrorHandlerService, HandleError } from './error-handler.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { LocationService } from './location.service';
import { catchError, Observable } from 'rxjs';
import { TestDataResult } from '../models/results/test-data-result';
import { FilterDetail, FilterDictionary } from '../models/requests/filter-detail';
import { ProductListRequest } from '../models/requests/product-list-request';


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

   getProducts( currentPage: number =1 ,
                pageSize: number = 10 ,
                filters?:FilterDictionary
              ):Observable<TestDataResult>{
    var filtersAsObject:any = null;
    if(filters && filters?.size>0)
    {
      filtersAsObject = {};
      filters?.forEach((value:FilterDetail[],key:string)=>{
        var tempObj = {[key]:value};
        filtersAsObject = Object.assign(filtersAsObject,tempObj);
      });

    }
    var request:ProductListRequest = {
      page: currentPage,
      pageSize: pageSize,
      filters:  filtersAsObject
    };
    var temp:string = JSON.stringify(request);
    return this.http.post<TestDataResult>(this.productServiceLocation,request)
    .pipe(
      catchError(this.handleError<TestDataResult>('getProducts'))
    );

    function replacer(key:any, value:FilterDictionary) {
      if(value instanceof Map && value.size > 0) {
//        var returnValue:object = {};
//        value.forEach((value:FilterDetail[],key:string)=>{
          return { [key]:JSON.stringify(value)};
//          returnValue = Object.assign(returnValue,tempVar);
//        });
//        return returnValue;

//        {
//          dataType: 'Map',
//          value: Array.from(value.entries()), // or with spread: value: [...value]
//        };
      } else {
        return;
      }
    }
  }
}
