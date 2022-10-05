import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SavedCareerProfilesComponent } from './saved-career-profiles.component';

describe('SavedCareerProfilesComponent', () => {
  let component: SavedCareerProfilesComponent;
  let fixture: ComponentFixture<SavedCareerProfilesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SavedCareerProfilesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SavedCareerProfilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
