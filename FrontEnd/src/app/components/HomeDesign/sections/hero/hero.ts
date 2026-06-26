import { Component } from '@angular/core';

@Component({
  selector: 'app-hero',
  imports: [],
  template: ` <section class="mx-auto max-w-6xl px-4 py-12 sm:px-6 lg:px-8">
            <div class="grid gap-8 lg:grid-cols-[0.95fr_1.05fr] lg:items-end">
                <div>
                    <p class="text-sm font-black uppercase tracking-[0.22em] text-cyan-700">Startup opportunities</p>
                    <h1 class="mt-4 max-w-2xl text-4xl font-black tracking-tight text-slate-950 sm:text-5xl">Bridge the gap between founders and builders.</h1>
                    <p class="mt-5 max-w-2xl text-lg leading-8 text-slate-600">BridgeStartup is a static board for startup posts. Founders can present an idea, stack, contact details, and project stage, while developers can quickly decide whether it fits them.</p>
                </div>
                <div class="grid gap-3 sm:grid-cols-3">
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-indigo-600">6</span>
                        <span class="text-sm font-semibold text-slate-500">open posts</span>
                    </div>
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-emerald-600">24</span>
                        <span class="text-sm font-semibold text-slate-500">stack badges</span>
                    </div>
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-amber-500">0</span>
                        <span class="text-sm font-semibold text-slate-500">dynamic parts</span>
                    </div>
                </div>
            </div>
        </section> `,
  styles: ``,
})
export class Hero {}
