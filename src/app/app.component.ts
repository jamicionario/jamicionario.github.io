import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  readonly pages = [
    {title: 'Home', url: '/categories'},
    {title: 'Search', url: '/scores'},
    {title: 'Download', url: '/download'},
    {title: 'About', url: '/about'},
  ];
}
