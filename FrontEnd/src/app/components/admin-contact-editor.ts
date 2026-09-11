import { Component, input, output, viewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Contact, CreateContactInput } from '../interfaces/contact-interface';
import { AdminUser } from '../interfaces/user-interface';

@Component({
  selector: 'app-admin-contact-editor',
  imports: [FormsModule],
  template: `
    <section class="panel mt-6" aria-labelledby="contact-form-title">
      <h2 id="contact-form-title" class="text-xl font-black">{{ contact() ? 'Edit contact' : 'Create contact' }}</h2>
      @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
      <form #form="ngForm" (ngSubmit)="submit(form)" class="form-grid">
        <fieldset [disabled]="busy()" class="grid gap-4">
          <label>Sender
            <select name="userId" required [(ngModel)]="draft.userId" #sender="ngModel" aria-describedby="sender-error">
              <option [ngValue]="0" disabled>Select a user</option>
              @for (user of users(); track user.id) { <option [ngValue]="user.id">{{ user.username }} ({{ user.email }})</option> }
            </select>
            @if ((sender.touched || form.submitted) && !validSender()) {
              <span id="sender-error" class="text-sm font-normal text-red-700">Select an existing user.</span>
            }
          </label>
          <label>Subject
            <input name="subject" required [pattern]="nonBlankPattern" [(ngModel)]="draft.subject" #subject="ngModel"
              aria-describedby="subject-error" [attr.aria-invalid]="subject.invalid && (subject.touched || form.submitted)">
            @if (subject.invalid && (subject.touched || form.submitted)) {
              <span id="subject-error" class="text-sm font-normal text-red-700">Enter a subject.</span>
            }
          </label>
          <label>Message
            <textarea name="message" rows="6" required [pattern]="nonBlankPattern" [(ngModel)]="draft.message" #message="ngModel"
              aria-describedby="message-error" [attr.aria-invalid]="message.invalid && (message.touched || form.submitted)"></textarea>
            @if (message.invalid && (message.touched || form.submitted)) {
              <span id="message-error" class="text-sm font-normal text-red-700">Enter a message.</span>
            }
          </label>
        </fieldset>
        <div class="flex gap-3">
          <button type="submit" class="button" [disabled]="busy()">{{ busy() ? 'Saving...' : 'Save contact' }}</button>
          <button type="button" class="button secondary" [disabled]="busy()" (click)="cancelled.emit()">Cancel</button>
        </div>
      </form>
    </section>
  `,
})
export class AdminContactEditor {
  contact = input<Contact | null>(null);
  users = input.required<AdminUser[]>();
  busy = input(false);
  error = input('');
  saved = output<CreateContactInput>();
  cancelled = output<void>();
  private form = viewChild<NgForm>('form');
  readonly nonBlankPattern = /\S/;
  draft: CreateContactInput = { userId: 0, subject: '', message: '' };

  ngOnChanges(changes: Record<string, unknown>) {
    if (!('contact' in changes)) return;
    const contact = this.contact();
    this.draft = contact ? { userId: contact.user.id, subject: contact.subject, message: contact.message }
      : { userId: 0, subject: '', message: '' };
    this.form()?.resetForm(this.draft);
  }
  validSender() { return this.users().some(user => user.id === this.draft.userId); }
  submit(form: NgForm) {
    if (this.busy()) return;
    const subject = this.draft.subject.trim();
    const message = this.draft.message.trim();
    if (form.invalid || !this.validSender() || !subject || !message) {
      form.control.markAllAsTouched();
      return;
    }
    this.saved.emit({ userId: this.draft.userId, subject, message });
  }
}
