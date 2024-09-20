import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductComponent } from './product.component';
import { FoodComponent } from './food/food.component';
import { DrinkComponent } from './drink/drink.component';

const routes: Routes = [
  { path: '', component: ProductComponent },
  { path: 'food', component: FoodComponent },
  { path: 'drink', component: DrinkComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProductRoutingModule {}
