import { Component } from '@angular/core';
import { ContactForm } from '../contact-form';

@Component({
  selector: 'app-contact-design',
  imports: [ContactForm],
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

            <app-contact-form></app-contact-form>
        </div>
    </section>`,
  styles: ``,
})
export class ContactDesign {}
