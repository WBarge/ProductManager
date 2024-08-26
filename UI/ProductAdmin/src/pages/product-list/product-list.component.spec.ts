import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideAnimations } from '@angular/platform-browser/animations';
import { ProductListComponent } from './product-list.component';
import { createSpyFromClass,Spy } from 'jasmine-auto-spies';
import { MessageService } from 'primeng/api';
import { HttpClient } from '@angular/common/http';
import { ProductService } from '../../services/product.service';
import { FilterTransformerService } from '../../services/transformers/filter-transformer.service';
import { Product } from '../../models/results/product';
import { ProductsListResult } from '../../models/results/productsListResult';
import { of } from 'rxjs';

describe('ProductListComponent', () => {
  let msgServcie: Spy<MessageService>;
  let httpClient: Spy<HttpClient>;
  let productService  =  jasmine.createSpyObj('ProductService',['getProducts']);
  let filterService: Spy<FilterTransformerService>;
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;

  let fakeProducts:Product[] =[
    {
      id:"testId1",
      name:"test name",
      shortDescription:"testshortdescrip",
      sku:"testsku",
      price:23.43,
      description:"TestDescription"
    },
    {
      id:"testId2",
      name:"test name",
      shortDescription:"testshortdescrip",
      sku:"testsku",
      price:23.43,
      description:"TestDescription"

    }
  ];
  let fakeGetResult :ProductsListResult =
  {
    data:fakeProducts,
    totalRecordSize:2
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductListComponent],
      providers:[provideAnimations(),
        {provide:MessageService, useValue: createSpyFromClass(MessageService)},
        {provide:HttpClient, useValue: createSpyFromClass(HttpClient)},
        {provide:ProductService, useValue: productService},
        {provide:FilterTransformerService, useValue: createSpyFromClass(FilterTransformerService)}

      ]
    })
    .compileComponents();

    msgServcie = TestBed.inject<any>(MessageService);
    httpClient = TestBed.inject<any>(HttpClient);
    productService = TestBed.inject(ProductService);
    filterService = TestBed.inject<any>(FilterTransformerService);
    fixture = TestBed.createComponent(ProductListComponent);
  });

  it('should create', () => {
    component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

  it('should load products automatically',()=>{
    productService.getProducts.and.returnValue(of(fakeGetResult));
    component = fixture.componentInstance;
    fixture.detectChanges();
    fixture.whenStable().then(()=>{
      expect(productService.getProducts).toHaveBeenCalled;
      expect(component.products.length).toEqual(fakeGetResult.data.length)
      expect(component.products[0].id).toEqual(fakeGetResult.data[0].id);
      expect(component.totalItems).toEqual(fakeGetResult.totalRecordSize);
    });
  });
});
