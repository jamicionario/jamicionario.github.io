import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScoreDescriptionComponent } from './score-description.component';

describe('ScoreDescriptionComponent', () => {
  let component: ScoreDescriptionComponent;
  let fixture: ComponentFixture<ScoreDescriptionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScoreDescriptionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScoreDescriptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
