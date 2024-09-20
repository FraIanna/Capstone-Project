import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { iProduct } from '../models/i-product';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private http: HttpClient) {}

  productUrl = `${environment.apiUrl}/Product`;

  getAllProduct(): Observable<iProduct[]> {
    return this.http.get<iProduct[]>(this.productUrl);
  }

  createProduct(product: iProduct): Observable<iProduct> {
    return this.http.post<iProduct>(this.productUrl, product);
  }

  updateProduct(product: iProduct): Observable<iProduct> {
    return this.http.put<iProduct>(`${this.productUrl}/${product.id}`, product);
  }

  getProductById(id: number): Observable<iProduct> {
    return this.http.get<iProduct>(`${this.productUrl}/${id}`);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.productUrl}/${id}`);
  }
}
