import { iUser } from './../models/i-user';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { iAuthResponse } from '../models/i-auth-response';
import { iAuthData } from '../models/i-auth-data';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  jwtHelper: JwtHelperService = new JwtHelperService();

  authSubject = new BehaviorSubject<null | iUser>(null);
  user$ = this.authSubject.asObservable();

  usersSubject = new BehaviorSubject<iUser[]>([]);
  users$ = this.usersSubject.asObservable();

  syncIsLoggedIn: boolean = false;

  isLoggedIn$ = this.user$.pipe(
    map((user) => !!user),
    tap((isLoggedIn) => {
      console.log('isLoggedIn$', isLoggedIn);
      this.syncIsLoggedIn = isLoggedIn;
    })
  );

  loginUrl: string = `${environment.apiUrl}/Auth/Login`;
  registerUrl: string = `${environment.apiUrl}/Auth/Register`;
  getProfileUrl: string = `${environment.apiUrl}/Auth/Profile`;
  updateProfileUrl: string = `${this.getProfileUrl}/Update`;

  constructor(private http: HttpClient, private router: Router) {}

  register(newUser: Partial<iUser>): Observable<iAuthResponse> {
    return this.http.post<iAuthResponse>(this.registerUrl, newUser);
  }

  login(authData: iAuthData): Observable<iAuthResponse> {
    return this.http.post<iAuthResponse>(this.loginUrl, authData).pipe(
      tap((data) => {
        console.log('Login successful. User data:', data);

        const decodedToken = this.jwtHelper.decodeToken(data.token);
        const user: iUser = {
          userId: data.userId,
          email: data.email,
          roles: decodedToken['roles'] || [],
        };

        this.authSubject.next(user);
        localStorage.setItem('accessData', JSON.stringify(data));
        console.log(user);
        this.autoLogout();
      })
    );
  }

  logout(): void {
    this.authSubject.next(null);
    localStorage.removeItem('accessData');
    this.router.navigate(['/']);
  }
  autoLogout(): void {
    const accessData = this.getAccessData();
    if (!accessData) return;
    const expDate = this.jwtHelper.getTokenExpirationDate(
      accessData.token
    ) as Date;
    const expMs = expDate.getTime() - new Date().getTime();
    setTimeout(this.logout, expMs);
  }

  getAccessData(): iAuthResponse | null {
    const accessDataJson = localStorage.getItem('accessData');
    if (!accessDataJson) return null;
    const accessData: iAuthResponse = JSON.parse(accessDataJson);
    console.log('Access Data from localStorage:', accessData);
    return accessData;
  }

  restoreUser(): void {
    const accessData = this.getAccessData();
    if (!accessData) return;
    if (this.jwtHelper.isTokenExpired(accessData.token)) return;
    const user: iUser = {
      userId: accessData.userId,
      email: accessData.email,
    };
    this.authSubject.next(user);
    this.autoLogout();
  }

  getUserRoles(): string[] | null {
    const accessData = this.getAccessData();
    if (!accessData) return null;

    const decodedToken = this.jwtHelper.decodeToken(accessData.token);
    const roles: string[] = [];

    const role =
      decodedToken[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ];
    if (role) {
      roles.push(role);
    }

    return roles.length > 0 ? roles : null;
  }

  updateProfile(
    user: Partial<iUser>,
    currentPassword: string,
    newPassword: string
  ): Observable<iUser> {
    const updateProfile = {
      name: user.name,
      email: user.email,
      currentPassword: currentPassword,
      newPassword: newPassword,
    };
    return this.http.put<iUser>(this.updateProfileUrl, updateProfile);
  }

  getUser(): Observable<iUser> {
    return this.http.get<iUser>(this.getProfileUrl);
  }

  toggleVisibility(currentState: boolean): boolean {
    return !currentState;
  }
}
