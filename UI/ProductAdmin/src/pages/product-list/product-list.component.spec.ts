import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductListComponent } from './product-list.component';
import { createSpyFromClass } from 'jasmine-auto-spies';
import { MessageService } from 'primeng/api';
import { HttpClient } from '@angular/common/http';
import { ProductService } from '../../services/product.service';
import { FilterTransformerService } from '../../services/transformers/filter-transformer.service';

describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductListComponent],
      providers:[
        {provide:MessageService, useValue: createSpyFromClass(MessageService)},
        {provide:HttpClient, useValue: createSpyFromClass(HttpClient)},
        {provide:ProductService, useValue: createSpyFromClass(ProductService)},
        {provide:FilterTransformerService, useValue: createSpyFromClass(FilterTransformerService)}

      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
    //fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
