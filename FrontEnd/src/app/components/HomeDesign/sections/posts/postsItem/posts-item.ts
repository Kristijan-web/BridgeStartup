import { Component, computed, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { PostsInterface } from '../../../../../interfaces/posts-interface';
import { PostBadgesInterface } from '../../../../../interfaces/post-badges-interface';
import { BadgesInterface } from '../../../../../interfaces/badges-interface';

import { PostsBadgeService } from '../../../../../services/posts-badge-service';
import { BadgesService } from '../../../../../services/badges-service';

import { BadgeItem } from '../../../../../common/badge-item';

@Component({
  selector: 'app-post-item',
  imports: [RouterLink, BadgeItem],
  template: `
    <article
      class="flex h-[340px] flex-col overflow-hidden rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm"
    >
      <div class="mb-4 flex max-h-7 flex-wrap gap-2 overflow-hidden">
        @for (badge of postBadges(); track badge.id) {
          <app-badge-item [badgeName]="badge.name"></app-badge-item>
        }
      </div>

      <h3 class="line-clamp-2 text-xl font-black">{{ post().title }}</h3>

      <p class="mt-3 line-clamp-4 flex-1 leading-7 text-slate-600">
        {{ post().description }}
      </p>

      <div class="mt-5 border-t border-slate-200 pt-4 text-sm">
        <p class="truncate font-bold text-slate-800">{{ post().phone }}</p>

        <p class="mt-1 truncate font-bold text-indigo-600">
          {{ post().email }}
        </p>

        <a
          [routerLink]="['/posts', post().id]"
          class="mt-4 inline-flex rounded-lg bg-slate-950 px-4 py-2 text-sm font-bold text-white"
        >
          View post
        </a>
      </div>
    </article>
  `,
  styles: ``,
})
export class PostItem {
  private postBadgesService = inject(PostsBadgeService);
  private badgesService = inject(BadgesService);

  post = input.required<PostsInterface>();

  postBadgesIds = signal<PostBadgesInterface[] | undefined>([]);

  badges = signal<BadgesInterface[]>([]);

  // Badgevi koji pripadaju trenutnom postu
  postBadges = computed(() => {
    const badgeIds = this.postBadgesIds()?.map(postBadge => Number(postBadge.badgeId));

    const filteredBadges = this.badges().filter(badge =>{   
         
    return  badgeIds?.includes(Number(badge.id))
    }
    
    );
  
    return filteredBadges;
  });

  ngOnInit() {
    this.postBadgesService
      .getPostBadges(this.post().id)
      .then(data => {
        this.postBadgesIds.set(data);
      });

    this.badgesService
      .getAllBadges()
      .then(data => {
        this.badges.set(data);
      });
  }
}
