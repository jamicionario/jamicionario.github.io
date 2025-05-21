import { normalizeStringForSearch } from '@utils/score-filtering';
import { Score } from './score';
import { Searchable } from './searchable';

export class Category implements Searchable {
  name: string;
  searchableName: string;
  scores: Score[];
  isCollapsed: boolean = false;

  constructor(name: string, scores: Score[] = []) {
    this.name = name;
    this.searchableName = normalizeStringForSearch(name);
    this.scores = scores;
  }

  get numberOfScores(): number {
    return this.scores.length;
  }

  get isNotEmpty(): boolean {
    return this.scores.length > 0;
  }
}
