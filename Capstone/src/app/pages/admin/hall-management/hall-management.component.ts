import { Component, TemplateRef, ViewChild } from '@angular/core';
import { HallService } from '../../../services/hall.service';
import { iHall } from '../../../models/i-hall';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-hall-management',
  templateUrl: './hall-management.component.html',
  styleUrl: './hall-management.component.scss',
})

//WORKING IN PROGRESS
export class HallManagementComponent {
  @ViewChild('hallModal') hallModal!: TemplateRef<any>;

  hallForm!: FormGroup;
  create!: FormGroup;
  halls: iHall[] = [];
  selectedHall!: iHall | null;

  constructor(
    private hallSvc: HallService,
    private modalService: NgbModal,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.hallSvc.getAllHalls().subscribe({
      next: (halls: iHall[]) => {
        this.halls = halls;
      },
      error: (error: any) => {
        console.error('Error retrieving halls:', error);
      },
    });
    this.create = this.fb.group({
      name: this.fb.control(null, [Validators.required]),
      description: this.fb.control(null, [Validators.required]),
      capacity: this.fb.control(null, [Validators.required, Validators.min(1)]),
    });
  }

  deleteHall(hall: iHall): void {
    if (hall && hall.id != null) {
      this.hallSvc.deleteHall(hall.id).subscribe({
        next: () => {
          this.halls = this.halls.filter((h) => h.id !== hall.id);
          console.log(`Sala ${hall.name} eliminata con successo.`);
        },
        error: (error: any) => {
          console.error("Errore durante l'eliminazione della sala:", error);
        },
      });
    } else {
      console.warn('Sala non valida o ID mancante.');
    }
  }

  createHall(): void {
    if (this.create.valid) {
      const newHall: Omit<iHall, 'id'> = {
        name: this.create.value.name,
        description: this.create.value.description,
        capacity: this.create.value.capacity,
      };
      this.hallSvc.createHall(newHall).subscribe({
        next: () => {
          console.log('Sala creata con successo');
          this.hallSvc.getAllHalls().subscribe({
            next: (halls: iHall[]) => {
              this.halls = halls;
            },
          });
        },
        error: (error: any) => {
          console.error('Error creando la sala:', error);
        },
      });
    } else {
      return;
    }
  }

  isTouchedAndInvalid(fieldName: string) {
    const field = this.create.get(fieldName) || this.hallForm.get(fieldName);
    return field?.invalid && field?.touched;
  }

  openModal(hall?: iHall): void {
    this.selectedHall = hall || null;
    this.initForm();
    this.modalService.open(this.hallModal);
    console.log(this.selectedHall);
  }

  initForm(): void {
    this.hallForm = this.fb.group({
      id: [this.selectedHall?.id || null],
      name: [this.selectedHall?.name || null, [Validators.minLength(3)]],
      description: [this.selectedHall?.description || null],
      capacity: [this.selectedHall?.capacity || null, [Validators.min(1)]],
    });
  }

  submitModal(modal: any): void {
    if (this.hallForm.invalid) {
      return;
    } else {
      const updatedHall: iHall = {
        id: this.hallForm.value.id,
        name: this.hallForm.value.name,
        description: this.hallForm.value.description,
        capacity: this.hallForm.value.capacity,
      };
      if (updatedHall.id != null)
        this.hallSvc.updateHall(updatedHall.id, updatedHall).subscribe({
          next: () => {
            console.log('Sala aggiornata con successo');
            this.hallSvc.getAllHalls().subscribe({
              next: (halls: iHall[]) => {
                this.halls = halls;
              },
              error: (error: any) => {
                console.error('Error retrieving halls:', error);
              },
            });
            this.modalService.dismissAll();
          },
          error: (error) => {
            console.error("Errore durante l'aggiornamento della sala:", error);
            console.log(updatedHall);
          },
        });
    }
  }
}
