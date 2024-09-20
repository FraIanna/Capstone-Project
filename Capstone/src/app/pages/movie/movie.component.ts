import { Component } from '@angular/core';
import { MovieService } from '../../services/movie.service';
import { iMovie } from '../../models/i-movie';
import { Router } from '@angular/router';

@Component({
  selector: 'app-movie',
  templateUrl: './movie.component.html',
  styleUrl: './movie.component.scss',
})
export class MovieComponent {
  movies: iMovie[] = [];

  constructor(private movieSvc: MovieService, private router: Router) {}

  ngOnInit() {
    this.movieSvc.getAllMovies().subscribe({
      next: (movie: iMovie[]) => {
        this.movies = movie;
      },
      error: (error) => {
        console.error('Errore nel recupero dei film:', error);
      },
    });
  }

  getById(id: number) {
    this.movieSvc.getById(id).subscribe({
      next: (movie: iMovie) => {
        this.router.navigate(['movie', movie.id, 'details']);
        console.log(movie);
      },
      error: (error) => {
        console.error('Errore nel recupero del film:', error);
      },
    });
  }
}
