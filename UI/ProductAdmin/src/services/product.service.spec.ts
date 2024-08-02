import { TestBed } from '@angular/core/testing';

import { ProductService } from './product.service';
import { createSpyFromClass, Spy } from 'jasmine-auto-spies';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { ErrorHandlerService } from './error-handler.service';
import { LocationService } from './location.service';
import { Observable, of } from 'rxjs';

describe('ProductService', () => {
  let service: ProductService;
  let httpSpy: Spy<HttpClient>;
  let errorHandler:Spy<ErrorHandlerService>;
  let configService:Spy<LocationService>;
  let testLocation:string = 'someLocation';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers:[
        {provide:HttpClient, useValue: createSpyFromClass(HttpClient)},
        {provide:ErrorHandlerService, useValue: createSpyFromClass(ErrorHandlerService)},
        {provide:MessageService, useValue: createSpyFromClass(MessageService)},
        {provide:LocationService, useValue: createSpyFromClass(LocationService)},
        ProductService
      ]
    });

    errorHandler = TestBed.inject<any>(ErrorHandlerService);
    errorHandler.createHandleError = MyHandleError as any;
    configService = TestBed.inject<any>(LocationService);
    configService.getLocationUrl.and.returnValue(testLocation);
    httpSpy = TestBed.inject<any>(HttpClient);
  });

  it('should be created', () => {
    service = TestBed.inject(ProductService);
    expect(service).toBeTruthy();
  });
});

function MyHandleError<T>(operation?: string, result?: T): (error: HttpErrorResponse) => Observable<T> {
  return (error: HttpErrorResponse): Observable<T> => {
    return of (result as T);
  };
}
