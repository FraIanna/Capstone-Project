import { iOrderItem } from './../../../models/i-order-item';
import { Component } from '@angular/core';
import { MovieService } from '../../../services/movie.service';
import { iMovie } from '../../../models/i-movie';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer } from '@angular/platform-browser';
import { CartService } from '../../../services/cart.service';
import { iReservation } from '../../../models/i-reservation';
import { AuthService } from '../../../auth/auth.service';
import { iUser } from '../../../models/i-user';
import { HallService } from '../../../services/hall.service';
import { iHall } from '../../../models/i-hall';
import { iOrder } from '../../../models/i-order';

@Component({
  selector: 'app-movie-details',
  templateUrl: './movie-details.component.html',
  styleUrl: './movie-details.component.scss',
})
export class MovieDetailsComponent {
  user!: iUser;
  hall!: iHall;
  movie!: iMovie;
  reservation!: iReservation;
  show: boolean = false;

  constructor(
    private movieSvc: MovieService,
    private hallSvc: HallService,
    private authSvc: AuthService,
    private cartSvc: CartService,
    private router: ActivatedRoute,
    private Router: Router,
    protected sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    const id = Number(this.router.snapshot.paramMap.get('id')) || null;
    if (!id) {
      console.error('Invalid Id');
      return;
    }

    this.authSvc.restoreUser();
    this.authSvc.user$.subscribe((user) => {
      if (user) this.user = user;
      else console.error('User not found');
    });

    this.movieSvc.getById(id).subscribe({
      next: (movie: iMovie) => {
        this.movie = movie;
        this.hallSvc.getHallById(this.movie.hallId).subscribe({
          next: (hall: iHall) => {
            console.log(hall);
            this.hall = hall;
          },
          error: (error) => {
            console.error('Error retrieving hall:', error);
          },
        });
      },
      error: (error) => {
        console.error('Error retrieving movie:', error);
      },
    });
  }

  createReservation(): void {
    if (this.movie.id && this.movie) {
      const movieId = this.movie.id;
      const reservation: iReservation = {
        id: 0,
        userId: this.user.userId,
        movieId: movieId,
        hallId: this.movie.hallId,
        hallSeatId: 0,
        createdAt: new Date().toISOString(),
      };
      console.log(reservation);
      this.cartSvc
        .createReservation(reservation, reservation.movieId)
        .subscribe({
          next: (response: iReservation) => {
            this.reservation = response;
            this.createOrder(response.id);
            this.show = true;
            const timeout = setTimeout(() => {
              this.Router.navigate(['/cart']);
            }, 1000);
          },
          error: (error) => {
            console.error('Error creating reservation:', error);
          },
        });
    } else return;
  }

  createOrder(reservationId: number) {
    const order: iOrder = {
      UserId: this.user.userId,
      reservationId: reservationId,
      items: [],
    };

    this.cartSvc.createOrder(order).subscribe({
      next: (response: iOrder) => {
        console.log(response);
      },
      error: (error) => {
        console.error('Errore while creating the order:', error);
      },
    });
  }
}
