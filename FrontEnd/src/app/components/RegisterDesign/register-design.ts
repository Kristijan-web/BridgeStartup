import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService, errorMessage } from '../../services/api-service';
@Component({
  selector: 'app-register-design', imports: [FormsModule, RouterLink],
  template: `<section class="auth-card">
    <p class="eyebrow">Join the community</p><h1>Create your account</h1>
    @if (success()) {
      <p class="success" role="status">Account created. Check your email for the activation link before signing in.</p>
      <a routerLink="/login" class="button">Go to sign in</a>
    } @else {
      @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
      <form #form="ngForm" (ngSubmit)="submit()" class="form-grid">
        <label>Username<input name="username" required minlength="3" pattern="[^0-9].{2,}" [(ngModel)]="username" autocomplete="username"></label>
        <label>Email<input name="email" type="email" email required [(ngModel)]="email" autocomplete="email"></label>
        <label>Password<input name="password" type="password" required minlength="8" pattern="(?=.*[A-Z])(?=.*[0-9]).{8,}" [(ngModel)]="password" autocomplete="new-password"></label>
        <p class="help">Use at least 8 characters, including an uppercase letter and a number.</p>
        <button class="button" [disabled]="form.invalid || busy()">{{ busy() ? 'Creating account…' : 'Create account' }}</button>
      </form>
      <p class="mt-5">Already registered? <a routerLink="/login" class="text-indigo-700 underline">Sign in</a></p>
    }
  </section>`
})
export class RegisterDesign {
  private api = inject(ApiService);
  username = ''; email = ''; password = '';
  busy = signal(false); error = signal(''); success = signal(false);
  async submit() {
    if (this.busy()) return;
    this.busy.set(true); this.error.set('');
    try {
      await this.api.request('/Auth/register', { method: 'POST', body: JSON.stringify({
        username: this.username.trim(), email: this.email.trim(), password: this.password
      }) }, false);
      this.password = ''; this.success.set(true);
    } catch (error) { this.error.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
}
