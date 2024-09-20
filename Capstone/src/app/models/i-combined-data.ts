import { iOrder } from './i-order';
import { iReservation } from './i-reservation';

export interface iCombinedData {
  order: iOrder[];
  reservation: iReservation[];
}
