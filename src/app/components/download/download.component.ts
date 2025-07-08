import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VersionComponent } from '@components/shared/version-component';
import { TimeAgoPipe } from "@utils/time-ago.pipe";

@Component({
  selector: 'app-download',
  imports: [
    RouterLink,
    TimeAgoPipe
],
  templateUrl: './download.component.html',
  styleUrl: './download.component.scss'
})
export class DownloadComponent extends VersionComponent {
}
