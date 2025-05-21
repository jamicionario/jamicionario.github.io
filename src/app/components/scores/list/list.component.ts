import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ScoresService } from '@services/scores.service';
import { Score } from '@models/score';
import { ScoreGroup } from '@models/score-group';
import { Category } from '@models/category';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TreeComponent } from './tree/tree.component';
import { map, ReplaySubject } from 'rxjs';
import { CategoriesService } from '@services/categories.service';
import { FilterValue, ScoresSearchComponent } from '../scores-search/scores-search.component';
import { filterCategories, filterScoreGroups, filterScores } from '@utils/score-filtering';

export enum SelectionType {
  List = 'list',
  Tree = 'tree',
  Categories = 'categories',
}

@Component({
  selector: 'app-list',
  imports: [
    RouterLink,
    CommonModule,
    FormsModule,
    TreeComponent,
    ScoresSearchComponent,
  ],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent {
  readonly SelectionType = SelectionType;

  private readonly scoreService = inject(ScoresService);
  private readonly categoriesService = inject(CategoriesService);

  private readonly allScores: Score[] = this.scoreService.getAll();
  private readonly allCategories: Category[] = this.categoriesService.getAll();
  private readonly allGroupedScores: ScoreGroup[] = this.scoreService.getGroupedScores();

  private readonly filter$ = new ReplaySubject<FilterValue>();

  scoresFiltered$ = this.filter$
    .pipe(
      map(search => filterScores(search, this.allScores)),
    );

  categoriesFiltered$ = this.filter$
    .pipe(
      map(search => filterCategories(search, this.allCategories)),
    );

  groupedScoresFiltered$ = this.filter$
    .pipe(
      map(search => filterScoreGroups(search, this.allGroupedScores)),
    );

  selectionType: SelectionType = SelectionType.Categories;
  changeSelectionTypeTo(type: SelectionType): void {
    this.selectionType = type;
  }
  searchChanged(value: FilterValue) {
    console.log('search changed to:', value);
    this.filter$.next(value);
  }
}
