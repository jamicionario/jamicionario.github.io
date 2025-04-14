import { Routes } from '@angular/router';
import { ListComponent } from './scores/list/list.component';
import { DetailsComponent } from './scores/details/details.component';

export const routes: Routes = [
    { path: '', title: 'Jamicionário', component: ListComponent, pathMatch: 'full' },
    { path: 'scores', redirectTo: '', pathMatch: 'full', },
    { path: 'scores/:number', title: 'Score details', component: DetailsComponent },
    // Redirect any broken paths back to the main page.
    { path: '**', redirectTo: '/scores' },
];
