import { iOrderItem } from './i-order-item';

export interface iOrder {
  id?: number;
  reservationId: number;
  UserId: number;
  items: iOrderItem[];
}
