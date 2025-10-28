
import {  Routes } from '@angular/router';
import { Dashboard } from "./dashboard/dashboard";
import {Login} from './login/login';
import { Register } from "./register/register";

export const route :Routes=[
 { path: '', redirectTo: '/dashboard', pathMatch: 'full',component:Dashboard },
 {path:'login',component:Login},
 {path:'register',component:Register}
]
