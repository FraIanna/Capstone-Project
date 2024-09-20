import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  GuardResult,
  MaybeAsync,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { AuthService } from './auth.service';
import { iUser } from '../models/i-user';

@Injectable({
  providedIn: 'root',
})
export class AdminGuard {
  constructor(private authSvc: AuthService, private router: Router) {}
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): MaybeAsync<GuardResult> {
    const roles = this.authSvc.getUserRoles();
    const isLoggedIn = this.authSvc.syncIsLoggedIn;
    if (isLoggedIn && roles && roles.includes('Admin')) {
      return true;
    } else {
      this.router.navigate(['/']);
      return false;
    }
  }

  canActivateChild(
    childRoute: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): MaybeAsync<GuardResult> {
    return this.canActivate(childRoute, state);
  }
}
