import { Routes } from '@angular/router';
import { ListComponent } from './scores/list/list.component';
import { DetailsComponent } from './scores/details/details.component';

export const routes: Routes = [
    { path: '', redirectTo: '/scores', pathMatch: 'full', },
    { path: 'scores', title: 'Jamicionário', component: ListComponent,
        children: [
            { path: ':id', title: 'Score details', component: DetailsComponent },
        ],
    },
    // Redirect any broken paths back to the main page.
    { path: '**', redirectTo: '/scores' },
];
