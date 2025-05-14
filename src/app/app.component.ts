import { Component, HostBinding, inject, OnDestroy, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { JamModeService } from '@services/jam-mode.service';
import { JamModeTogglerComponent } from "./components/jam-mode-toggler/jam-mode-toggler.component";
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    JamModeTogglerComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit, OnDestroy {
  readonly pages = [
    { title: 'Home', url: '/categories' },
    { title: 'Search', url: '/scores' },
    { title: 'Download', url: '/download' },
    { title: 'About', url: '/about' },
  ];
  private readonly destroyed$ = new Subject<void>();

  @HostBinding('class.jam-mode')
  isJamMode: boolean = true;

  jamModeService = inject(JamModeService);
  isJamMode$: BehaviorSubject<boolean> = this.jamModeService.isJamMode$;

  ngOnInit() {
    this.isJamMode$
      .pipe(
        takeUntil(this.destroyed$),
      )
      .subscribe({
        next: value => this.isJamMode = value,
      });

  }
  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
