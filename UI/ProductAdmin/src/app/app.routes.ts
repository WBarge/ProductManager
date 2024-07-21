import { Routes } from '@angular/router';
import { AboutComponent } from '../pages/about/about.component';
import { ProductListComponent } from '../pages/product-list/product-list.component';

export const routes: Routes = [
  { path:'',redirectTo:'/products',pathMatch:'full'},
  { path:'about',component: AboutComponent},
  { path:'products',component: ProductListComponent}
];
