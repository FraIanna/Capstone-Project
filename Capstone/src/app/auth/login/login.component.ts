import { Component } from '@angular/core';
import { iAuthData } from '../../models/i-auth-data';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  authData!: iAuthData;
  form!: FormGroup;
  showPassword: boolean = false;

  constructor(
    protected authSvc: AuthService,
    private router: Router,
    private fb: FormBuilder
  ) {}

  login() {
    if (this.form.valid) {
      const authData: iAuthData = {
        email: this.form.value.email,
        password: this.form.value.password,
      };

      this.authSvc.login(authData).subscribe({
        next: () => {
          this.router.navigate(['/']);
          const roles = this.authSvc.getUserRoles();
        },
        error: (error) => {
          console.error('Login failed', error);
        },
      });
    } else {
      console.log('Form invalid');
    }
  }

  ngOnInit() {
    this.form = this.fb.group({
      email: this.fb.control(null, [Validators.required, Validators.email]),
      password: this.fb.control(null, [Validators.required]),
    });
    this.authSvc.restoreUser();
  }

  isTouchedAndInvalid(fieldName: string) {
    const field = this.form.get(fieldName);
    return field?.invalid && field?.touched;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
