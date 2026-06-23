import { Component, inject } from '@angular/core';
import { Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-home-page',
  imports: [],
  template: `  
        <section class="mx-auto max-w-6xl px-4 py-12 sm:px-6 lg:px-8">
            <div class="grid gap-8 lg:grid-cols-[0.95fr_1.05fr] lg:items-end">
                <div>
                    <p class="text-sm font-black uppercase tracking-[0.22em] text-cyan-700">Startup opportunities</p>
                    <h1 class="mt-4 max-w-2xl text-4xl font-black tracking-tight text-slate-950 sm:text-5xl">Bridge the gap between founders and builders.</h1>
                    <p class="mt-5 max-w-2xl text-lg leading-8 text-slate-600">BridgeStartup is a static board for startup posts. Founders can present an idea, stack, contact details, and project stage, while developers can quickly decide whether it fits them.</p>
                </div>
                <div class="grid gap-3 sm:grid-cols-3">
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-indigo-600">6</span>
                        <span class="text-sm font-semibold text-slate-500">open posts</span>
                    </div>
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-emerald-600">24</span>
                        <span class="text-sm font-semibold text-slate-500">stack badges</span>
                    </div>
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-amber-500">0</span>
                        <span class="text-sm font-semibold text-slate-500">dynamic parts</span>
                    </div>
                </div>
            </div>
        </section>

        <section class="border-y border-slate-200 bg-white">
            <div class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
                <div class="mb-7 flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
                    <div>
                        <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">Posts</p>
                        <h2 class="mt-2 text-3xl font-black tracking-tight">Fresh startup ideas</h2>
                    </div>
                   
                </div>

                <div class="grid gap-5 lg:grid-cols-3">
                    <article class="flex min-h-[340px] flex-col rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm">
                        <div class="mb-4 flex flex-wrap gap-2">
                            <span class="rounded-lg bg-indigo-50 px-3 py-1 text-xs font-black text-indigo-700">React</span>
                            <span class="rounded-lg bg-cyan-50 px-3 py-1 text-xs font-black text-cyan-700">Node.js</span>
                            <span class="rounded-lg bg-emerald-50 px-3 py-1 text-xs font-black text-emerald-700">PostgreSQL</span>
                        </div>
                        <h3 class="text-xl font-black">Mentorship platform</h3>
                        <p class="mt-3 flex-1 leading-7 text-slate-600">The founder is looking for a team to build an MVP platform that connects experienced mentors with young entrepreneurs. The focus is on profiles, scheduling, and payments.</p>
                        <div class="mt-5 border-t border-slate-200 pt-4 text-sm">
                            <p class="font-bold text-slate-800">+381 60 118 2400</p>
                            <p class="mt-1 font-bold text-indigo-600">hello@mentorloop.dev</p>
                            <a href="post-detail.html" class="mt-4 inline-flex rounded-lg bg-slate-950 px-4 py-2 text-sm font-bold text-white">View post</a>
                        </div>
                    </article>

                    <article class="flex min-h-[340px] flex-col rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm">
                        <div class="mb-4 flex flex-wrap gap-2">
                            <span class="rounded-lg bg-cyan-50 px-3 py-1 text-xs font-black text-cyan-700">Python</span>
                            <span class="rounded-lg bg-emerald-50 px-3 py-1 text-xs font-black text-emerald-700">FastAPI</span>
                            <span class="rounded-lg bg-violet-50 px-3 py-1 text-xs font-black text-violet-700">AI</span>
                        </div>
                        <h3 class="text-xl font-black">AI resume assistant</h3>
                        <p class="mt-3 flex-1 leading-7 text-slate-600">The startup is building a tool that helps candidates tailor their resume to a specific job posting. A prototype with upload and text analysis is needed.</p>
                        <div class="mt-5 border-t border-slate-200 pt-4 text-sm">
                            <p class="font-bold text-slate-800">+381 64 881 9021</p>
                            <p class="mt-1 font-bold text-indigo-600">team@cvpilot.ai</p>
                            <a href="post-detail.html" class="mt-4 inline-flex rounded-lg border border-slate-300 px-4 py-2 text-sm font-bold text-slate-800">View post</a>
                        </div>
                    </article>

                    <article class="flex min-h-[340px] flex-col rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm">
                        <div class="mb-4 flex flex-wrap gap-2">
                            <span class="rounded-lg bg-slate-100 px-3 py-1 text-xs font-black text-slate-700">Next.js</span>
                            <span class="rounded-lg bg-emerald-50 px-3 py-1 text-xs font-black text-emerald-700">Supabase</span>
                            <span class="rounded-lg bg-cyan-50 px-3 py-1 text-xs font-black text-cyan-700">Tailwind</span>
                        </div>
                        <h3 class="text-xl font-black">Event venue marketplace</h3>
                        <p class="mt-3 flex-1 leading-7 text-slate-600">A Belgrade-based team is looking for a frontend developer for a venue catalog, filters, booking inquiries, and a simple owner dashboard.</p>
                        <div class="mt-5 border-t border-slate-200 pt-4 text-sm">
                            <p class="font-bold text-slate-800">+381 63 440 2211</p>
                            <p class="mt-1 font-bold text-indigo-600">founder@spaceyard.co</p>
                            <a href="post-detail.html" class="mt-4 inline-flex rounded-lg border border-slate-300 px-4 py-2 text-sm font-bold text-slate-800">View post</a>
                        </div>
                    </article>
                </div>
            </div>
        </section>
   `,
  styles: ``,
})
export class HomePage {
    private meta = inject(Meta);

  constructor() {
    this.meta.updateTag({
      name: 'description',
      content: 'Browse posts, projects, and opportunities on the BridgeStartup platform.'
    });
  }
}
