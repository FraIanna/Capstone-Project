import { Component, TemplateRef, ViewChild } from '@angular/core';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../auth/auth.service';
import { iUser } from '../../models/i-user';
import { iReservation } from '../../models/i-reservation';
import { iOrder } from '../../models/i-order';
import { iOrderItem } from '../../models/i-order-item';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent {
  @ViewChild('fakeModal') fakeModal!: TemplateRef<any>;

  reservations: iReservation[] = [];
  user!: iUser;
  orders: iOrder[] = [];
  combinedData: any[] = [];

  constructor(
    private cartSvc: CartService,
    private authSvc: AuthService,
    private modalService: NgbModal
  ) {}
  ngOnInit() {
    this.authSvc.restoreUser();
    this.authSvc.user$.subscribe((user) => {
      if (user) {
        this.user = user;
        this.loadReservations(user.userId);
        this.loadOrders();
      } else console.error('User not found');
    });
  }

  loadReservations(userId: number): void {
    this.cartSvc.getAllReservation(userId).subscribe({
      next: (reservations: iReservation[]) => {
        this.reservations = reservations;
        this.combineData();
      },
      error: (error) => {
        if (error.status === 404) {
          this.reservations = [];
          this.combineData();
        } else {
          console.error('Errore nel recupero delle prenotazioni:', error);
        }
      },
    });
  }

  loadOrders(): void {
    this.cartSvc.getAllOrders().subscribe({
      next: (orders: iOrder[]) => {
        this.orders = orders;
        this.combineData();
      },
      error: (error) => {
        console.error('Errore nel recupero degli ordini:', error);
      },
    });
  }

  deleteReservation(id: number): void {
    this.cartSvc.deleteReservation(id).subscribe({
      next: () => {
        console.log('Prenotazione cancellata con successo');
        this.loadReservations(this.user.userId);
      },
      error: (error) => {
        console.error(
          'Errore durante la cancellazione della prenotazione:',
          error
        );
      },
    });
  }

  updateItemQuantity(orderItem: iOrderItem, event: Event) {
    const input = event.target as HTMLInputElement;
    const newQuantity = Number(input.value);
    orderItem.quantity = newQuantity;
    this.cartSvc.updateItem(orderItem).subscribe({
      next: () => {
        console.log('Quantità aggiornata con successo.');
        this.loadOrders();
      },
      error: (error) => {
        console.error("Errore durante l'aggiornamento dell'articolo:", error);
      },
    });
  }

  combineData() {
    if (this.reservations.length > 0 && this.orders.length > 0) {
      this.combinedData = this.reservations.map((reservation) => {
        const correspondingOrder = this.orders.find(
          (order) => order.reservationId === reservation.id
        );
        return {
          reservation,
          order: correspondingOrder || null,
        };
      });
      console.log(this.combinedData);
    }
  }

  getTotal(): number {
    let total = 0;
    this.combinedData.forEach((item) => {
      if (item.order) {
        total += item.order.items.reduce(
          (acc: any, i: any) => acc + i.product.price * i.quantity,
          0
        );
      }
    });

    return total;
  }

  clearCart(): void {
    this.orders = [];
    this.reservations = [];
  }

  fakePayment() {
    this.clearCart();
    this.openModal();
  }

  openModal() {
    this.modalService.open(this.fakeModal);
  }
}
