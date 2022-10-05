import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PersonalSettingsComponent } from './personal-settings.component';

describe('PersonalSettingsComponent', () => {
  let component: PersonalSettingsComponent;
  let fixture: ComponentFixture<PersonalSettingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PersonalSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonalSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
