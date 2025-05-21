import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScoresSearchComponent } from './scores-search.component';

describe('ScoresSearchComponent', () => {
  let component: ScoresSearchComponent;
  let fixture: ComponentFixture<ScoresSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScoresSearchComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScoresSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
