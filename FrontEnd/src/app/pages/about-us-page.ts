import { Component, inject } from '@angular/core';
import { Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-about-us-page',
  imports: [],
  template: `     <section class="mx-auto max-w-6xl px-4 py-12 sm:px-6 lg:px-8">
        <div class="grid gap-8 lg:grid-cols-[1fr_1fr] lg:items-center">
            <div>
                <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">About us</p>
                <h1 class="mt-4 text-4xl font-black tracking-tight text-slate-950 sm:text-5xl">A clear bridge between startup ideas and technical talent.</h1>
                <p class="mt-5 text-lg leading-8 text-slate-600">BridgeStartup is a static MVP for startup project posts. The goal is for founders to quickly present an idea, contact details, and the technologies needed for the first version of the product.</p>
            </div>
      
        </div>

 
    </section> `,
  styles: ``,
})
export class AboutUsPage {
  private meta = inject(Meta);

  constructor() {
    this.meta.updateTag({
      name: 'description',
      content: 'About us, learn more, who are we'
    });
  }
}
