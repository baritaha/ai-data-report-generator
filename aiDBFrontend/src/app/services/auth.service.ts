import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { ApiService } from './api.service';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private apiService: ApiService,
    private router: Router
  ) {
    // Check if user data exists in localStorage when service is initialized
    const savedUser = localStorage.getItem('currentUser');
    if (savedUser) {
      this.currentUserSubject.next(JSON.parse(savedUser));
    }
  }

  // Register new user
  register(user: User) {
    return this.apiService.createUser(user);
  }

  // Login user
  login(username: string, password: string) {
    return this.apiService.login(username, password);
  }

  // Logout user
  logout() {
    // Remove user from localStorage
    localStorage.removeItem('currentUser');
    // Clear current user
    this.currentUserSubject.next(null);
    // Redirect to login page
    this.router.navigate(['/login']);
  }

  // Save user to localStorage and update current user
  setCurrentUser(user: User) {
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  // Get current user
  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  // Check if user is logged in
  isLoggedIn(): boolean {
    return this.getCurrentUser() !== null;
  }
}
