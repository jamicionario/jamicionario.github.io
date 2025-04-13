import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ScoreGroup, ScoreService } from '../scores.service';
import { CommonModule } from '@angular/common';
import { TreeComponent } from './tree/tree.component';

@Component({
  selector: 'app-list',
  imports: [
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    CommonModule,
    TreeComponent,
  ],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss'
})
export class ListComponent {
  private readonly service = inject(ScoreService);

  groupedScores: ScoreGroup[] = this.service.getGroupedScores();
}
