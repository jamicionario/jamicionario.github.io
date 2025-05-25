import { DOCUMENT, NgFor } from "@angular/common";
import { Component, Inject, Injectable } from "@angular/core";
import { EventManager } from "@angular/platform-browser";
import { Observable } from "rxjs";
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';

type Options = {
    element: any;
    keys: string;
    description: string;
}

/**
 * Copyright netanelbasal:
 * https://medium.com/netanelbasal/diy-keyboard-shortcuts-in-your-angular-application-4704734547a2
 */
@Injectable({ providedIn: 'root' })
export class Hotkeys {
    readonly defaults: Partial<Options>;

    private readonly hotkeys = new Map<string, string>();

    constructor(private eventManager: EventManager,
        private dialog: MatDialog,
        @Inject(DOCUMENT) private document: Document) {
        this.defaults = {
            element: this.document
        };
        this.addShortcut({ keys: 'shift.?' }).subscribe(() => {
            this.openHelpModal();
        });
    }

    addShortcut(options: Partial<Options>) {
        const merged = { ...this.defaults, ...options };
        const event = `keydown.${merged.keys}`;

        if (merged.keys && merged.description) {
            this.hotkeys.set(merged.keys, merged.description);
        }

        return new Observable(observer => {
            const handler = (e: Event) => {
                e.preventDefault()
                observer.next(e);
            };

            const dispose = this.eventManager.addEventListener(
                merged.element, event, handler
            );

            return () => {
                dispose();
                if (merged.keys) {
                    this.hotkeys.delete(merged.keys);
                }
            };
        })
    }

    openHelpModal() { }

}

@Component({
    template: `<table>
  <tbody>
    <tr *ngFor="let keys of hotkeys">
      <td>{{ hotkeys[1] }}</td>
      <td class="text-right">
        <kbd>{{ hotkeys[0] }}</kbd>
      </td>
    </tr>
  </tbody>
</table>`,
    imports: [
        NgFor,
    ],
})
export class HotkeysDialogComponent {
    hotkeys: any[];

    constructor(@Inject(MAT_DIALOG_DATA) public data: any) {
        this.hotkeys = Array.from(this.data);
    }
}
