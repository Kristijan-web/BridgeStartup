import { Component, inject, signal } from '@angular/core';
import { AdminContactEditor } from '../admin-contact-editor';
import { AdminContactsList } from '../admin-contacts-list';
import { Contact, CreateContactInput } from '../../interfaces/contact-interface';
import { AdminUser } from '../../interfaces/user-interface';
import { ContactsService } from '../../services/contacts-service';
import { AdminService } from '../../services/user-service';
import { errorMessage } from '../../services/api-service';

@Component({
  selector: 'app-admin-contacts-design',
  imports: [AdminContactEditor, AdminContactsList],
  template: `
    <div class="flex flex-wrap items-center justify-between gap-4">
      <div><p class="eyebrow">Administration</p><h1>Contacts</h1><p class="mt-2 text-slate-500">Read and manage messages sent to the team.</p></div>
      <button class="button" (click)="create()" [disabled]="busy() || loading() || usersLoading() || !users().length">Add contact</button>
    </div>
    @if (success()) { <p class="success" role="status">{{ success() }}</p> }
    @if (usersError()) { <p class="error" role="alert">{{ usersError() }} <button class="underline" (click)="loadUsers()" [disabled]="busy() || usersLoading()">Retry users</button></p> }
    @if (!usersLoading() && !usersError() && !users().length) { <p class="notice">Create a user before adding contact messages.</p> }
    @if (editing()) {
      <app-admin-contact-editor [contact]="selected()" [users]="users()" [busy]="busy() || loading()" [error]="actionError()"
        (saved)="save($event)" (cancelled)="editing.set(false)" />
    } @else if (deleting(); as contact) {
      <section class="panel mt-6 border-red-200" role="alert" aria-labelledby="delete-contact-title">
        <h2 id="delete-contact-title" class="break-words text-xl font-black">Delete {{ contact.subject }}?</h2>
        <p class="my-4">This permanently removes the contact message.</p>
        @if (actionError()) { <p class="error">{{ actionError() }}</p> }
        <div class="flex gap-3">
          <button class="button danger" (click)="confirmDelete()" [disabled]="busy() || loading()">{{ busy() ? 'Deleting...' : 'Delete contact' }}</button>
          <button class="button secondary" (click)="deleting.set(null)" [disabled]="busy()">Cancel</button>
        </div>
      </section>
    } @else {
      @if (actionError()) { <p class="error" role="alert">{{ actionError() }}</p> }
      @if (selected(); as contact) {
        <section class="panel mt-6" aria-labelledby="contact-detail-title">
          <div class="flex items-start justify-between gap-3"><h2 id="contact-detail-title" class="min-w-0 break-words text-xl font-black">{{ contact.subject }}</h2>
            <button class="button secondary" (click)="selected.set(null)" [disabled]="busy()">Close</button></div>
          <p class="mt-3 break-words text-sm text-slate-500">From {{ contact.user.username }} ({{ contact.user.email }})</p>
          <p class="mt-5 whitespace-pre-wrap break-words">{{ contact.message }}</p>
        </section>
      }
    }
    <app-admin-contacts-list [contacts]="contacts()" [loading]="loading()" [busy]="busy()" [canEdit]="!!users().length && !usersLoading()"
      [error]="loadError()" (refreshed)="load()" (viewed)="open($event, false)" (edited)="open($event, true)" (deleted)="askDelete($event)" />
  `,
})
export class AdminContactsDesign {
  private service = inject(ContactsService);
  private admin = inject(AdminService);
  contacts = signal<Contact[]>([]);
  users = signal<AdminUser[]>([]);
  loading = signal(false);
  usersLoading = signal(false);
  busy = signal(false);
  loadError = signal('');
  usersError = signal('');
  actionError = signal('');
  success = signal('');
  editing = signal(false);
  selected = signal<Contact | null>(null);
  deleting = signal<Contact | null>(null);

  ngOnInit() { void this.load(); void this.loadUsers(); }
  private clearAction() { this.actionError.set(''); this.success.set(''); this.deleting.set(null); }
  create() {
    if (this.busy() || this.loading() || !this.users().length) return;
    this.clearAction(); this.selected.set(null); this.editing.set(true);
  }
  async open(contact: Contact, edit: boolean) {
    if (this.busy()) return;
    this.clearAction(); this.editing.set(false); this.selected.set(null); this.busy.set(true);
    try { this.selected.set(await this.service.getContact(contact.id)); this.editing.set(edit); }
    catch (error) { this.actionError.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
  askDelete(contact: Contact) {
    if (this.busy()) return;
    this.clearAction(); this.editing.set(false); this.selected.set(null); this.deleting.set(contact);
  }
  async load() {
    if (this.loading()) return;
    this.loading.set(true); this.loadError.set('');
    try { this.contacts.set(await this.service.getContacts()); }
    catch (error) { this.loadError.set(errorMessage(error)); }
    finally { this.loading.set(false); }
  }
  async loadUsers() {
    if (this.usersLoading()) return;
    this.usersLoading.set(true); this.usersError.set('');
    try { this.users.set(await this.admin.getUsers()); }
    catch (error) { this.usersError.set('Unable to load senders. ' + errorMessage(error)); }
    finally { this.usersLoading.set(false); }
  }
  async save(input: CreateContactInput) {
    if (this.busy() || this.loading()) return;
    const subject = input.subject.trim(), message = input.message.trim();
    if (!subject || !message || !this.users().some(user => user.id === input.userId)) {
      this.actionError.set('Select an existing sender and enter a subject and message.'); return;
    }
    this.busy.set(true); this.actionError.set(''); this.success.set('');
    try {
      const contact = this.selected();
      const value = { userId: input.userId, subject, message };
      if (contact) await this.service.updateContact(contact.id, value);
      else await this.service.createContact(value);
      this.editing.set(false); this.selected.set(null); this.success.set('Contact saved.'); await this.load();
    } catch (error) { this.actionError.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
  async confirmDelete() {
    const contact = this.deleting();
    if (!contact || this.busy() || this.loading()) return;
    this.busy.set(true); this.actionError.set(''); this.success.set('');
    try {
      await this.service.deleteContact(contact.id);
      this.deleting.set(null); this.success.set('Contact deleted.'); await this.load();
    } catch (error) { this.actionError.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
}
