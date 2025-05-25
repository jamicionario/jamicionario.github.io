import { Component, inject, OnInit } from '@angular/core';
import { ScoresService } from '@services/scores.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { map, Observable, switchMap } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { Score } from '@models/score';
import { ScoreDescriptionComponent } from './score-description/score-description.component';
import { Hotkeys, KnownKey } from '@utils/hot-keys';

@Component({
  selector: 'app-details',
  imports: [
    AsyncPipe,
    ScoreDescriptionComponent,
    RouterLink,
  ],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss'
})
export class DetailsComponent implements OnInit {
  private readonly service = inject(ScoresService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly hotKeys = inject(Hotkeys);

  readonly totalScores: number = this.service.getTotal();

  score$: Observable<Score | undefined> = this.route.paramMap.pipe(
    map(paramMap => Number(paramMap.get('number'))),
    map(scoreNumber => this.service.get(scoreNumber)),
  );
  previous$: Observable<Score | undefined> = this.score$.pipe(
    map(score => score === undefined ? undefined : this.service.get(score?.number - 1)),
  );
  next$: Observable<Score | undefined> = this.score$.pipe(
    map(score => score === undefined ? undefined : this.service.get(score?.number + 1)),
  );

  readonly scoreHeader$ = this.score$.pipe(
    map(score =>
      score?.pages.length === 1
        ? "1 page:"
        : (score?.pages.length ?? 0) + " pages:"
    ),
  );

  ngOnInit(): void {
    this.setupKeyboardNavigation(KnownKey.ArrowLeft, this.previous$);
    this.setupKeyboardNavigation(KnownKey.ArrowRight, this.next$);
  }

  private setupKeyboardNavigation(key: KnownKey, target: Observable<Score | undefined>): void {
    target.pipe(
      switchMap(score => this.hotKeys.registerShortcut(key)
        .pipe(
          map(_event => score),
        )),
    ).subscribe({
      next: score => {
        if (score) {
          this.router.navigate(['/scores', score.number]);
        }
      },
    });
  }
}
