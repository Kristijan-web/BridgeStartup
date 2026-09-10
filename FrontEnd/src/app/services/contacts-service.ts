import { inject, Injectable } from '@angular/core';
import { CreateContactInput } from '../interfaces/contact-interface';
import { ApiService } from './api-service';

@Injectable({ providedIn: 'root' })
export class ContactsService {
  private api = inject(ApiService);

  createContact(contact: CreateContactInput): Promise<void> {
    return this.api.request('/contacts', { method: 'POST', body: JSON.stringify(contact) });
  }
}
