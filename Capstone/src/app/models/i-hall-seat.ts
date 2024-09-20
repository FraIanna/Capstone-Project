import { iReservation } from './i-reservation';

export interface iHallSeat {
  id: number;
  hallId: number;
  seatNumber: number;
  isReserved: boolean;
  reservations: iReservation[];
}
