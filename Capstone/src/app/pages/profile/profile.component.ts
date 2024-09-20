import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Component, TemplateRef, ViewChild } from '@angular/core';
import { iUser } from '../../models/i-user';
import { AuthService } from '../../auth/auth.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent {
  @ViewChild('passwordModal') passwordModal!: TemplateRef<any>;
  @ViewChild('profileModal') profileModal!: TemplateRef<any>;

  user!: iUser;
  showPassword: boolean = false;
  showConfirmPassword: boolean = false;
  passwordForm!: FormGroup;
  profileForm!: FormGroup;
  show: boolean = false;

  constructor(
    private authSvc: AuthService,
    private fb: FormBuilder,
    private modalService: NgbModal
  ) {}

  ngOnInit() {
    this.authSvc.getUser().subscribe({
      next: (data: iUser) => {
        this.user = data;
      },
      error: (error) => {
        console.error('Errore nel recuperare il profilo utente:', error);
      },
    });
    this.initForms();
  }
  initForms() {
    this.passwordForm = this.fb.group({
      currentPassword: [null, [Validators.required, Validators.minLength(10)]],
    });

    this.profileForm = this.fb.group({
      name: [this.user?.name, []],
      email: [this.user?.email, [Validators.email]],
      newPassword: [
        null,
        [
          Validators.minLength(10),
          Validators.maxLength(50),
          Validators.pattern(
            '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&-])[A-Za-z\\d@$!%*?&-]{10,50}$'
          ),
        ],
      ],
      confirmPassword: [
        null,
        [Validators.minLength(10), Validators.maxLength(50)],
      ],
    });
  }

  openPasswordModal() {
    this.modalService.open(this.passwordModal);
  }

  validateInitialPassword(modal: any) {
    if (this.passwordForm.valid) {
      modal.close();
      this.modalService.open(this.profileModal);
    }
  }

  togglePasswordVisibility(field: 'password' | 'confirmPassword') {
    if (field === 'password') {
      this.showPassword = this.authSvc.toggleVisibility(this.showPassword);
    } else {
      this.showConfirmPassword = this.authSvc.toggleVisibility(
        this.showConfirmPassword
      );
    }
  }

  updateProfile(modal: any) {
    if (this.profileForm.valid) {
      const updatedUser: iUser = {
        userId: this.user.userId,
        name: this.profileForm.value.name,
        email: this.profileForm.value.email,
        password: this.user.password,
      };
      this.authSvc
        .updateProfile(
          updatedUser,
          this.profileForm.value.currentPassword,
          this.profileForm.value.newPassword
        )
        .subscribe({
          next: () => {
            modal.close();
            this.show = true;
          },
          error: (error) => {
            console.error(
              "Errore durante l'aggiornamento del profilo utente:",
              error
            );
          },
        });
    }
  }
}
