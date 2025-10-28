import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5138/api';

  constructor(private http: HttpClient) { }

  // Get all users
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/Users`);
  }

  // Create new user (register)
  createUser(user: User): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/Users`, user);
  }

  // Get user by ID
  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/Users/${id}`);
  }

  // Update user
  updateUser(id: number, user: User): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/Users/${id}`, user);
  }

  // Delete user
  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Users/${id}`);
  }

  // Custom login method - checks credentials against users list
  login(username: string, password: string): Observable<{user: User, message: string}> {
    return new Observable(observer => {
      this.getUsers().subscribe({
        next: (users) => {
          const user = users.find(u => u.userName === username && u.password === password);
          if (user) {
            observer.next({ user: user, message: 'Login successful' });
            observer.complete();
          } else {
            observer.error({ message: 'Invalid username or password' });
          }
        },
        error: (error) => {
          observer.error({ message: 'Login failed. Please try again.' });
        }
      });
    });
  }
}
