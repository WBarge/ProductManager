import { Component,OnInit } from '@angular/core';
import { ToastModule } from 'primeng/toast';
import { FilterMetadata, MessageService } from 'primeng/api';
import { TableLazyLoadEvent, TableModule, TablePageEvent } from 'primeng/table';
import { ProductService } from '../../services/product.service';
import { TestDataResult } from '../../models/results/test-data-result';
import { TestData } from '../../models/test-data';
import { ButtonModule } from 'primeng/button';
import { FilterTransformerService } from '../../services/transformers/filter-transformer.service';


@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [ToastModule, TableModule,ButtonModule],
  providers:[ProductService,FilterTransformerService],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent {


  products!:TestData[];
  currentPage:number =1;
  pageSize:number = 10;
  totalItems:number = 0;
  filters?: {
    [s: string]: FilterMetadata | FilterMetadata[] | undefined;
  };

  constructor(private msgService:MessageService,
              private dataService:ProductService,
              private filterTransformer:FilterTransformerService)  { }

  loadProducts(){
    this.sendMessage('info','System Message','Loading Products');
    var transFormedFilters = this.filterTransformer.transformGridFilters(this.filters);
    this.dataService.getProducts(this.currentPage,this.pageSize,transFormedFilters)
      .subscribe((results:TestDataResult)=>{
        this.products = results.data;
        this.totalItems = results.totalRecordSize;
      });
  }

  loadProductsLazy(event: TableLazyLoadEvent) {
    this.filters = event.filters;
    this.loadProducts();
  }

  pageChange(event:TablePageEvent){
    this.pageSize = event.rows;
    this.currentPage = event.first/this.pageSize + 1;
  }


  private sendMessage(severity:string,summary:string,detail:string){
    this.msgService.add({severity:severity,summary:summary,detail:detail});
  }
}
