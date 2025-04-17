import { Injectable } from '@angular/core';
import metadata from '@public/score-metadata.json';

export class Category {
  name: string;
  scores: Score[];
  isCollapsed: boolean = false;

  constructor(name: string, scores: Score[] = []) {
    this.name = name;
    this.scores = scores;
  }
}
export class ScoreGroup {
  name: string;
  branches: ScoreGroup[] = [];
  leaves: Score[] = [];
  isCollapsed: boolean = false;
  isPortuguese: boolean;
  numberOfScores: number = 0;
  parent?: ScoreGroup;

  constructor(name: string, isPortuguese: boolean, parent: ScoreGroup | undefined) {
    this.name = name;
    this.isPortuguese = isPortuguese;
    this.parent = parent;
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
};

export type Score = {
  number: number,
  name: string,
  searchableName: string,
  // mscz: string,
  pages: string[],
  isPortuguese: boolean,
  category: string,
  subcategories: string[],
};

@Injectable({
  providedIn: 'root'
})
export class ScoreService {
  private grouped?: ScoreGroup[] = undefined;
  private byCategory?: Category[] = undefined;

  getScores(): Score[] {
    return metadata;
  }

  getScore(scoreId: number): Score | undefined {
    return metadata.find(score => score.number === scoreId);
  }

  getGroupedScores(): ScoreGroup[] {
    if (this.grouped === undefined) {
      this.grouped = ScoreService.groupScores(this.getScores());
    }
    return this.grouped;
  }

  private static groupScores(scores: Score[]): ScoreGroup[] {
    // Create a base dummy node where all the Groups will hang.
    let root = new ScoreGroup("All", false, undefined);
    root.branches = [];

    // Add each score to the groups tree, one by one.
    scores.forEach(score => {
      let branch: ScoreGroup = root;

      // For each category, find or add the needed branch in the tree,
      // then dive inside that branch to handle the next category.
      const nationality = score.isPortuguese ? "Danças Portuguesas" : "Danças Estrangeiras";
      const allCategories = [nationality, score.category].concat(score.subcategories);
      allCategories.forEach(category => {
        // Does it exist?
        let existing = branch.branches.find(branch => branch.name === category && branch.isPortuguese == score.isPortuguese);

        // If not, we create a new one and add it.
        if (existing === undefined) {
          existing = new ScoreGroup(category, score.isPortuguese, branch);
          branch.addBranch(existing);
        }

        // Then we dive in this branch, to take care of the next category.
        branch = existing;
      });
      // After we reached the last category, we insert the score in that final category.
      branch.addScore(score);
    });

    // In the end we have a large tree.
    // We want to return all its branches - the set of groups.

    // But first we want to swap the portuguese dances to the top.
    // We must have pride! ;)
    root.branches.sort((one, _other) => one.isPortuguese === true ? -1 : 1);
    return root.branches;
  }

  getCategories(): Category[] {
    if (this.byCategory === undefined) {
      this.byCategory = ScoreService.getCategories(this.getScores());
    }
    return this.byCategory;
  }

  private static getCategories(scores: Score[]): Category[] {
    const categories: Category[] = [];
    scores.forEach(score => {
      let existing = categories.find(c => c.name == score.category);
      if (existing === undefined) {
        existing = new Category(score.category);
        categories.push(existing);
      }
      existing.scores.push(score);
    });
    return categories;
  }
}
