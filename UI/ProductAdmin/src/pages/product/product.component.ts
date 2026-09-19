import { Component, Input, OnInit, ChangeDetectionStrategy,inject } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { OptionService } from '../../services/option.service';
import { Product } from '../../models/results/product';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { InputTextModule} from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SplitterModule } from 'primeng/splitter';
import { TextareaModule } from 'primeng/textarea';
import { ButtonModule } from 'primeng/button';
import { ScrollAreaModule } from 'primeng/scrollarea';
import { SelectModule } from 'primeng/select';
import { OptionSelection } from '../../models/results/optionSelection';
import { FieldsetModule } from 'primeng/fieldset';
import { ToolbarModule } from 'primeng/toolbar';
import { IftaLabelModule } from 'primeng/iftalabel';


@Component({
    selector: 'app-product',
    imports: [InputTextModule,
      InputNumberModule,
      FormsModule,
      SplitterModule,
      TextareaModule,
      ButtonModule,
      ScrollAreaModule,
      SelectModule,
      FieldsetModule,
      ToolbarModule,
      IftaLabelModule
    ],
    providers: [ProductService,OptionService],
    templateUrl: './product.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrl: './product.component.css'
})
export class ProductComponent implements OnInit {

  @Input() set productId (productId:string){
      this.pId = productId;
  }
  private productService=inject(ProductService);
  private optionService=inject(OptionService);
  private msgService=inject(MessageService);
  private router=inject(Router);

  private pId:string = '';
  public _productObj:Product | null = null;
  public _optionsList: OptionSelection[] = [];
  newProductOption: any ;

constructor() {
    this.newProductOption = {
      optionId: '',
      price: 0,
    };
  }

  ngOnInit(): void {
    this.optionService.getOptions().subscribe({
      next: (options) => {
        this._optionsList = options;
      }
    });
    if (this.pId) {
      this.sendMessage('info','System Message','Loading Product');
      this.productService.getProductById(this.pId).subscribe({
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
    this.sendMessage('info','System Message','Updating Product');
    this.router.navigate(['/products']);
  }

  addOptionToProduct(newProductOption: any) {
    if (this._productObj) {
      const newOption = {
        optionId: newProductOption.optionId,
        price: newProductOption.price
      };
      //this._productObj.options.push(newOption);
    }
  }
}
