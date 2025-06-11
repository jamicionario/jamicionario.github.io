import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CategoryCardComponent } from '@components/categories/category-card/category-card.component';
import { Category } from '@models/category';
import { CategoriesService } from '@services/categories.service';
import { PluralizePipe } from '@utils/pluralize.pipe';

@Component({
  selector: 'app-list-categories',
  imports: [
    RouterLink,
    CategoryCardComponent,
    PluralizePipe,
  ],
  templateUrl: './list-categories.component.html',
  styleUrl: './list-categories.component.scss'
})
export class ListCategoriesComponent {
  private readonly categoriesService = inject(CategoriesService);

  categories: Category[] = this.categoriesService.getAll();
}
