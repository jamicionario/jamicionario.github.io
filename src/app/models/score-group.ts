import { Score } from './score';

export class ScoreGroup {
  name: string;
  branches: ScoreGroup[] = [];
  leaves: Score[] = [];
  isCollapsed: boolean = false;
  numberOfScores: number = 0;
  parent?: ScoreGroup;

  constructor(name: string, parent: ScoreGroup | undefined) {
    this.name = name;
    this.parent = parent;
  }

  get isEmpty(): boolean {
    return this.branches.length === 0 && this.leaves.length === 0;
  }

  /**
   * Adds a new score to this group, incrementing the count of scores properly in this group and all its ancestors.
   * @param score The score to add.
   */
  addScore(score: Score): void {
    this.leaves.push(score);
    let node: ScoreGroup | undefined = this;
    while (node !== undefined) {
      node.numberOfScores++;
      node = node.parent;
    }
  }

  addBranch(branch: ScoreGroup): void {
    this.branches.push(branch);
  }
}
