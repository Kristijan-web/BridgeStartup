import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { SESSION_KEY, Session, tokenExpiry, validSession } from './session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private router = inject(Router);
  private session = signal<Session | null>(this.restore());
  private expiryTimer?: ReturnType<typeof setTimeout>;
  readonly user = computed(() => this.session()?.user ?? null);
  readonly isAdmin = computed(() => this.user()?.role.toLowerCase() === 'admin');
  constructor() {
    this.scheduleExpiry();
    window.addEventListener('storage', event => {
      if (event.key === SESSION_KEY || event.key === null) {
        this.session.set(this.restore());
        this.scheduleExpiry();
        if (!this.session() || (this.router.url.startsWith('/admin') && !this.isAdmin())) {
          void this.router.navigateByUrl('/');
        }
      }
    });
  }
  getToken(): string | null {
    const session = this.session();
    if (session && tokenExpiry(session.token) <= Date.now()) {
      this.logout(true);
      return null;
    }
    return session?.token ?? null;
  }
  establish(session: Session): void {
    if (!validSession(session)) throw new Error('The server returned an invalid login session.');
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    this.session.set(session);
    this.scheduleExpiry();
  }
  logout(expired = false): void {
    try { localStorage.removeItem(SESSION_KEY); } catch { /* Still clear the in-memory session. */ }
    this.session.set(null);
    clearTimeout(this.expiryTimer);
    void this.router.navigate(['/login'], { queryParams: expired ? { expired: '1' } : {} });
  }
  private restore(): Session | null {
    try {
      const value: unknown = JSON.parse(localStorage.getItem(SESSION_KEY) ?? 'null');
      if (validSession(value)) return value;
      localStorage.removeItem(SESSION_KEY);
    } catch {
      try { localStorage.removeItem(SESSION_KEY); } catch { /* Storage is unavailable. */ }
    }
    return null;
  }
  private scheduleExpiry(): void {
    clearTimeout(this.expiryTimer);
    const session = this.session();
    if (session) this.expiryTimer = setTimeout(() => {
      if (tokenExpiry(session.token) <= Date.now()) this.logout(true);
      else this.scheduleExpiry();
    }, Math.min(Math.max(tokenExpiry(session.token) - Date.now(), 0), 2147483647));
  }
}
