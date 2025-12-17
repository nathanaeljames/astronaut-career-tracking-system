import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DutyHistory } from './duty-history';

describe('DutyHistory', () => {
  let component: DutyHistory;
  let fixture: ComponentFixture<DutyHistory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DutyHistory]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DutyHistory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
