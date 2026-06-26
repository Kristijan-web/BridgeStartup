import { Component, input } from '@angular/core';
import { PostsInterface } from '../../../../../interfaces/posts-interface';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-post-item',
  imports: [RouterLink],
  template: `                  <article class="flex min-h-[340px] flex-col rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm">
                        <div class="mb-4 flex flex-wrap gap-2">
                            <span class="rounded-lg bg-indigo-50 px-3 py-1 text-xs font-black text-indigo-700">React</span>
                            <span class="rounded-lg bg-cyan-50 px-3 py-1 text-xs font-black text-cyan-700">Node.js</span>
                            <span class="rounded-lg bg-emerald-50 px-3 py-1 text-xs font-black text-emerald-700">PostgreSQL</span>
                        </div>
                        
                        <h3 class="text-xl font-black">{{ post().title }}</h3>
                        <p class="mt-3 flex-1 leading-7 text-slate-600">{{ post().description }}</p>
                        <div class="mt-5 border-t border-slate-200 pt-4 text-sm">
                            <p class="font-bold text-slate-800">{{ post().phone }}</p>
                            <p class="mt-1 font-bold text-indigo-600">{{ post().email }}</p>
                            <a routerLink="/posts/{{post().id}}" class="mt-4 inline-flex rounded-lg bg-slate-950 px-4 py-2 text-sm font-bold text-white">View post</a>
                        </div>  
                    </article> `,
  styles: ``,
})
export class PostItem {


  post = input.required<PostsInterface>();


  
}
