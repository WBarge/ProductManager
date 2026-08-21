import { Component, Input, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-product',
    imports: [],
    templateUrl: './product.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrl: './product.component.css'
})
export class ProductComponent {
  @Input() set productId (productId:string){
      this.pId = productId;
  }

  public pId:string = '';
}
