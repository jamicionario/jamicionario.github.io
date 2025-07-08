
import { describeContributors } from '@components/about/about.component';
import { SourceInfo } from '@models/source-info';
import versionInfo from '@public/Jamicionario.metadata.json';
import sourceInfo from '@public/source-info.json';

export class VersionComponent {
  public readonly versionNumber = versionInfo.Version;
  public readonly versionDate = new Date(versionInfo.GenerationDate);

  public readonly sourceInfo: SourceInfo = sourceInfo;

  public get readableDate(): string {
    // Formats the date like "25 April 2025".
    return this.versionDate.toLocaleDateString("en-UK", {
        dateStyle: "long"
    });
  }

  public get contributors(): string {
    return describeContributors(this.sourceInfo.source.contributors);
  }

  public get dataContributors(): string {
    return describeContributors(this.sourceInfo.data.contributors);
  }
}