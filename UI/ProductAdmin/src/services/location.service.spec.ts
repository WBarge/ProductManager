import { TestBed } from '@angular/core/testing';

import { LocationService } from './location.service';
import { MessageService } from 'primeng/api';
import { createSpyFromClass } from 'jasmine-auto-spies';

describe('LocationServiceService', () => {
  let service: LocationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers:[
      {provide:MessageService, useValue: createSpyFromClass(MessageService)},
      LocationService
    ]
    });
  });

  it('should be created', () => {
    service = TestBed.inject(LocationService);
    expect(service).toBeTruthy();
  });
});
