import { Component } from '@angular/core';
import { TimeAgoPipe } from "@utils/time-ago.pipe";
import { VersionComponent } from '@components/shared/version-component';

@Component({
  selector: 'app-footer',
  imports: [
    TimeAgoPipe,
  ],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent extends VersionComponent {
}
