import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [RouterLink],
  template: `     <header class="border-b border-slate-200 bg-white/90">
        <nav class="mx-auto flex max-w-6xl flex-col gap-4 px-4 py-5 sm:flex-row sm:items-center sm:justify-between sm:px-6 lg:px-8">
            <a  routerLink="" class="flex items-center gap-3">
                <span class="grid h-11 w-11 place-items-center rounded-lg bg-indigo-600 font-black text-white">BS</span>
                <span>
                    <span class="block text-xl font-black tracking-tight">BridgeStartup</span>
                    <span class="block text-xs font-bold uppercase tracking-[0.18em] text-emerald-600">Founder to builder</span>
                </span>
            </a>
            <div class="flex flex-wrap items-center justify-center gap-2">
                <a routerLink="" class="rounded-lg bg-indigo-600 cursor-pointer px-4 py-2 text-sm font-bold text-white">Posts</a>
                <a routerLink="/about" class="rounded-lg px-4 py-2 cursor-pointer text-sm font-bold text-slate-700 hover:bg-slate-100">About us</a>
                <a routerLink="/contact" class="rounded-lg px-4 py-2 cursor-pointer text-sm font-bold text-slate-700 hover:bg-slate-100">Contact us</a>
            </div>
        </nav>
    </header>`, 
  styles: ``,
})
export class Header {}
