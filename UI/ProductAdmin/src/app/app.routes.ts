import { Routes } from '@angular/router';
import { AboutComponent } from '../pages/about/about.component';
import { ProductListComponent } from '../pages/product-list/product-list.component';
import { ProductComponent } from '../pages/product/product.component';

export const routes: Routes = [
  { path:'',redirectTo:'/products',pathMatch:'full'},
  { path:'products',component: ProductListComponent},
  { path:'product/:productId',component: ProductComponent},
  { path:'about',component: AboutComponent}

];
