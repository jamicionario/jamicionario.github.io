import { Component, inject, Input } from '@angular/core';
import { Score, ScoreService } from '../scores.service';
import { ActivatedRoute } from '@angular/router';
import { map, Observable, tap } from 'rxjs';
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
    map(paramMap => Number(paramMap.get('id'))),
    map(scoreId => this.service.getScore(scoreId)),
  );
}
