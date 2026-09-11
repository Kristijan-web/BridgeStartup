import { Component } from '@angular/core';
import { RegisterDesign } from '../components/RegisterDesign/register-design';

@Component({
  selector: 'app-register-page',
  imports: [RegisterDesign],
  template: `<app-register-design></app-register-design>`,
  styles: ``,
})
export class RegisterPage {}
