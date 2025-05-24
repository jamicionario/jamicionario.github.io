import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { JamModeService } from '@services/jam-mode.service';
import { MatSlideToggle, MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component({
  selector: 'app-jam-mode-toggler',
  imports: [
    AsyncPipe,
    MatSlideToggle,
  ],
  templateUrl: './jam-mode-toggler.component.html',
  styleUrl: './jam-mode-toggler.component.scss'
})
export class JamModeTogglerComponent {
  private readonly jamModeService = inject(JamModeService);

  readonly isJamMode$ = this.jamModeService.isJamMode$;

  toggleJamModeTo($event: MatSlideToggleChange) {
    this.jamModeService.jamMode = $event.checked;
  }
}
