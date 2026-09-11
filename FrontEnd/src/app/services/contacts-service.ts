import { inject, Injectable } from '@angular/core';
import { Contact, CreateContactInput } from '../interfaces/contact-interface';
import { ApiService } from './api-service';

@Injectable({ providedIn: 'root' })
export class ContactsService {
  private api = inject(ApiService);

  getContacts(): Promise<Contact[]> { return this.api.request('/contacts'); }

  getContact(id: number): Promise<Contact> { return this.api.request('/contacts/' + id); }

  createContact(contact: CreateContactInput): Promise<void> {
    return this.api.request('/contacts', { method: 'POST', body: JSON.stringify(contact) });
  }

  updateContact(id: number, contact: CreateContactInput): Promise<void> {
    return this.api.request('/contacts/' + id, { method: 'PUT', body: JSON.stringify(contact) });
  }

  deleteContact(id: number): Promise<void> {
    return this.api.request('/contacts/' + id, { method: 'DELETE' });
  }
}
