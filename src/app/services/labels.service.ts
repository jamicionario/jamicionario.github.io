import { Injectable } from '@angular/core';
import rawCategories from '@public/search-categories.json';
import { ScoreLabel } from '@models/score-label';


@Injectable({
  providedIn: 'root'
})
export class LabelsService {
  readonly labels: ScoreLabel[] = <ScoreLabel[]>rawCategories;

  getAll(): ScoreLabel[] {
    return this.labels;
  }
}