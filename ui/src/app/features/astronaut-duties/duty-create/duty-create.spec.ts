import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DutyCreate } from './duty-create';

describe('DutyCreate', () => {
  let component: DutyCreate;
  let fixture: ComponentFixture<DutyCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DutyCreate]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DutyCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
