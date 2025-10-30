export interface User {
  id?: number;
  userName: string;
  email: string;
  password: string;
  role: UserRole;
}
//enum role user and admin
export enum UserRole {
  USER = 'user',
  ADMIN = 'admin'
}
