import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
@Component({
  selector: 'app-status-design', imports: [RouterLink],
  template: `<section class="auth-card"><h1>{{ forbidden ? 'Access denied' : 'Page not found' }}</h1>
    <p class="my-5">{{ forbidden ? 'An administrator account is required to open this page.' : 'This page does not exist.' }}</p>
    <a routerLink="/" class="button">Back to posts</a></section>`
})
export class StatusDesign { forbidden = inject(ActivatedRoute).snapshot.data['forbidden'] === true; }
