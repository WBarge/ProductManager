import { Component, Input, OnInit, ChangeDetectionStrategy,inject } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/results/product';
import { MessageService } from 'primeng/api';

import { FormsModule } from '@angular/forms';
import { InputTextModule} from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SplitterModule } from 'primeng/splitter';
import { TextareaModule } from 'primeng/textarea';

@Component({
    selector: 'app-product',
    imports: [InputTextModule,
      InputNumberModule,
      FormsModule,
      SplitterModule,
      TextareaModule
    ],
    providers: [ProductService],
    templateUrl: './product.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrl: './product.component.css'
})
export class ProductComponent implements OnInit {

  @Input() set productId (productId:string){
      this.pId = productId;
  }
  private dataService=inject(ProductService);
  private msgService=inject(MessageService);

  private pId:string = '';
  public _productObj:Product | null = null;

  ngOnInit(): void {
    if (this.pId) {
      this.sendMessage('info','System Message','Loading Product');
      this.dataService.getProductById(this.pId).subscribe({
        next: (product) => {
          this._productObj = product;
        }
      });
    }
  }

  private sendMessage(severity:string,summary:string,detail:string){
    this.msgService.add({severity:severity,summary:summary,detail:detail});
  }

  submit(productToAdd:Product){
    this.sendMessage('info','System Message','Adding Product');
  }
}
