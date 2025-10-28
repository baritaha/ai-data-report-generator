import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {Dashboard} from './components/dashboard/dashboard';
import {Login} from './components/login/login';
import { Register } from './components/register/register';
import { AuthGuard } from './guards/auth-guard';
export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard,canActivate: [AuthGuard] },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
