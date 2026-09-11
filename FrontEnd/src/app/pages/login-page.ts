import { Component } from '@angular/core';
import { LoginDesign } from '../components/LoginDesign/login-design';

@Component({
  selector: 'app-login-page',
  imports: [LoginDesign],
  template: `<app-login-design></app-login-design>`,
  styles: ``,
})
export class LoginPage {}
