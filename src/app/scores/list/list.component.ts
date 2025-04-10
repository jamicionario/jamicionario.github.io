import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { Score, ScoreService } from '../scores.service';

@Component({
  selector: 'app-list',
  imports: [
    RouterLink,
    RouterOutlet,
  ],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent {
  private readonly service = inject(ScoreService);

  scores: Score[] = this.service.getScores();
}
