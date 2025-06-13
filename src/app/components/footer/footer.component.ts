import { Component } from '@angular/core';
import versionInfo from '@public/Jamicionario.metadata.json';
import sourceInfo from '@public/source-info.json';
import { TimeAgoPipe } from "@utils/time-ago.pipe";
import { SourceInfo } from '@models/source-info';
import { describeContributors } from '@components/about/about.component';

@Component({
  selector: 'app-footer',
  imports: [
    TimeAgoPipe,
  ],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  versionNumber = versionInfo.Version;
  versionDate = new Date(versionInfo.GenerationDate);

  readonly sourceInfo: SourceInfo = sourceInfo;

  get readableDate(): string {
    return `${this.versionDate.toDateString()} at ${this.versionDate.getUTCHours()}h`;
  }

  get contributors(): string {
    return describeContributors(this.sourceInfo.source.contributors);
  }

  get dataContributors(): string {
    return describeContributors(this.sourceInfo.data.contributors);
  }
}
