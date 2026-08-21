import { Service,inject } from '@angular/core';
import { ErrorHandlerService, HandleError } from './error-handler.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { LocationService } from './location.service';
import { catchError, Observable } from 'rxjs';
import { ProductsListResult } from '../models/results/productsListResult';
import { FilterDetail, FilterDictionary } from '../models/requests/filter-detail';
import { ProductListRequest } from '../models/requests/product-list-request';
import { Product } from '../models/results/product';


@Service()
export class ProductService {
  private http=inject(HttpClient);
  private location=inject(LocationService);
  private httpErrorHandler=inject( ErrorHandlerService);

  private productsServiceLocation:string;
  private productServiceLocation:string;
  private handleError: HandleError;



  constructor() {
    this.productsServiceLocation = this.location.getLocationUrl()+'Products';
    this.productServiceLocation = this.location.getLocationUrl()+'Product';
    this.handleError = this.httpErrorHandler.createHandleError('ProductService');
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
    return this.http.post<ProductsListResult>(this.productsServiceLocation,request)
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
