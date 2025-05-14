import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JamModeTogglerComponent } from './jam-mode-toggler.component';

describe('JamModeTogglerComponent', () => {
  let component: JamModeTogglerComponent;
  let fixture: ComponentFixture<JamModeTogglerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JamModeTogglerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JamModeTogglerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
