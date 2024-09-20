import { Component, ViewChild } from '@angular/core';
import {
  NgbCarousel,
  NgbSlideEvent,
  NgbSlideEventSource,
} from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../auth/auth.service';
import { MovieService } from '../../services/movie.service';
import { iMovie } from '../../models/i-movie';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  @ViewChild('carousel', { static: true }) carousel!: NgbCarousel;
  isLoggedIn: boolean = false;
  movies: iMovie[] = [];

  images = [
    'assets/Img/filmImg1.jpg',
    'assets/Img/filmImg2.jpg',
    'assets/Img/FilmImg3.jpg',
  ];

  paused = false;
  unpauseOnArrow = false;
  pauseOnIndicator = false;
  pauseOnHover = true;
  pauseOnFocus = true;

  constructor(
    private authSvc: AuthService,
    private movieSvc: MovieService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authSvc.isLoggedIn$.subscribe(
      (isLoggedIn) => (this.isLoggedIn = isLoggedIn)
    );
    this.authSvc.restoreUser();
    this.authSvc.getUserRoles();

    this.movieSvc.getAllMovies().subscribe({
      next: (movies: iMovie[]) => {
        this.movies = this.getRandomMovies(movies, 10);
      },
      error: (error: any) => {
        console.error('Error retrieving movies:', error);
      },
    });
  }

  getRandomMovies(movies: iMovie[], count: number): iMovie[] {
    const shuffledMovies = [...movies];
    for (let i = shuffledMovies.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [shuffledMovies[i], shuffledMovies[j]] = [
        shuffledMovies[j],
        shuffledMovies[i],
      ];
    }
    return shuffledMovies.slice(0, count);
  }

  getById(id: number) {
    this.movieSvc.getById(id).subscribe({
      next: (movie: iMovie) => {
        this.router.navigate(['movie', movie.id, 'details']);
        console.log(movie);
      },
      error: (error) => {
        console.error('Error retrieving movie:', error);
      },
    });
  }

  togglePaused() {
    if (this.paused) {
      this.carousel.cycle();
    } else {
      this.carousel.pause();
    }
    this.paused = !this.paused;
  }

  onSlide(slideEvent: NgbSlideEvent) {
    if (
      this.unpauseOnArrow &&
      slideEvent.paused &&
      (slideEvent.source === NgbSlideEventSource.ARROW_LEFT ||
        slideEvent.source === NgbSlideEventSource.ARROW_RIGHT)
    ) {
      this.togglePaused();
    }
    if (
      this.pauseOnIndicator &&
      !slideEvent.paused &&
      slideEvent.source === NgbSlideEventSource.INDICATOR
    ) {
      this.togglePaused();
    }
  }
}
