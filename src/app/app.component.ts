import { Component, HostBinding, inject, OnDestroy, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { JamModeService } from '@services/jam-mode.service';
import { JamModeTogglerComponent } from "./components/jam-mode-toggler/jam-mode-toggler.component";
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';
import { FooterComponent } from '@components/footer/footer.component';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    JamModeTogglerComponent,
    FooterComponent,
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
  private isJamMode: boolean = true;

  private readonly jamModeService = inject(JamModeService);
  private readonly isJamMode$: BehaviorSubject<boolean> = this.jamModeService.isJamMode$;

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
