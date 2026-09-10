import { Component, inject, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../services/auth-service';
import { ContactsService } from '../services/contacts-service';
import { errorMessage } from '../services/api-service';

@Component({
  selector: 'app-contact-form',
  imports: [FormsModule, RouterLink],
  template: `
    <section class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm sm:p-8" aria-label="Send a message">
      @if (auth.user(); as user) {
        <form #form="ngForm" (ngSubmit)="submit(form)" [attr.aria-busy]="busy()">
          @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
          @if (success()) { <p class="success" role="status">Your message has been sent. Thank you for getting in touch.</p> }
          <fieldset [disabled]="busy()" class="grid gap-5 sm:grid-cols-2">
            <label>Name<input name="senderName" [value]="user.username" readonly autocomplete="name"></label>
            <label>Email<input name="senderEmail" type="email" [value]="user.email" readonly autocomplete="email"></label>
            <label class="sm:col-span-2">Subject
              <input name="subject" required [pattern]="nonBlankPattern" [(ngModel)]="subject" #subjectControl="ngModel"
                placeholder="What is this about?" aria-describedby="contact-subject-error"
                [attr.aria-invalid]="subjectControl.invalid && (subjectControl.touched || form.submitted)">
              @if (subjectControl.invalid && (subjectControl.touched || form.submitted)) {
                <span id="contact-subject-error" class="text-sm font-normal text-red-700">Enter a subject.</span>
              }
            </label>
            <label class="sm:col-span-2">Message
              <textarea name="message" rows="6" required [pattern]="nonBlankPattern" [(ngModel)]="message" #messageControl="ngModel"
                placeholder="Write your message..." aria-describedby="contact-message-error"
                [attr.aria-invalid]="messageControl.invalid && (messageControl.touched || form.submitted)"></textarea>
              @if (messageControl.invalid && (messageControl.touched || form.submitted)) {
                <span id="contact-message-error" class="text-sm font-normal text-red-700">Enter a message.</span>
              }
            </label>
          </fieldset>
          <div class="mt-6 flex flex-col gap-3 border-t border-slate-200 pt-6 sm:flex-row sm:items-center sm:justify-between">
            <p class="help">Your message will be sent from your account.</p>
            <button type="submit" class="button" [disabled]="busy()">{{ busy() ? 'Sending...' : 'Send message' }}</button>
          </div>
        </form>
      } @else {
        <h2 class="text-xl font-black">Send us a message</h2>
        <p class="my-5 text-slate-600">Sign in to contact the BridgeStartup team using your account.</p>
        <a routerLink="/login" [queryParams]="{ returnUrl: '/contact' }" class="button">Sign in to contact us</a>
      }
    </section>
  `,
})
export class ContactForm {
  readonly auth = inject(AuthService);
  private contacts = inject(ContactsService);
  readonly nonBlankPattern = /\S/;
  subject = '';
  message = '';
  busy = signal(false);
  error = signal('');
  success = signal(false);

  async submit(form: NgForm) {
    if (this.busy()) return;
    this.error.set('');
    this.success.set(false);
    const token = this.auth.getToken();
    const user = this.auth.user();
    if (!token || !user) {
      this.error.set('Please sign in to send a message.');
      return;
    }
    const subject = this.subject.trim();
    const message = this.message.trim();
    if (form.invalid || !subject || !message) {
      form.control.markAllAsTouched();
      return;
    }
    this.busy.set(true);
    try {
      await this.contacts.createContact({ userId: user.id, subject, message });
      if (this.auth.getToken() !== token) return;
      this.subject = '';
      this.message = '';
      form.resetForm({ subject: '', message: '' });
      this.success.set(true);
    } catch (error) {
      if (this.auth.getToken() === token) this.error.set(errorMessage(error));
    } finally {
      this.busy.set(false);
    }
  }
}
