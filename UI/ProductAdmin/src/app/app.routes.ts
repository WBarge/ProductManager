import { Routes } from '@angular/router';
import { AboutComponent } from '../pages/about/about.component';
import { ProductListComponent } from '../pages/product-list/product-list.component';
import { ProductComponent } from '../pages/product/product.component';
import { OptionListComponent } from '../pages/option-list/option-list.component';
import { OptionComponent } from '../pages/option/option.component';
import { CharacteristicListComponent } from '../pages/characteristic-list/characteristic-list.component';
import { CharacteristicComponent } from '../pages/characteristic/characteristic.component';

export const routes: Routes = [
  { path:'',redirectTo:'/products',pathMatch:'full'},
  { path:'products',component: ProductListComponent},
  { path:'product/:productId',component: ProductComponent},
  { path:'options',component: OptionListComponent},
  {path:'option/:optionId',component: OptionComponent},
  {path:'characteristics',component: CharacteristicListComponent},
  {path:'characteristic/:charId',component:CharacteristicComponent},
  { path:'about',component: AboutComponent}

];
