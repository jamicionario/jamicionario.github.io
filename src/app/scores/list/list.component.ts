import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Score, ScoreGroup, ScoreService } from '../scores.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TreeComponent } from './tree/tree.component';
import { BehaviorSubject, debounceTime, distinctUntilChanged, map } from 'rxjs';

export enum SelectionType {
  List,
  Tree,
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

  scores: Score[] = this.service.getScores();
  groupedScores: ScoreGroup[] = this.service.getGroupedScores();

  scoresFiltered$ = this.search$
    .pipe(
      debounceTime(200),
      map(str => normalizeStringForSearch(str)),
      distinctUntilChanged(),
      map(str => this.scores.filter(score => str.length === 0 ||score.SearchableName.includes(str))),
    );

  selectionType: SelectionType = SelectionType.Tree;
  changeSelectionTypeTo(type: SelectionType): void {
    this.selectionType = type;
  }
}
