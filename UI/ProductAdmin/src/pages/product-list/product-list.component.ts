import { Component,OnInit } from '@angular/core';
import {CurrencyPipe} from '@angular/common';
import { FilterMetadata, MessageService } from 'primeng/api';
import { TableLazyLoadEvent, TableModule, TablePageEvent } from 'primeng/table';
import { ProductService } from '../../services/product.service';
import { ProductsListResult } from '../../models/results/productsListResult';
import { FieldsetModule } from 'primeng/fieldset';
import  {FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule} from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { FilterTransformerService } from '../../services/transformers/filter-transformer.service';
import { Product } from '../../models/results/product';
import { FormsModule } from '@angular/forms';



@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [TableModule,
            ButtonModule,
            CurrencyPipe,
            FieldsetModule,
            FloatLabelModule,
            InputTextModule,
            FormsModule,
            InputNumberModule],

  providers:[ProductService,FilterTransformerService],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit{
  products!:Product[];
  currentPage:number =1;
  pageSize:number = 10;
  totalItems:number = 0;
  filters?: {
    [s: string]: FilterMetadata | FilterMetadata[] | undefined;
  };
  newProduct: Product;

  constructor(private msgService:MessageService,
              private dataService:ProductService,
              private filterTransformer:FilterTransformerService)  {
      this.newProduct = new Product();
    }

  public ngOnInit(): void {

  }
  loadProducts(){
    this.sendMessage('info','System Message','Loading Products');
    var transFormedFilters = this.filterTransformer?.transformGridFilters(this.filters);
    this.dataService.getProducts(this.currentPage,this.pageSize,transFormedFilters)
      .subscribe((results:ProductsListResult)=>{
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

  submit(productToAdd:Product){
    this.dataService.quickAdd(productToAdd).subscribe(()=>{
      this.products.push(productToAdd);
    });
    this.newProduct = new Product();
  }

  delete(productToDelete:Product){
    this.dataService?.deleteProduct(productToDelete).subscribe(()=>{
      this.products = this.products.filter(p=>p.id!= productToDelete.id);
    });
  }

  private sendMessage(severity:string,summary:string,detail:string){
    this.msgService.add({severity:severity,summary:summary,detail:detail});
  }
}
