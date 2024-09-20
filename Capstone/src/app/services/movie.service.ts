import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { iMovie } from '../models/i-movie';

@Injectable({
  providedIn: 'root',
})
export class MovieService {
  apiMovieUrl = 'https://localhost:7297/Movie';

  constructor(private http: HttpClient) {}

  getAllMovies(): Observable<iMovie[]> {
    return this.http.get<iMovie[]>(this.apiMovieUrl);
  }

  getById(id: number): Observable<iMovie> {
    return this.http.get<iMovie>(`${this.apiMovieUrl}/${id}`);
  }

  create(movieData: FormData): Observable<iMovie> {
    return this.http.post<iMovie>(this.apiMovieUrl, movieData);
  }

  update(id: number, movieData: FormData): Observable<iMovie> {
    return this.http.put<iMovie>(`${this.apiMovieUrl}/${id}`, movieData);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiMovieUrl}/${id}`);
  }
}
