import { Component, TemplateRef, ViewChild } from '@angular/core';
import { iProduct } from '../../../models/i-product';
import { ProductService } from '../../../services/product.service';
import { CartService } from '../../../services/cart.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../../auth/auth.service';
import { iReservation } from '../../../models/i-reservation';
import { iOrder } from '../../../models/i-order';
import { iUser } from '../../../models/i-user';
import { iOrderItem } from '../../../models/i-order-item';

@Component({
  selector: 'app-drink',
  templateUrl: './drink.component.html',
  styleUrl: './drink.component.scss',
})
export class DrinkComponent {
  @ViewChild('foodModal') foodModal!: TemplateRef<any>;

  drinkProducts: iProduct[] = [];
  reservations: iReservation[] = [];
  selectedProduct!: iProduct;
  orders: iOrder[] = [];
  combinedData: any[] = [];
  user!: iUser;
  show: boolean = false;

  constructor(
    private productSvc: ProductService,
    private cartSvc: CartService,
    private modalService: NgbModal,
    private authSvc: AuthService
  ) {}

  ngOnInit() {
    this.productSvc.getAllProduct().subscribe((products) => {
      this.drinkProducts = products.filter(
        (product) => product.category === 'Bevande'
      );
    });
    this.authSvc.restoreUser();
    this.authSvc.user$.subscribe((user) => {
      if (user) {
        this.user = user;
        this.loadReservations(user.userId);
        this.loadOrders();
        this.combineData();
      } else console.error('User not found');
    });
  }

  loadReservations(userId: number): void {
    this.cartSvc.getAllReservation(userId).subscribe({
      next: (reservations: iReservation[]) => {
        this.reservations = reservations;
      },
      error: (error) => {
        if (error.status === 404) {
          this.reservations = [];
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

  openModal(product: iProduct): void {
    this.selectedProduct = product;
    this.modalService.open(this.foodModal);
  }

  selectReservation(
    reservation: iReservation,
    product: iProduct,
    modal: any
  ): void {
    const correspondingOrder = this.orders.find(
      (order) => order.reservationId === reservation.id
    );
    const orderId = correspondingOrder ? correspondingOrder.id : null;

    if (orderId) {
      const orderItem: iOrderItem = {
        id: 0,
        orderId: orderId,
        productId: product.id,
        quantity: 1,
      };

      this.cartSvc.addOrderItem(orderItem).subscribe({
        next: () => {
          console.log("Prodotto aggiunto all'ordine con successo.");
          modal.close();
          this.show = true;
        },
        error: (error) => {
          console.error("Errore nell'aggiunta dell'ordine:", error);
        },
      });
    } else {
      console.error('Nessun ordine associato alla prenotazione.');
    }
  }
}
