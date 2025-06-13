import { Component } from '@angular/core';
import sourceInfo from '@public/source-info.json';
import { RouterLink } from '@angular/router';
import { SourceInfo } from '@models/source-info';

@Component({
  selector: 'app-about',
  imports: [
    RouterLink,
  ],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss'
})
export class AboutComponent {
  readonly sourceInfo: SourceInfo = sourceInfo;

  get contributors(): string {
    return describeContributors(this.sourceInfo.source.contributors);
  }

  get dataContributors(): string {
    return describeContributors(this.sourceInfo.data.contributors);
  }
}

export function describeContributors(contributors: string[], cuttofLength: number = 3): string {
  if (contributors.length == 0) {
    return 'the community';
  }
  if (contributors.length <= cuttofLength) {
    return contributors.join(", ");
  }
  return contributors.slice(0, 3).join(", ") + `, and ${contributors.length - cuttofLength} more`;
}
