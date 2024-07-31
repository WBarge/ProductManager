import { Component,OnInit } from '@angular/core';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { TableLazyLoadEvent, TableModule, TablePageEvent } from 'primeng/table';
import { ProductService } from '../../services/product.service';
import { TestDataResult } from '../../models/test-data-result';
import { TestData } from '../../models/test-data';
import { ButtonModule } from 'primeng/button';


@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [ToastModule, TableModule,ButtonModule],
  providers:[ProductService],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent {


  products!:TestData[];
  currentPage:number =1;
  pageSize:number = 10;
  totalItems:number = 0;

  constructor(private msgService:MessageService,private dataService:ProductService)  { }

  loadProducts(){
    this.sendMessage('info','System Message','Loading Products');
    this.dataService.getProducts(this.currentPage,this.pageSize)
      .subscribe((results:TestDataResult)=>{
        this.products = results.data;
        this.totalItems = results.totalRecordSize;
      });
  }

  loadProductsLazy(event: TableLazyLoadEvent) {
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
