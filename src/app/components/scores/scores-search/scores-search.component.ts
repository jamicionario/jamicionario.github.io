import { Component, inject, Input, Output, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { ScoreLabel } from '@models/score-label';
import { LabelsService } from '@services/labels.service';
import { normalizeStringForSearch } from '@utils/score-filtering';
import { BehaviorSubject, combineLatest, debounceTime, distinctUntilChanged, map, Observable, scan, startWith, Subject } from 'rxjs';

export interface FilterValue {
  /**
   * The text searched, or empty.
   */
  text: string;
  /**
   * The active search labels. Empty if none are being filtered.
   */
  labels: LabelFilter[];
}
/**
 * An active search filter for a label.
 * Note: never null, only exists if a particular filter is active.
 */
export type LabelFilter = {
  name: string;
  value: string;
};
/**
 * A currently-selected value for a label filter: string OR undefined.
 */
type SelectedLabelValue = {
  /**
   * The name of the label.
   */
  name: string;
  value: string | undefined;
};

@Component({
  selector: 'app-scores-search',
  imports: [
    MatFormFieldModule,
    MatSelectModule,
    FormsModule,
  ],
  templateUrl: './scores-search.component.html',
  styleUrl: './scores-search.component.scss'
})
export class ScoresSearchComponent {
  @Output()
  public searched: Observable<FilterValue>;

  private readonly labelsService = inject(LabelsService);

  allLabels: ScoreLabel[] = this.labelsService.getAll();

  searchText: string = '';
  searchText$ = new BehaviorSubject<string>('');
  someLabelFiltered$ = new Subject<SelectedLabelValue>();

  constructor() {
    this.searched = buildProcessedTextSearch(
      this.searchText$,
      this.someLabelFiltered$,
    );
  }

  searchTextChanged($event: string) {
    this.searchText$.next($event);
  }
  filterChanged(filter: ScoreLabel, $event: MatSelectChange<string | undefined>) {
    this.someLabelFiltered$.next({ name: filter.name, value: $event.value });
  }
}

function buildProcessedTextSearch(
  searchText$: BehaviorSubject<string>,
  someLabelFiltered$: Subject<SelectedLabelValue>
): Observable<FilterValue> {
  const processedTextSearch$ = searchText$.pipe(
    debounceTime(200),
    map(str => normalizeStringForSearch(str)),
    distinctUntilChanged(),
  );
  const seed: LabelFilter[] = [];
  const processedLabels$ = someLabelFiltered$.pipe(
    scan((accumulated: LabelFilter[], current: SelectedLabelValue) => {
      // If value is removed, just remove it from the accumulated.
      if (current.value == null) {
        return accumulated.filter(labelFilter => labelFilter.name != current.name);
      }
      // Else, add or update value.
      const existing = accumulated.find(label => label.name == current.name);
      if (existing != null) {
        existing.value = current.value;
      } else {
        accumulated.push({
          name: current.name,
          value: current.value,
        });
      }
      return accumulated;
    }, seed),
    // This allows emitting a value with text search even if no labels have been selected.
    // Since that one is a Subject, and not a BehaviorSubject...
    startWith(seed),
  );
  const combined = combineLatest([
    processedTextSearch$,
    processedLabels$,
  ]);
  return combined.pipe(
    map(([searchText, filteredLabels]) => <FilterValue>{
      text: searchText,
      labels: filteredLabels
    }),
  );
}
