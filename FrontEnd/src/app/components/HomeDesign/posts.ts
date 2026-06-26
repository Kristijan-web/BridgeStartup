import { Component, inject, signal } from '@angular/core';
import { PostItem } from './post-item';
import { PostsService } from '../../services/posts-service';
import { PostsInterface } from '../../interfaces/posts-interface';

@Component({
  selector: 'app-posts',
  imports: [PostItem],
  template: `   <section class="border-y border-slate-200 bg-white">
            <div class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
                <div class="mb-7 flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
                    <div>
                        <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">Posts</p>
                        <h2 class="mt-2 text-3xl font-black tracking-tight">Fresh startup ideas</h2>
                    </div>
                   
                </div>

                <div class="grid gap-5 lg:grid-cols-3">
           
              @for (post of posts(); track post.id) {

                <app-post-item [post]="post"></app-post-item>

              }


                </div>
            </div>
        </section> `,
  styles: ``,
})
export class Posts {

  
    postsService: PostsService = inject(PostsService) 

  posts= signal<PostsInterface[]>([]);

  
  ngOnInit() {
 
    this.postsService.getAllPosts().then(data => {
      this.posts.set(data);
    });
   


    
  }

}
