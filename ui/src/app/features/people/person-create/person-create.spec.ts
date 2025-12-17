import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonCreate } from './person-create';

describe('PersonCreate', () => {
  let component: PersonCreate;
  let fixture: ComponentFixture<PersonCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonCreate]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PersonCreate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
