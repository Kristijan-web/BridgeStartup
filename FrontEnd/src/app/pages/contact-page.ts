import { Component, inject } from '@angular/core';
import { Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-contact-page',
  imports: [],
  template: `   <section class="mx-auto max-w-6xl px-4 py-12 sm:px-6 lg:px-8">
        <div class="grid gap-8 lg:grid-cols-[0.85fr_1.15fr] lg:items-start">
            <div>
                <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">Contact us</p>
                <h1 class="mt-4 text-4xl font-black tracking-tight text-slate-950 sm:text-5xl">Reach the BridgeStartup team.</h1>
                <p class="mt-5 text-lg leading-8 text-slate-600">Kontakt strana je staticka i prikazuje direktne podatke za komunikaciju. Bez forme, bez slanja, bez dinamike.</p>
            </div>

            <div class="grid gap-5 sm:grid-cols-2">
                <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
                    <p class="text-sm font-black uppercase tracking-[0.18em] text-slate-500">Email</p>
                    <p class="mt-3 text-xl font-black text-indigo-600">support@bridgestartup.dev</p>
                    <p class="mt-3 leading-7 text-slate-600">Za pitanja oko objava, partnerstva ili feedback za MVP.</p>
                </div>
                <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
                    <p class="text-sm font-black uppercase tracking-[0.18em] text-slate-500">Phone</p>
                    <p class="mt-3 text-xl font-black text-cyan-700">+381 60 111 2233</p>
                    <p class="mt-3 leading-7 text-slate-600">Radnim danima od 10:00 do 17:00.</p>
                </div>
                <div class="rounded-lg border border-slate-200 bg-slate-950 p-6 text-white sm:col-span-2">
                    <p class="text-sm font-black uppercase tracking-[0.18em] text-cyan-300">Location</p>
                    <p class="mt-3 text-xl font-black">Belgrade, Serbia</p>
                    <p class="mt-3 leading-7 text-slate-300">Remote-first projekat za foundere i male startup timove iz regiona.</p>
                </div>
            </div>
        </div>
    </section>`,
  styles: ``,
})
export class ContactPage {
    private meta = inject(Meta);

  constructor() {
    this.meta.updateTag({
      name: 'description',
      content: 'Contact us, reach us, get in touch'
    });
  }


}
