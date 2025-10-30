import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  loginData = {
    username: '',
    password: ''
  };

  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onLogin() {
    // Basic validation
    if (!this.loginData.username || !this.loginData.password) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.loginData.username, this.loginData.password).subscribe({
      next: (response: any) => {
        // Save user to localStorage and update auth state
        this.authService.setCurrentUser(response.user);
        // Redirect to dashboard
        this.router.navigate(['/report-generator']);
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.message || 'Login failed. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onNavigateToRegister() {
    this.router.navigate(['/register']);
  }

  onNavigateToDashboard() {
    this.router.navigate(['/dashboard']);
  }
}
