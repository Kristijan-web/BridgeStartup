import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../services/auth-service';
@Component({
  selector: 'app-header', imports: [RouterLink, RouterLinkActive],
  template: `<header class="border-b border-slate-200 bg-white">
    <nav class="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-4 px-6 py-5" aria-label="Main navigation">
      <a routerLink="/" class="flex items-center gap-3">
        <span class="grid h-11 w-11 place-items-center rounded-lg bg-indigo-600 font-black text-white">BS</span>
        <span><span class="block text-xl font-black">BridgeStartup</span><span class="eyebrow text-xs">Founder to builder</span></span>
      </a>
      <div class="flex flex-wrap items-center gap-3 text-sm font-bold">
        <a routerLink="/" routerLinkActive="text-indigo-600" [routerLinkActiveOptions]="{ exact: true }">Posts</a>
        <a routerLink="/about" routerLinkActive="text-indigo-600">About us</a>
        <a routerLink="/contact" routerLinkActive="text-indigo-600">Contact us</a>
        @if (auth.user(); as user) {
          @if (auth.isAdmin()) { <a routerLink="/admin" class="button secondary">Admin panel</a> }
          <span class="text-slate-500">{{ user.username }}</span>
          <button type="button" class="button secondary" (click)="auth.logout()">Log out</button>
        } @else {
          <a routerLink="/login" class="button">Sign in</a>
        }
      </div>
    </nav>
  </header>`
})
export class Header { readonly auth = inject(AuthService); }
