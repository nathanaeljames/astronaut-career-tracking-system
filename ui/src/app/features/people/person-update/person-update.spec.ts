import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonUpdate } from './person-update';

describe('PersonUpdate', () => {
  let component: PersonUpdate;
  let fixture: ComponentFixture<PersonUpdate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonUpdate]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PersonUpdate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
