import { Component } from '@angular/core';
import versionInfo from '@public/Jamicionario.metadata.json';

@Component({
  selector: 'app-download',
  imports: [
  ],
  templateUrl: './download.component.html',
  styleUrl: './download.component.scss'
})
export class DownloadComponent {
  versionNumber = versionInfo.Version;
  versionDate = new Date(versionInfo.GenerationDate);

  public get timeAgo(): string {
    const today = new Date();

    const years = today.getUTCFullYear() - this.versionDate.getUTCFullYear();
    const inYears = this.compare(years, 'year');
    if (inYears != null) {
      return inYears;
    }

    const months = today.getUTCFullYear() - this.versionDate.getUTCFullYear();
    const inMonths = this.compare(months, 'month');
    if (inMonths != null) {
      return inMonths;
    }

    const days = today.getUTCFullYear() - this.versionDate.getUTCFullYear();
    const inDays = this.compare(days, 'day');
    if (inDays != null) {
      return inDays;
    }

    return 'today';
  }

  private compare(count: number, datePartName: string): string | null {
    switch (count) {
      case 0:
        return null;
      case 1:
        return `${count} ${datePartName}`;
      default:
        return `${count} ${datePartName}s`;
    }
  }
}
