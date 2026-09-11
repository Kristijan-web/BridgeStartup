import { Component } from '@angular/core';
import { AdminContactsDesign } from '../components/AdminContactsDesign/admin-contacts-design';

@Component({
  selector: 'app-admin-contacts',
  imports: [AdminContactsDesign],
  template: `<app-admin-contacts-design></app-admin-contacts-design>`,
  styles: ``,
})
export class AdminContactsPage {}
