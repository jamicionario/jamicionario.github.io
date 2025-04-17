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

  constructor(name: string) {
    this.name = name;
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
    let root = new ScoreGroup("All");
    root.branches = [];

    // Add each score to the groups tree, one by one.
    scores.forEach(score => {
      let branches = root.branches;
      let branch: ScoreGroup | undefined = root;

      // For each category, find or add the needed branch in the tree,
      // then dive inside that branch to handle the next category.
      const nationality = score.isPortuguese ? "Danças Portuguesas" : "Danças Estrangeiras";
      const allCategories = [nationality, score.category].concat(score.subcategories);
      allCategories.forEach(category => {
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
