import { Component } from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { iUser } from '../../models/i-user';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss',
})

//WORKING IN PROGRESS
export class AdminComponent {
  user!: iUser;
  constructor(private authSvc: AuthService) {}
}
