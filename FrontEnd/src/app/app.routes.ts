import { Routes } from '@angular/router';
import { HomePage } from './pages/home-page';
import { UserLayout } from './layouts/user/user-layout';
import { AboutUsPage } from './pages/about-us-page';
import { ContactPage } from './pages/contact-page';
import { PostDetailsPage } from './pages/post-details-page';
import { adminGuard } from './guards/auth-guards';

export const routes: Routes = [
  {
    path: 'admin', canActivate: [adminGuard], canActivateChild: [adminGuard],
    loadComponent: () => import('./layouts/admin/admin').then(m => m.Admin),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'users' },
      { path: 'users', title: 'Manage users | BridgeStartup', loadComponent: () => import('./pages/admin-users-page').then(m => m.AdminUsersPage) },
      { path: 'posts', title: 'Manage posts | BridgeStartup', loadComponent: () => import('./pages/admin-posts-page').then(m => m.AdminPostsPage) }
    ]
  },
  {
    path: '', component: UserLayout, children: [
      { path: '', component: HomePage, title: 'BridgeStartup' },
      { path: 'about', component: AboutUsPage, title: 'About us' },
      { path: 'contact', component: ContactPage, title: 'Contact us' },
      { path: 'posts/:id', component: PostDetailsPage, title: 'Post details' },
      { path: 'login', title: 'Sign in | BridgeStartup', loadComponent: () => import('./pages/login-page').then(m => m.LoginPage) },
      { path: 'register', title: 'Register | BridgeStartup', loadComponent: () => import('./pages/register-page').then(m => m.RegisterPage) },
      { path: 'forbidden', data: { forbidden: true }, title: 'Access denied', loadComponent: () => import('./pages/status-page').then(m => m.StatusPage) },
      { path: '**', title: 'Page not found', loadComponent: () => import('./pages/status-page').then(m => m.StatusPage) }
    ]
  }
];
