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
                <p class="mt-5 text-lg leading-8 text-slate-600">Send a note about your startup post, partnership idea, or feedback for the platform.</p>
                <div class="mt-8 space-y-4 text-slate-700">
                    <p class="font-bold text-slate-950">support@bridgestartup.dev</p>
                    <p class="font-bold text-slate-950">+381 60 111 2233</p>
                    <p class="leading-7">Belgrade, Serbia</p>
                </div>
            </div>

            <form class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm sm:p-8">
                <div class="grid gap-5 sm:grid-cols-2">
                    <label class="block">
                        <span class="text-sm font-black uppercase tracking-[0.16em] text-slate-500">Name</span>
                        <input type="text" placeholder="Your name" class="mt-2 w-full rounded-lg border border-slate-300 bg-white px-4 py-3 font-semibold text-slate-950 outline-none placeholder:text-slate-400 focus:border-indigo-600 focus:ring-4 focus:ring-indigo-100" />
                    </label>
                    <label class="block">
                        <span class="text-sm font-black uppercase tracking-[0.16em] text-slate-500">Email</span>
                        <input type="email" placeholder="you@example.com" class="mt-2 w-full rounded-lg border border-slate-300 bg-white px-4 py-3 font-semibold text-slate-950 outline-none placeholder:text-slate-400 focus:border-indigo-600 focus:ring-4 focus:ring-indigo-100" />
                    </label>
                    <label class="block sm:col-span-2">
                        <span class="text-sm font-black uppercase tracking-[0.16em] text-slate-500">Subject</span>
                        <input type="text" placeholder="What is this about?" class="mt-2 w-full rounded-lg border border-slate-300 bg-white px-4 py-3 font-semibold text-slate-950 outline-none placeholder:text-slate-400 focus:border-indigo-600 focus:ring-4 focus:ring-indigo-100" />
                    </label>
                    <label class="block sm:col-span-2">
                        <span class="text-sm font-black uppercase tracking-[0.16em] text-slate-500">Message</span>
                        <textarea rows="6" placeholder="Write your message..." class="mt-2 w-full resize-none rounded-lg border border-slate-300 bg-white px-4 py-3 font-semibold text-slate-950 outline-none placeholder:text-slate-400 focus:border-indigo-600 focus:ring-4 focus:ring-indigo-100"></textarea>
                    </label>
                </div>
                <div class="mt-6 flex flex-col gap-3 border-t border-slate-200 pt-6 sm:flex-row sm:items-center sm:justify-between">
                    <p class="text-sm font-semibold leading-6 text-slate-500">Static form design only.</p>
                    <button type="button" class="inline-flex justify-center rounded-lg bg-slate-950 px-5 py-3 text-sm font-black text-white shadow-sm">Send message</button>
                </div>
            </form>
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
