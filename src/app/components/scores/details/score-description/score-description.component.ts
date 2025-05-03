import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Score } from '@models/score';

@Component({
  selector: 'app-score-description',
  imports: [
    RouterLink,
  ],
  templateUrl: './score-description.component.html',
  styleUrl: './score-description.component.scss'
})
export class ScoreDescriptionComponent {
  @Input({ required: true })
  public score!: Score;
}
