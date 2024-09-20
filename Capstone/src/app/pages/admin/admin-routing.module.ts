import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { MovieManagementComponent } from './movie-management/movie-management.component';
import { HallManagementComponent } from './hall-management/hall-management.component';
import { ProductManagementComponent } from './product-management/product-management.component';

const routes: Routes = [
  { path: '', component: AdminComponent },
  { path: 'movie-management', component: MovieManagementComponent },
  { path: 'hall-management', component: HallManagementComponent },
  { path: 'product-management', component: ProductManagementComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
