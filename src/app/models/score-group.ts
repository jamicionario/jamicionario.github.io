import { Category } from './category';
import { Score } from './score';

export class ScoreGroup extends Category {
  subGroups: ScoreGroup[];
  parent?: ScoreGroup;

  constructor(name: string, parent: ScoreGroup | undefined, scores: Score[] = [], subGroups: ScoreGroup[] = []) {
    super(name, scores);
    this.parent = parent;
    this.subGroups = subGroups;
  }

  override get isNotEmpty(): boolean {
    return super.isNotEmpty
      || this.subGroups.length > 0;
  }
}
