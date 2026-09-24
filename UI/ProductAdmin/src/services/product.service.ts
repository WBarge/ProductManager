import { Service,inject } from '@angular/core';
import { ErrorHandlerService, HandleError } from './error-handler.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { LocationService } from './location.service';
import { catchError, map, Observable } from 'rxjs';
import { ProductsListResult } from '../models/results/productsListResult';
import { FilterDetail, FilterDictionary } from '../models/requests/filter-detail';
import { ListRequest } from '../models/requests/list-request';
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

    var request:ListRequest = {
      page: currentPage,
      pageSize: pageSize,
      filters:  filtersAsObject
    };
    return this.http.post<ProductsListResult>(this.productsServiceLocation,request)
      .pipe(
        map((results:any) => {
          const returnValue = new ProductsListResult();
          returnValue.totalRecordSize = results.totalRecordSize;
          returnValue.data = results.data.map((product:any) => {
            const tempProduct = new Product();
            tempProduct.idValue = product.id;
            tempProduct.name = product.name;
            tempProduct.shortDescription = product.shortDescription;
            tempProduct.sku = product.sku;
            tempProduct.price = product.price;
            tempProduct.description = product.description;
            tempProduct.options = product.options;
            return tempProduct;
          });
          return returnValue;
        }),
        catchError(this.handleError<ProductsListResult>('getProducts'))
      );
  }

  getProductById(productId:string):Observable<Product>{
    var requestURl = this.productServiceLocation+'/'+productId;
    return this.http.get<Product>(requestURl)
    .pipe(
      map((product:any) => {
        const tempProduct = new Product();
        tempProduct.idValue = product.id;
        tempProduct.name = product.name;
        tempProduct.shortDescription = product.shortDescription;
        tempProduct.sku = product.sku;
        tempProduct.price = product.price;
        tempProduct.description = product.description;
        tempProduct.options = product.options;
        return tempProduct;
      }),
      catchError(this.handleError<Product>('getProductById'))
    )
  }

  quickAdd(newProduct:Product):Observable<any>{
    return this.http.post(this.productServiceLocation+'/QuickAdd',newProduct)
    .pipe(
      catchError(this.handleError<any>('quickAdd'))
    );
  }

  deleteProduct(productToDelete: Product):Observable<any> {
    var requestURl = this.productServiceLocation+'/'+productToDelete.idValue;
    return this.http.delete(requestURl).pipe(catchError(this.handleError<any>('deleteProduct')));
  }
}
