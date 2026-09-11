import { Component } from '@angular/core';
import { StatusDesign } from '../components/StatusDesign/status-design';

@Component({
  selector: 'app-status-page',
  imports: [StatusDesign],
  template: `<app-status-design></app-status-design>`,
  styles: ``,
})
export class StatusPage {}
