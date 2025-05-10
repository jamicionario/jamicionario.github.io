import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ScoresService } from '@services/scores.service';
import { Score } from '@models/score';
import { ScoreGroup } from '@models/score-group';
import { Category } from '@models/category';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TreeComponent } from './tree/tree.component';
import { BehaviorSubject, debounceTime, distinctUntilChanged, map } from 'rxjs';
import { CategoriesService } from '@services/categories.service';

export enum SelectionType {
  List = 'list',
  Tree = 'tree',
  Categories = 'categories',
}

function normalizeStringForSearch(value: string): string {
  return value.toLowerCase().trim();
}

@Component({
  selector: 'app-list',
  imports: [
    RouterLink,
    CommonModule,
    FormsModule,
    TreeComponent,
],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent {
  private readonly scoreService = inject(ScoresService);
  private readonly categoriesService = inject(CategoriesService);
  readonly SelectionType = SelectionType;

  searchText: string = "";
  search$ = new BehaviorSubject<string>(this.searchText);
  normalizedSearch$ = this.search$
    .pipe(
      debounceTime(200),
      map(str => normalizeStringForSearch(str)),
      distinctUntilChanged(),
    );

  scores: Score[] = this.scoreService.getAll();
  groupedScores: ScoreGroup[] = this.scoreService.getGroupedScores();
  categories: Category[] = this.categoriesService.getAll();

  scoresFiltered$ = this.normalizedSearch$
    .pipe(
      map(str => this.refilterScores(str, this.scores)),
    );

  categoriesFiltered$ = this.normalizedSearch$
    .pipe(
      map(str => this.categories
        .map(category => new Category(
          category.name,
          this.refilterScores(str, category.scores))
        )
      ),
      map(cats => cats.filter(cat => cat.scores.length > 0)),
    );

  groupedScoresFiltered$ = this.normalizedSearch$
    .pipe(
      map(str => this.groupedScores.map(group => this.refilterGroupedScore(str, group))),
      map(groups => groups.filter(group => group.isEmpty === false)),
    );

  selectionType: SelectionType = SelectionType.Categories;
  changeSelectionTypeTo(type: SelectionType): void {
    this.selectionType = type;
  }

  private refilterScores(str: string, scores: Score[]): Score[] {
    return scores.filter(score => str.length === 0 || score.searchableName.includes(str))
  }

  private refilterGroupedScore(search: string, group: ScoreGroup): ScoreGroup {
    if (search == '') {
      // No filter, return all.
      return group;
    }

    if (group.name.includes(search)) {
      // Filter matches this group, so we return all children.
      return group;
    }

    const filteredBranches = group.branches
      .map(branch => this.refilterGroupedScore(search, branch))
      .filter(branch => branch.isEmpty === false);
    const filteredLeaves = this.refilterScores(search, group.leaves);
    const filtered = new ScoreGroup(group.name, group.parent);
    Object.assign(filtered, group, {
      branches: filteredBranches,
      leaves: filteredLeaves,
    });
    return filtered;
  }
}
