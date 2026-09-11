import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth-service';
@Component({
  selector: 'app-admin', imports: [RouterLink, RouterLinkActive, RouterOutlet],
  template: `<div class="min-h-screen bg-slate-50">
    <header class="border-b border-slate-200 bg-white">
      <div class="mx-auto flex max-w-7xl flex-wrap items-center justify-between gap-4 px-6 py-5">
        <a routerLink="/admin" class="text-xl font-black">BridgeStartup <span class="text-indigo-600">Admin</span></a>
        <div class="flex items-center gap-4 text-sm">
          <a routerLink="/" class="font-bold text-indigo-600">View site</a>
          <span>{{ auth.user()?.username }}</span>
          <button class="button secondary" (click)="auth.logout()">Log out</button>
        </div>
      </div>
    </header>
    <div class="mx-auto grid max-w-7xl gap-8 px-6 py-8 md:grid-cols-[180px_1fr]">
      <nav class="flex flex-wrap gap-3 md:flex-col" aria-label="Admin navigation">
        <a routerLink="/admin/users" routerLinkActive="bg-indigo-100 text-indigo-800" class="rounded-lg px-4 py-3 font-bold">Users</a>
        <a routerLink="/admin/posts" routerLinkActive="bg-indigo-100 text-indigo-800" class="rounded-lg px-4 py-3 font-bold">Posts</a>
        <a routerLink="/admin/contacts" routerLinkActive="bg-indigo-100 text-indigo-800" class="rounded-lg px-4 py-3 font-bold">Contacts</a>
      </nav>
      <main class="min-w-0"><router-outlet /></main>
    </div>
  </div>`
})

// Kako sam rekao da kada ruta pocinje sa /Admin da se koristi ovaj layout?
export class Admin { readonly auth = inject(AuthService); }
