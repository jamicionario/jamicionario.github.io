import { Injectable } from '@angular/core';
import metadata from '@public/score-metadata.json';

export class ScoreGroup {
  name: string;
  branches: ScoreGroup[] = [];
  leaves: Score[] = [];

  constructor(name: string) {
    this.name = name;
  }
};

export type Score = {
  Number: number,
  Name: string,
  SearchableName: string,
  Mscz: string,
  Pages: string[],
  Categories: string[],
};

@Injectable({
  providedIn: 'root'
})
export class ScoreService {
  private scoreGroups?: ScoreGroup[] = undefined;

  getGroupedScores(): ScoreGroup[] {
    if (this.scoreGroups === undefined) {
      this.scoreGroups = ScoreService.groupScores(this.getScores());
    }
    return this.scoreGroups;
  }

  private static groupScores(scores: Score[]): ScoreGroup[] {
    // Create a base dummy node where all the Groups will hang.
    let root = new ScoreGroup("All");
    root.branches = [];

    // Add each score to the groups tree, one by one.
    scores.forEach(score => {
      let branches = root.branches;
      let branch: ScoreGroup | undefined = root;

      // For each category, find or add the needed branch in the tree,
      // then dive inside that branch to handle the next category.
      score.Categories.forEach(category => {
        // Does it exist?
        branch = branches.find(branch => branch.name === category);

        // If not, we create a new one and add it.
        if (branch === undefined) {
          branch = new ScoreGroup(category);
          branches.push(branch);
        }
        // Then we dive in this branch, to take care of the next category.
        branches = branch.branches;
      });
      // After we reached the last category, we insert the score in that final category.
      branch.leaves.push(score);
    });

    // In the end we have a large tree.
    // We want to return all its branches - the set of groups.
    return root.branches;
  }

  getScores(): Score[] {
    return metadata;
  }

  getScore(scoreId: number): Score | undefined {
    return metadata.find(score => score.Number === scoreId);
  }
}
