import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  imports: [],
  template: `     <footer>
        <div class="mx-auto flex max-w-6xl flex-col gap-2 px-4 py-7 text-sm text-slate-500 sm:flex-row sm:items-center sm:justify-between sm:px-6 lg:px-8">
            <p class="font-semibold text-slate-700">BridgeStartup</p>
            <p>© 2026 BridgeStartup. All rights reserved.</p>
        </div>
    </footer> `,
  styles: ``,
})
export class Footer {}
