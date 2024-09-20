import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ProductRoutingModule } from './product-routing.module';
import { ProductComponent } from './product.component';
import { DrinkComponent } from './drink/drink.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [ProductComponent, DrinkComponent],
  imports: [CommonModule, ProductRoutingModule, NgbModule],
})
export class ProductModule {}
