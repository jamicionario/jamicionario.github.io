import { TestBed } from '@angular/core/testing';

import { JamModeService } from './jam-mode.service';

describe('JamModeService', () => {
  let service: JamModeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JamModeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
