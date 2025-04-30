import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ScoreService } from '@services/scores.service';
import { Score } from '@models/score';
import { ScoreGroup } from '@models/score-group';
import { Category } from '@models/category';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TreeComponent } from './tree/tree.component';
import { BehaviorSubject, debounceTime, distinctUntilChanged, map } from 'rxjs';

export enum SelectionType {
  List,
  Tree,
  Categories,
}

function normalizeStringForSearch(value: string): string {
  return value.toLowerCase().trim();
}

@Component({
  selector: 'app-list',
  imports: [
    RouterOutlet,
    CommonModule,
    TreeComponent,
    FormsModule,
  ],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent {
  private readonly service = inject(ScoreService);
  readonly SelectionType = SelectionType;

  searchText: string = "";
  search$ = new BehaviorSubject<string>(this.searchText);
  normalizedSearch$ = this.search$
    .pipe(
      debounceTime(200),
      map(str => normalizeStringForSearch(str)),
      distinctUntilChanged(),
    );

  scores: Score[] = this.service.getScores();
  groupedScores: ScoreGroup[] = this.service.getGroupedScores();
  categories: Category[] = this.service.getCategories();

  scoresFiltered$ = this.normalizedSearch$
    .pipe(
      map(str => this.scores.filter(score => str.length === 0 || score.searchableName.includes(str))),
    );

  categoriesFiltered$ = this.normalizedSearch$
    .pipe(
      map(str => this.categories
        .map(category => new Category(
          category.name,
          category.scores.filter(score => str.length === 0 || score.searchableName.includes(str)))
          )
        ),
      map(cats => cats.filter(cat => cat.scores.length > 0)),
    );

  selectionType: SelectionType = SelectionType.Tree;
  changeSelectionTypeTo(type: SelectionType): void {
    this.selectionType = type;
  }
}
