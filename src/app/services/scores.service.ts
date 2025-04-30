import { Injectable } from '@angular/core';
import rawMetadata from '@public/score-metadata.json';
import labels from '@public/score-labels.json';
import { Category } from '@models/category';
import { ScoreGroup } from '@models/score-group';
import { Score } from '@models/score';


function getMetadata(): Score[] {
  // First we clone and initialize all metadata to ensure they have empty labels.
  const metadata: Score[] = rawMetadata.map(item => new Score(Object.assign({ labels: new Map() }, item)));

  // Then we find the existing labels, and assign them to the Scores.
  addLabelsToMetadata(metadata);
  return metadata;
}

function addLabelsToMetadata(metadata: Score[]): void {
  // In this bag we'll store any label that has a problem, so we can notify the user at the end.
  const notFoundScores: string[] = [];
  labels.forEach((element) => {
    const metadataToLabel: Score | undefined = metadata.find(item => item.name == element.scoreName);
    if (metadataToLabel === undefined) {
      notFoundScores.push(element.scoreName);
    } else {
      metadataToLabel.labels = new Map(Object.entries(element.labels));
    }
  });

  if (notFoundScores.length > 0) {
    // We want to notify of everything at once in a single line, instead of throwing 200 debug log lines if something is broken.
    console.debug(`${notFoundScores.length} labels could not be associated with their scores.`, notFoundScores);
  }
}

@Injectable({
  providedIn: 'root'
})
export class ScoreService {
  private metadata!: Score[];
  private grouped?: ScoreGroup[] = undefined;
  private byCategory?: Category[] = undefined;

  constructor() {
    this.metadata = getMetadata();
  }

  getScores(): Score[] {
    return this.metadata;
  }

  getScore(scoreId: number): Score | undefined {
    return this.metadata.find(score => score.number === scoreId);
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
