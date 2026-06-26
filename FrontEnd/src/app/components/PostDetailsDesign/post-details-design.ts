import { Component } from '@angular/core';

@Component({
  selector: 'app-post-details-design',
  imports: [],
  template: `  <main class="flex-1">
      <section class="mx-auto max-w-6xl px-4 py-12 sm:px-6 lg:px-8">
        <a href="#" class="inline-flex items-center rounded-lg bg-slate-100 px-4 py-2 text-sm font-bold text-slate-700 hover:bg-slate-200">
          Back to posts
        </a>

        <div class="mt-8 grid gap-8 lg:grid-cols-[1fr_360px] lg:items-start">
          <article class="rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm sm:p-8">
            <div class="mb-5 flex flex-wrap gap-2">
              <span class="rounded-lg bg-indigo-50 px-3 py-1 text-xs font-black text-indigo-700">JavaScript</span>
              <span class="rounded-lg bg-cyan-50 px-3 py-1 text-xs font-black text-cyan-700">MVP</span>
              <span class="rounded-lg bg-emerald-50 px-3 py-1 text-xs font-black text-emerald-700">Open post</span>
            </div>

            <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">Post detail</p>
            <h1 class="mt-4 max-w-3xl text-4xl font-black tracking-tight text-slate-950 sm:text-5xl">
              Intelligent Frozen Table
            </h1>

            <p class="mt-5 text-lg leading-8 text-slate-600">
              The slim &amp; simple Maple Gaming Keyboard from Dev Era is designed for everyday use.
            </p>

            <div class="mt-8 grid gap-4 border-y border-slate-200 py-6 sm:grid-cols-3">
              <div>
                <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Created</span>
                <span class="mt-2 block font-bold text-slate-950">Jun 17, 2026</span>
              </div>
              <div>
                <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Founder</span>
                <span class="mt-2 block font-bold text-slate-950">Unknown user</span>
              </div>
              <div>
                <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Status</span>
                <span class="mt-2 block font-bold text-emerald-600">Available</span>
              </div>
            </div>

            <div class="mt-8">
              <h2 class="text-2xl font-black tracking-tight">Project overview</h2>
              <div class="mt-4 space-y-4 leading-8 text-slate-600">
                <p>
                  This startup post is presented as a focused opportunity card expanded into a readable detail page.
                  The founder can explain the idea, show the preferred stack, and leave direct contact information for
                  developers who want to join the first build.
                </p>
                <p>
                  The current BridgeStartup design is intentionally static and clean, so the post detail keeps the same
                  simple structure: strong title, visible stack badges, concise description, and a clear contact panel.
                </p>
              </div>
            </div>

          </article>

          <aside class="space-y-5">
            <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">Contact</p>
              <div class="mt-5 space-y-4">
                <div>
                  <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Phone</span>
                  <p class="mt-2 font-bold text-slate-950">(601) 695-6017 x6329</p>
                </div>
                <div>
                  <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Email</span>
                  <p class="mt-2 break-words font-bold text-indigo-600">Sylvester_Smitham@yahoo.com</p>
                </div>
              </div>
              <a href="mailto:Sylvester_Smitham@yahoo.com" class="mt-6 inline-flex w-full justify-center rounded-lg bg-slate-950 px-5 py-3 text-sm font-black text-white shadow-sm">
                Send message
              </a>
            </div>

            <div class="rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm">
              <p class="text-sm font-black uppercase tracking-[0.22em] text-cyan-700">Stack</p>
              <div class="mt-5 flex flex-wrap gap-2">
                <span class="rounded-lg bg-indigo-50 px-3 py-1 text-xs font-black text-indigo-700">JavaScript</span>
                <span class="rounded-lg bg-cyan-50 px-3 py-1 text-xs font-black text-cyan-700">Frontend</span>
                <span class="rounded-lg bg-emerald-50 px-3 py-1 text-xs font-black text-emerald-700">Static MVP</span>
              </div>
            </div>

            <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
              <span class="block text-2xl font-black text-indigo-600">1</span>
              <span class="text-sm font-semibold text-slate-500">matching stack badge</span>
            </div>
          </aside>
        </div>
      </section>
    </main>
 `,
  styles: ``,
})
export class PostDetailsDesign {
  // Ova componenta treba da pokupi id iz urla i da prosledi endpointu 
}
