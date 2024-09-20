import { iReservation } from './../models/i-reservation';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { iOrder } from '../models/i-order';
import { iOrderItem } from '../models/i-order-item';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private http: HttpClient) {}

  reservationUrl: string = `${environment.apiUrl}/Reservation`;
  orderUrl: string = `${environment.apiUrl}/Order`;

  //RESERVATION
  createReservation(
    reservation: iReservation,
    movieId: number
  ): Observable<iReservation> {
    return this.http.post<iReservation>(
      `${this.reservationUrl}/create/${movieId}`,
      reservation
    );
  }

  getAllReservation(userId: number): Observable<iReservation[]> {
    return this.http.get<iReservation[]>(
      `${this.reservationUrl}/user/${userId}`
    );
  }

  deleteReservation(id: number): Observable<iReservation> {
    return this.http.delete<iReservation>(
      `${this.reservationUrl}/delete/${id}`
    );
  }

  //ORDER
  createOrder(order: iOrder): Observable<iOrder> {
    return this.http.post<iOrder>(`${this.orderUrl}/create`, order);
  }

  getAllOrders(): Observable<iOrder[]> {
    return this.http.get<iOrder[]>(this.orderUrl);
  }

  addOrderItem(item: iOrderItem): Observable<iOrderItem> {
    return this.http.post<iOrderItem>(`${this.orderUrl}/add-item`, item);
  }

  updateItem(item: iOrderItem): Observable<iOrderItem> {
    return this.http.put<iOrderItem>(`${this.orderUrl}/update-item`, item);
  }
  deleteOrderItem(itemId: number): Observable<any> {
    return this.http.delete(`${environment}/${itemId}`);
  }
}
