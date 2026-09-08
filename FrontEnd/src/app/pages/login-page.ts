import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService, errorMessage } from '../services/api-service';
import { AuthService } from '../services/auth-service';
import { safeReturnUrl, Session } from '../services/session';

@Component({
  selector: 'app-login-page', imports: [FormsModule, RouterLink],
  template: `
    <section class="auth-card">
      <p class="eyebrow">Welcome back</p><h1>Sign in to BridgeStartup</h1>
      @if (expired) { <p class="notice">Your session expired. Please sign in again.</p> }
      @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
      <form #form="ngForm" (ngSubmit)="submit()" class="form-grid">
        <label>Email<input name="email" type="email" email required [(ngModel)]="email" autocomplete="username"></label>
        <label>Password<input name="password" type="password" required [(ngModel)]="password" autocomplete="current-password"></label>
        <button class="button" [disabled]="form.invalid || busy()">{{ busy() ? 'Signing in…' : 'Sign in' }}</button>
      </form>
      <p class="mt-5">New here? <a class="text-indigo-700 underline" routerLink="/register">Create an account</a></p>
    </section>`
})
export class LoginPage {
  private api = inject(ApiService);
  private auth = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  expired = this.route.snapshot.queryParamMap.has('expired');
  email = ''; password = '';
  busy = signal(false); error = signal('');
  async submit() {
    if (this.busy()) return;
    this.busy.set(true); this.error.set('');
    try {
      const session = await this.api.request<Session>('/Auth/login', {
        method: 'POST', body: JSON.stringify({ email: this.email.trim(), password: this.password })
      }, false);
      this.auth.establish(session);
      this.password = '';
      const target = this.route.snapshot.queryParamMap.get('returnUrl');
      await this.router.navigateByUrl(target ? safeReturnUrl(target) : this.auth.isAdmin() ? '/admin/users' : '/');
    } catch (error) { this.error.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
}
