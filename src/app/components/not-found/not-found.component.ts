import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { startPage } from '../../app.routes';

@Component({
  selector: 'app-not-found',
  imports: [
    RouterLink,
  ],
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.scss'
})
export class NotFoundComponent {
  readonly startPage = startPage;
}
