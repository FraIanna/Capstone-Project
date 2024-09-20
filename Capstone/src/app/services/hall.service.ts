import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { iHall } from '../models/i-hall';
import { iHallSeat } from '../models/i-hall-seat';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class HallService {
  apiHallUrl = 'https://localhost:7297/Hall';

  constructor(private http: HttpClient) {}

  getAllHalls(): Observable<iHall[]> {
    return this.http.get<iHall[]>(this.apiHallUrl);
  }

  getHallById(id: number): Observable<iHall> {
    return this.http.get<iHall>(`${this.apiHallUrl}/${id}`);
  }

  createHall(hall: iHall): Observable<iHall> {
    return this.http.post<iHall>(`${this.apiHallUrl}/create`, hall);
  }

  updateHall(id: number, hall: iHall): Observable<iHall> {
    return this.http.put<iHall>(`${this.apiHallUrl}/${id}`, hall);
  }

  deleteHall(id: number): Observable<iHall> {
    return this.http.delete<iHall>(`${this.apiHallUrl}/${id}`);
  }

  getAvailableSeats(hallId: number, movieId: number): Observable<iHallSeat[]> {
    return this.http.get<iHallSeat[]>(
      `${environment.apiUrl}/avaible-seats/${hallId}/${movieId}`
    );
  }
}
