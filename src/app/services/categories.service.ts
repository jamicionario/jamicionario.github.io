import { inject, Injectable } from '@angular/core';
import { Category } from '@models/category';
import rawCategories from '@public/search-categories.json';
import { ScoresService } from './scores.service';
import { Score } from '@models/score';

// Keep synchronized with Categories class in file MetadataBuilder.cs .
export enum CategoriesOfInterest {
  Region = 'region',
  TypeOfDance = 'type of dance',
}

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {
  private readonly categories: Category[];

  private readonly scoreService = inject(ScoresService);

  constructor() {
    this.categories = CategoriesService.buildCategories(this.scoreService);
  }

  /**
   * Gets the categories, such as An Dro, Repasseado, etc.
   * Includes the scores for each of the categories, including a category for holding all "orphan" scores.
   * @returns the categories with their scores.
   */
  getAll(): Category[] {
    return this.categories;
  }

  /**
   * Gets a specific category, by name.
   * @param categoryName The name of the category to get.
   * @returns the category, with its scores.
   */
  get(categoryName: string): Category | undefined {
    return this.categories.find(cat => cat.name == categoryName);
  }


  private static buildCategories(scoreService: ScoresService): Category[] {
    const rawCategory = rawCategories.find(cat => cat.name == CategoriesOfInterest.TypeOfDance);
    if (rawCategory === undefined) {
      throw new Error("Cannot parse metadata files.");
    }

    // Build the categories, from the existing metadata.
    const scores: Score[] = scoreService.getAll();
    const categories: Category[] = rawCategory
      .values
      .map(categoryName => {
        return new Category(
          categoryName,
          scores.filter(score => score.typeOfDance == categoryName)
        )
      });

    // Include an "Other" category, for the scores that are not categorized.
    const assignedScoreNumbers: number[] = categories.flatMap(cat => cat.scores.map(score => score.number));
    const unassignedScores = scores.filter(score => !assignedScoreNumbers.includes(score.number));
    // Categories in the metadata are sorted. We put "other" at the end.
    categories.push(new Category('Other', unassignedScores));

    return categories;
  }
}