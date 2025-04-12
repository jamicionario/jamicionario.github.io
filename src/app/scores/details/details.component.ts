import { Component, inject } from '@angular/core';
import { ScoreService } from '../scores.service';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';
import { AsyncPipe } from '@angular/common';

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
  score$ = this.route.paramMap.pipe(
    map(paramMap => Number(paramMap.get('number'))),
    map(scoreNumber => this.service.getScore(scoreNumber)),
  );

  scoreTitle$ = this.score$.pipe(
    map(score =>
        score?.Pages.length === 1
        ? "1 page:"
        : (score?.Pages.length ?? 0) + " pages:"
      ),
  );
}
