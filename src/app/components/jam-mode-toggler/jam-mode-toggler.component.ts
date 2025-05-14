import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { JamModeService } from '@services/jam-mode.service';

@Component({
  selector: 'app-jam-mode-toggler',
  imports: [
    AsyncPipe,
  ],
  templateUrl: './jam-mode-toggler.component.html',
  styleUrl: './jam-mode-toggler.component.scss'
})
export class JamModeTogglerComponent {
  jamModeService = inject(JamModeService);
  isJamMode$ = this.jamModeService.isJamMode$;

  toggleJamModeFrom(previousValue: boolean | null): void {
    this.jamModeService.jamMode = !previousValue;
  }
}
