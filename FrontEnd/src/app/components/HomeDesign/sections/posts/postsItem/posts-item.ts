import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PostsInterface } from '../../../../../interfaces/posts-interface';
import { BadgeItem } from '../../../../../common/badge-item';
@Component({
  selector: 'app-post-item', imports: [RouterLink, BadgeItem],
  template: `<article class="flex h-full flex-col rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm">
    <div class="mb-4 flex flex-wrap gap-2">
      @for (badge of post().badges; track badge) { <app-badge-item [badgeName]="badge" /> }
    </div>
    <h3 class="line-clamp-2 text-xl font-black">{{ post().title }}</h3>
    <p class="mt-3 line-clamp-4 flex-1 leading-7 text-slate-600">{{ post().description }}</p>
    <div class="mt-5 border-t border-slate-200 pt-4 text-sm">
      <p class="font-bold">{{ post().user.username }}</p>
      <p class="mt-1 truncate text-indigo-600">{{ post().email || post().user.email }}</p>
      <a [routerLink]="['/posts', post().id]" class="button mt-4">View post</a>
    </div>
  </article>`
})
export class PostItem { post = input.required<PostsInterface>(); }
