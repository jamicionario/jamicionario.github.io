import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withInMemoryScrolling({
      // These two values allow us to navigate to fragments. Example: /download#files
      scrollPositionRestoration: 'enabled',
      anchorScrolling: 'enabled',
    })),
    provideClientHydration(withEventReplay()),
    // TODO: extend TitleStrategy to include score name in title.
  ]
};
