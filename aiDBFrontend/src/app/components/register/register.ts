import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user';
import { UserRole } from '../../models/user';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {
UserRole = UserRole;
  registerData: User & { confirmPassword: string } = {
    userName: '',
    email: '',
    role:UserRole.USER,
    password: '',
    confirmPassword: ''
  };

  errorMessage: string = '';
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onRegister() {
    // Validation
    if (!this.registerData.userName || !this.registerData.email || !this.registerData.password) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    if (this.registerData.password !== this.registerData.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    if (this.registerData.password.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters long';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    // Remove confirmPassword before sending to API
    const { confirmPassword, ...userToRegister } = this.registerData;

    this.authService.register(userToRegister).subscribe({
      next: (user) => {
        // Auto-login after successful registration
        this.authService.setCurrentUser(user);
        // Redirect to dashboard
        this.router.navigate(['/login']);
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Registration failed. Please try again.';
        this.isLoading = false;
      }
    });
  }

  onNavigateToLogin() {
    this.router.navigate(['/login']);
  }

  onNavigateToDashboard() {
    this.router.navigate(['/dashboard']);
  }
}
