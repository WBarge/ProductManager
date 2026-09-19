import { TestBed } from '@angular/core/testing';
import { OptionService } from './option.service';

describe('OptionServiceService', () => {
  let service: OptionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OptionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
