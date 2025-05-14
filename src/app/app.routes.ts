import { Routes } from '@angular/router';
import { ListComponent } from './components/scores/list/list.component';
import { DetailsComponent } from './components/scores/details/details.component';
import { ListCategoriesComponent } from '@components/categories/list-categories/list-categories.component';
import { CategoryDetailsComponent } from '@components/categories/category-details/category-details.component';
import { DownloadComponent } from '@components/download/download.component';
import { AboutComponent } from '@components/about/about.component';

export const routes: Routes = [
    { path: '', title: 'Categories', component: ListCategoriesComponent },
    { path: 'scores', title: 'All scores', component: ListComponent },
    { path: 'scores/:number', title: 'Score details', component: DetailsComponent },
    { path: 'categories', title: 'Categories', component: ListCategoriesComponent },
    { path: 'categories/:name', title: 'Scores for a category', component: CategoryDetailsComponent },
    { path: 'download', title: 'Download Jamicionário', component: DownloadComponent },
    { path: 'about', title: 'About Jamicionário', component: AboutComponent },
    // Redirect any broken paths back to the main page.
    // { path: '**', redirectTo: '' },
];
