import { Score } from './score';


export class Category {
  name: string;
  scores: Score[];
  isCollapsed: boolean = false;

  constructor(name: string, scores: Score[] = []) {
    this.name = name;
    this.scores = scores;
  }
}
