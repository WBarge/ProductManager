import { Injectable } from '@angular/core';
import { ErrorHandlerService, HandleError } from './error-handler.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { LocationService } from './location.service';
import { catchError, Observable } from 'rxjs';
import { ProductsListResult } from '../models/results/productsListResult';
import { FilterDetail, FilterDictionary } from '../models/requests/filter-detail';
import { ProductListRequest } from '../models/requests/product-list-request';
import { Product } from '../models/results/product';


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
              ):Observable<ProductsListResult>{
    var filtersAsObject:any = null;
    if(filters && filters?.size>0){
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
    return this.http.post<ProductsListResult>(this.productServiceLocation,request)
    .pipe(
      catchError(this.handleError<ProductsListResult>('getProducts'))
    );
  }

  quickAdd(newProduct:Product):Observable<any>{
    return this.http.post(this.productServiceLocation+'/QuickAdd',newProduct)
    .pipe(
      catchError(this.handleError<any>('quickAdd'))
    );
  }

  deleteProduct(productToDelete: Product):Observable<any> {
    var requestURl = this.productServiceLocation+'/'+productToDelete.id
    return this.http.delete(requestURl).pipe(catchError(this.handleError<any>('deleteProduct')));
  }
}
