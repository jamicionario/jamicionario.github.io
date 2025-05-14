import { Component, inject } from '@angular/core';
import { ScoresService } from '@services/scores.service';
import { ActivatedRoute } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { Score } from '@models/score';
import { ScoreDescriptionComponent } from './score-description/score-description.component';

@Component({
  selector: 'app-details',
  imports: [
    AsyncPipe,
    ScoreDescriptionComponent,
  ],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss'
})
export class DetailsComponent {
  private service = inject(ScoresService);
  private route = inject(ActivatedRoute);
  score$ : Observable<Score | undefined> = this.route.paramMap.pipe(
    map(paramMap => Number(paramMap.get('number'))),
    map(scoreNumber => this.service.get(scoreNumber)),
  );

  scoreHeader$ = this.score$.pipe(
    map(score =>
        score?.pages.length === 1
        ? "1 page:"
        : (score?.pages.length ?? 0) + " pages:"
      ),
  );
}
