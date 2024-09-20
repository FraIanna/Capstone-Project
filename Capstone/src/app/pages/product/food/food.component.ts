import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { iOrderItem } from '../../../models/i-order-item';
import { CartService } from '../../../services/cart.service';
import { ProductService } from '../../../services/product.service';
import { iProduct } from './../../../models/i-product';
import { Component, TemplateRef, ViewChild } from '@angular/core';
import { AuthService } from '../../../auth/auth.service';
import { iReservation } from '../../../models/i-reservation';
import { iUser } from '../../../models/i-user';
import { iOrder } from '../../../models/i-order';

@Component({
  selector: 'app-food',
  templateUrl: './food.component.html',
  styleUrl: './food.component.scss',
})
export class FoodComponent {
  @ViewChild('foodModal') foodModal!: TemplateRef<any>;

  reservations: iReservation[] = [];
  foodProducts: iProduct[] = [];
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
      this.foodProducts = products.filter(
        (product) => product.category === 'Cibo'
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
    if (!this.user) {
      console.error(
        "Utente non loggato. Impossibile aggiungere il prodotto all'ordine."
      );
      alert('Devi effettuare il login per aggiungere prodotti al carrello.');
      return;
    }
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
