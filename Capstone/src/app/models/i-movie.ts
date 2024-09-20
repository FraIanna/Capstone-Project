import { iHallSeat } from './i-hall-seat';
import { iShowtime } from './i-showtime';

export interface iMovie {
  id?: number;
  title: string;
  description: string;
  releaseDate: string;
  category: string;
  runtime: number;
  imagePath: string;
  trailerUrl: string;
  casts: string[];
  showtimes: iShowtime[];
  hallSeats?: iHallSeat[];
  hallId: number;
}
