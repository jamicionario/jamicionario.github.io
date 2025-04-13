import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Score, ScoreGroup, ScoreService } from '../scores.service';
import { CommonModule } from '@angular/common';
import { TreeComponent } from './tree/tree.component';

export enum SelectionType {
  List,
  Tree,
}

@Component({
  selector: 'app-list',
  imports: [
    RouterOutlet,
    CommonModule,
    TreeComponent,
  ],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent {
  private readonly service = inject(ScoreService);
  readonly SelectionType = SelectionType;

  scores: Score[] = this.service.getScores();
  groupedScores: ScoreGroup[] = this.service.getGroupedScores();

  selectionType: SelectionType = SelectionType.Tree;
  changeSelectionTypeTo(type: SelectionType) : void {
    this.selectionType = type;
  }
}
