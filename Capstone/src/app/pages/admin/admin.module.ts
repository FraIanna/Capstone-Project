import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';
import { MovieManagementComponent } from './movie-management/movie-management.component';
import { HallManagementComponent } from './hall-management/hall-management.component';
import { ProductManagementComponent } from './product-management/product-management.component';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AdminComponent,
    MovieManagementComponent,
    HallManagementComponent,
    ProductManagementComponent,
  ],
  imports: [CommonModule, AdminRoutingModule, ReactiveFormsModule],
})
export class AdminModule {}
