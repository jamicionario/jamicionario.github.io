import { DOCUMENT } from "@angular/common";
import { Inject, Injectable } from "@angular/core";
import { EventManager } from "@angular/platform-browser";
import { Observable, TeardownLogic } from "rxjs";

export enum KnownKey {
    ArrowLeft = 'ArrowLeft',
    ArrowRight = 'ArrowRight',
}

/**
 * Copyright netanelbasal:
 * https://medium.com/netanelbasal/diy-keyboard-shortcuts-in-your-angular-application-4704734547a2
 * With edits for TypeScript fixes, typing, and simplification to adapt to local needs.
 */
@Injectable({ providedIn: 'root' })
export class Hotkeys {
    constructor(
        private eventManager: EventManager,
        @Inject(DOCUMENT) private document: Document
    ) {
    }

    /**
     * Registers a shortcut key, for key-up.
     * The handler that is registered will be unregistered when the Observable is released.
     * @param key The keyboard shortcut to monitor, such as "LeftArrow" or "CTRL+K".
     * @param element The HTML element to monitor for the keyboard shortcut, or none to attach to the document itself.
     * @returns An observable that emits when the shortcut is pressed.
     */
    registerShortcut(key: KnownKey, element?: HTMLElement): Observable<Event> {
        const eventName = `keyup.${key}`;
        const target: HTMLElement = element ?? this.document.body;

        return new Observable<Event>(observer => {
            const handler: Function = (e: Event): void => {
                e.preventDefault()
                observer.next(e);
            };

            const removeHandler: Function = this.eventManager.addEventListener(
                target, eventName, handler
            );

            const teardown: TeardownLogic = () => {
                removeHandler();
                observer.complete();
            };

            return teardown;
        })
    }
}
