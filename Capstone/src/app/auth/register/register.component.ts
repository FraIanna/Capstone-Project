import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { iUser } from '../../models/i-user';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  MaxLengthValidator,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  newUser: Partial<iUser> = {};
  form!: FormGroup;
  showConfirmPassword: boolean = false;
  showPassword: boolean = false;

  constructor(
    protected authSvc: AuthService,
    private router: Router,
    private fb: FormBuilder
  ) {}
  register() {
    if (this.form.invalid) {
      return;
    }
    const user: iUser = this.form.value;
    this.authSvc.register(user).subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: (error) => {
        console.error('Registrazione Fallita', error);
      },
    });
  }

  ngOnInit() {
    this.form = this.fb.group(
      {
        name: this.fb.control(null, [
          Validators.required,
          Validators.maxLength(30),
        ]),
        email: this.fb.control(null, [Validators.required, Validators.email]),
        password: this.fb.control(null, [
          Validators.required,
          Validators.minLength(10),
          Validators.maxLength(50),
          Validators.pattern(
            '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&-])[A-Za-z\\d@$!%*?&-]{10,50}$'
          ),
        ]),
        confirmPassword: this.fb.control(null, [
          Validators.required,
          Validators.minLength(10),
          Validators.maxLength(50),
        ]),
      },
      { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const password = group.get('password');
    const confirmPassword = group.get('confirmPassword');
    return password?.value === confirmPassword?.value
      ? null
      : { passwordMatch: true };
  }

  isTouchedAndInvalid(fieldName: string) {
    const field = this.form.get(fieldName);
    return field?.invalid && field?.touched;
  }

  invalidPassword(fieldName: string) {
    const field = this.form.get(fieldName);
    return (
      field?.hasError('minlength') ||
      field?.hasError('maxlength') ||
      field?.hasError('pattern')
    );
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
}
