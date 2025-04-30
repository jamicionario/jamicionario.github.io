import { Component, inject } from '@angular/core';
import { ScoreService } from '@services/scores.service';
import { ActivatedRoute } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { Score } from '@models/score';

@Component({
  selector: 'app-details',
  imports: [
    AsyncPipe,
  ],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss'
})
export class DetailsComponent {
  private service = inject(ScoreService);
  private route = inject(ActivatedRoute);
  score$ : Observable<Score | undefined> = this.route.paramMap.pipe(
    map(paramMap => Number(paramMap.get('number'))),
    map(scoreNumber => this.service.getScore(scoreNumber)),
  );

  scoreTitle$ = this.score$.pipe(
    map(score =>
        score?.pages.length === 1
        ? "1 page:"
        : (score?.pages.length ?? 0) + " pages:"
      ),
  );
}
