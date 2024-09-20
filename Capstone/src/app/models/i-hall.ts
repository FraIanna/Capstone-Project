import { iMovie } from './i-movie';

export interface iHall {
  id?: number;
  name: string;
  description: string;
  capacity: number;
  movies?: iMovie[];
  seats?: any[];
}
