import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SavedIndustryProfilesComponent } from './saved-industry-profiles.component';

describe('SavedIndustryProfilesComponent', () => {
  let component: SavedIndustryProfilesComponent;
  let fixture: ComponentFixture<SavedIndustryProfilesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SavedIndustryProfilesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SavedIndustryProfilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
