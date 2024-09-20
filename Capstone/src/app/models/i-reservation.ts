import { iHallSeat } from './i-hall-seat';
import { iMovie } from './i-movie';

export interface iReservation {
  id: number;
  userId: number;
  movieId: number;
  movie?: iMovie;
  hallId: number;
  hallSeatId: number;
  seat?: iHallSeat;
  createdAt: string;
  ticketPrice?: number;
}
