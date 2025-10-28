import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ApiService } from '../../services/api.service';
import { User } from '../../models/user';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  currentUser: User | null = null;
  allUsers: User[] = [];
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private apiService: ApiService,
    private router: Router
  ) {}

  ngOnInit() {
    // Get current logged in user
    this.currentUser = this.authService.getCurrentUser();

    // Load all users from API
    this.loadAllUsers();
  }

  loadAllUsers() {
    this.isLoading = true;
    this.apiService.getUsers().subscribe({
      next: (users) => {
        this.allUsers = users;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.isLoading = false;
      }
    });
  }

  onLogout() {
    this.authService.logout();
  }

  onNavigateToLogin() {
    this.router.navigate(['/login']);
  }

  onNavigateToRegister() {
    this.router.navigate(['/register']);
  }
}
