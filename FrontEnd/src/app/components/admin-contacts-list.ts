import { Component, computed, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Contact } from '../interfaces/contact-interface';

@Component({
  selector: 'app-admin-contacts-list',
  imports: [FormsModule],
  template: `
    <section class="panel mt-6" aria-label="Contact messages">
      <div class="mb-5 flex flex-wrap gap-3">
        <label class="grow"><span class="sr-only">Search contacts</span>
          <input name="contactSearch" placeholder="Search subject, message, or sender" [ngModel]="search()" (ngModelChange)="search.set($event)">
        </label>
        <button class="button secondary" (click)="refreshed.emit()" [disabled]="loading() || busy()">Refresh</button>
      </div>
      @if (loading()) { <p role="status">Loading contacts...</p> }
      @else if (error()) { <p class="error" role="alert">{{ error() }} <button class="underline" (click)="refreshed.emit()" [disabled]="busy()">Retry</button></p> }
      @else {
        <p class="help mb-3">{{ filtered().length }} contacts</p>
        <div class="overflow-x-auto"><table class="w-full">
          <thead><tr><th>Subject</th><th>Sender</th><th>Actions</th></tr></thead>
          <tbody>
            @for (contact of filtered(); track contact.id) {
              <tr>
                <td class="max-w-xs break-words font-bold">{{ contact.subject }}</td>
                <td class="max-w-xs break-words">{{ contact.user.username }}<br><span class="text-slate-500">{{ contact.user.email }}</span></td>
                <td><div class="flex gap-2">
                  <button class="button secondary" (click)="viewed.emit(contact)" [disabled]="busy()" [attr.aria-label]="'View contact ' + contact.id">View</button>
                  <button class="button secondary" (click)="edited.emit(contact)" [disabled]="busy() || !canEdit()" [attr.aria-label]="'Edit contact ' + contact.id">Edit</button>
                  <button class="button secondary" (click)="deleted.emit(contact)" [disabled]="busy()" [attr.aria-label]="'Delete contact ' + contact.id">Delete</button>
                </div></td>
              </tr>
            } @empty { <tr><td colspan="3">{{ contacts().length ? 'No contacts match your search.' : 'No contacts yet.' }}</td></tr> }
          </tbody>
        </table></div>
      }
    </section>
  `,
})
export class AdminContactsList {
  contacts = input.required<Contact[]>();
  loading = input(false);
  busy = input(false);
  canEdit = input(true);
  error = input('');
  refreshed = output<void>();
  viewed = output<Contact>();
  edited = output<Contact>();
  deleted = output<Contact>();
  search = signal('');
  filtered = computed(() => {
    const term = this.search().trim().toLowerCase();
    return this.contacts().filter(contact =>
      [contact.subject, contact.message, contact.user.username, contact.user.email].join(' ').toLowerCase().includes(term));
  });
}
